using Alpaca.Markets;
using Denali.Processors.TrueFadeStrategy;
using Denali.Services;
using InteractiveBrokers.Services;
using Microsoft.Extensions.Logging;
using Denali.Shared.Extensions;
using Denali.Processors.DenaliClimbStrategy;
using InteractiveBrokers.Models.Response;

namespace Denali.Processors.GapUpFadeStrategy
{
    public class GapUpFadeHistoricProcessor
    {
        private readonly DataLayerComponent _dataLayer;
        private readonly IInteractiveBrokersService _ibService;
        private readonly GapUpScreener _gapUpScreener;
        private readonly GapUpFadeAllocator _gapUpFadeAllocator;
        private readonly ILogger _logger;

        public GapUpFadeHistoricProcessor(DataLayerComponent dataLayer, IInteractiveBrokersService ibService, GapUpScreener gapUpScreener, GapUpFadeAllocator gapUpFadeAllocator, ILogger<GapUpFadeHistoricProcessor> logger)
        {
            _dataLayer = dataLayer;
            _ibService = ibService;
            _gapUpScreener = gapUpScreener;
            _gapUpFadeAllocator = gapUpFadeAllocator;
            _logger = logger;
        }

        public async Task ProcessRange(DateTime start, DateTime end)
        {
            await _ibService.InitializeHttpAuth();
            _logger.NewLine();
            await _dataLayer.Initialize();
            _logger.NewLine();

            _logger.LogInformation("Fetching account info...");
            var account = await _ibService.GetAccounts();
            _logger.LogInformation($"Account Id: {account.selectedAccount}, IsPaper: {account.isPaper}");
            _logger.NewLine();

            var assets = await GetAssetUniverse();

            var workingDays = await _dataLayer.GetMarketDays(start.AddDays(-(4)), end);
            var lookbackDays = workingDays.Where(x => x.GetTradingOpenTimeUtc().Date < start.Date);
            var forwardDays = workingDays.Where(x => x.GetTradingOpenTimeUtc().Date >= start.Date);

            List<IIntervalCalendar> marketDays = new List<IIntervalCalendar>();
            marketDays.AddRange(lookbackDays.Last());
            marketDays.AddRange(forwardDays);

            decimal capitolToTrade = 25000;
            List<DailyResult> results = new List<DailyResult>();

            for (int i = 1; i < marketDays.Count; i++)
            {
                var currentDay = marketDays[i];
                var previousDay = marketDays[i-1];
                _logger.LogInformation($"Processing day {currentDay.GetTradingOpenTimeUtc().ToShortDateString()}");
                var result = await ProcessGapUp(assets, currentDay, previousDay, capitolToTrade);
                capitolToTrade = result.RunningCapital;
                results.Add(result);
            }

            await WriteBaseLine(marketDays[1].GetTradingOpenTimeUtc(), marketDays.Last().GetSessionCloseTimeUtc(), 25000);
        }

        public async Task<AssetContext> GetAssetUniverse()
        {
            _logger.LogInformation("Fetching Alpaca assets...");
            var alpacaAssets = await _dataLayer.GetAllTradableAssets();
            _logger.LogInformation($"Alapca assets count: {alpacaAssets.Count}");
            _logger.LogInformation("Fetching IB assets...");
            var ibAssets = await _ibService.GetContractsByExchanges("NASDAQ", "NYSE");
            _logger.LogInformation($"IB assets count: {ibAssets.Count}");
            var ibTickers = ibAssets.Select(x => x.ticker).ToList();
            var mergedSymbols = alpacaAssets.Where(x => ibTickers.Contains(x.Symbol)).Select(x => x.Symbol).ToList();
            _logger.LogInformation($"Merged assets count: {mergedSymbols.Count()}");
            _logger.NewLine();

            return new AssetContext(alpacaAssets, ibAssets, mergedSymbols);
        }

        public async Task WriteBaseLine(DateTime start, DateTime end, decimal capital)
        {
            var baseLineDataRequest = await _dataLayer.GetAggregateDataMulti(new List<string> { "VTI" }, start, end, BarTimeFrame.Day);
            var data = baseLineDataRequest["VTI"];
            decimal positionSize = capital / data.First().Open;

            List<DailyResult> results = new List<DailyResult>();
            foreach (var datum in data)
            {
                var result = new DailyResult(datum.TimeUtc, 0, 0, 0, positionSize * datum.Open);
                results.Add(result);
            }
            _logger.LogInformation($"Baseline: {results.Last().RunningCapital}");
            //await _fileService.WriteJSONResourceToFile($"TrueFadeBaseLine_{start.Year}-{start.Month}-{start.Day}_{end.Year}-{end.Month}-{end.Day}.JSON", results);

        }

        public async Task<DailyResult> ProcessGapUp(AssetContext assets, IIntervalCalendar currentDay, IIntervalCalendar previousDay, decimal capitalToTrade)
        {
            var screenedAssets = await _gapUpScreener.GetGapUp(
                previousDay.GetTradingCloseTimeUtc(), 
                currentDay.GetTradingOpenTimeUtc().AddMinutes(-3), 
                assets.MergedSymbols, 
                3m, 
                4m,
                500m,
                true);

            var spy = await _gapUpScreener.GetGapUp(
                previousDay.GetTradingCloseTimeUtc(),
                currentDay.GetTradingOpenTimeUtc().AddMinutes(-3),
                new List<string> { "SPY" },
                0m,
                -1000m,
                10000m,
                true);

            screenedAssets = screenedAssets.Take(30);

            var mergedAssets = screenedAssets.Join(assets.IBAssets, x => x.Symbol, y => y.ticker, (x, y) =>
            {
                x.IBContract = y;
                return x;
            }).ToList();


            var positions = await _gapUpFadeAllocator.Allocate(mergedAssets, currentDay, capitalToTrade);

            var data = await _dataLayer.GetAggregateDataMulti(
                mergedAssets.Select(x => x.Symbol), 
                currentDay.GetTradingOpenTimeUtc(), 
                currentDay.GetTradingOpenTimeUtc(), 
                BarTimeFrame.Minute);


            foreach (var position in positions)
            {
                if (!data.TryGetValue(position.Signal.Symbol, out var barList))
                    continue;

                var bar = barList.FirstOrDefault();
                if (bar != null)
                {
                    if (spy.First().GapUpPercentage < 0)
                    {
                        position.PerStockProfit = bar.Open - bar.Close;
                    }
                    else
                    {
                        position.PerStockProfit = bar.Close - bar.Open;
                    }

                        position.Price = bar.Open;
                    position.TotalPrice = position.Price * position.PositionSize; // OPG order gets opening price
                    position.Commision = Math.Min(position.TotalPrice * 0.01m, Math.Max(0.0035m * position.PositionSize, 0.35m));
                    position.TotalProfit = position.PerStockProfit * position.PositionSize;
                    position.GrossProfit = position.TotalProfit - position.Commision;
                }
            }

            var runningCapital = (capitalToTrade += positions.Sum(x => x.GrossProfit)).RoundToMoney();
            var result = new DailyResult(
                currentDay.GetTradingOpenTimeUtc(),
                positions.Sum(x => x.TotalPrice),
                positions.Sum(x => x.Commision),
                positions.Sum(x => x.GrossProfit),
                runningCapital);

            _logger.NewLine();
            _logger.LogInformation($"Daily Capital Traded: {result.CapitalTraded}");
            _logger.LogInformation($"Daily Commision: {result.TotalCommision}");
            _logger.LogInformation($"Daily Gross Profit: {result.GrossProfit}");
            _logger.LogInformation($"Running Capitol: {result.RunningCapital}");
            _logger.NewLine();
            return result;
        }
    }

    public record GapUpFadeSignal
    {
        public string Symbol { get; set; }
        public decimal LastPrice { get; set; }
        public decimal GapUpPercentage { get; set; }
        public Contract IBContract { get; set; }
    }
}
