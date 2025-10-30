using Alpaca.Markets;
using Denali.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Denali.Shared.Extensions;

namespace Denali.Processors.TrueFadeStrategy
{
    public class TrueFadeIBHistoricProcessor
    {
        private readonly TrueFadeIBProcessor _processor;
        private readonly DataLayerComponent _dataLayer;
        private readonly TrueFadeStrategySettings _settings;
        private readonly FileService _fileService;
        private readonly ILogger _logger;

        public TrueFadeIBHistoricProcessor(
            TrueFadeIBProcessor processor,
            DataLayerComponent dataLayer,
            IOptions<TrueFadeStrategySettings> settings,
            FileService fileService,
            ILogger<TrueFadeIBHistoricProcessor> logger
            )
        {
            _processor = processor;
            _dataLayer = dataLayer;
            _settings = settings.Value;
            _fileService = fileService;
            _logger = logger;
        }

        public async Task ProcessRange(DateTime start, DateTime end)
        {
            await _processor.Initialize();
            var assets = await _processor.GetAssetUniverse();

            var workingDays = await _dataLayer.GetMarketDays(start.AddDays(-(_settings.LookBackMarketDays * 2)), end);
            var lookbackDays = workingDays.Where(x => x.GetTradingOpenTimeUtc().Date < start.Date);
            var forwardDays = workingDays.Where(x => x.GetTradingOpenTimeUtc().Date >= start.Date);

            List<IIntervalCalendar> marketDays = new List<IIntervalCalendar>();
            marketDays.AddRange(lookbackDays.TakeLast(_settings.LookBackMarketDays + 1));
            marketDays.AddRange(forwardDays);

            decimal capitolToTrade = _settings.CapitalToTrade;
            List<DailyResult> results = new List<DailyResult>();

            for (int i = (_settings.LookBackMarketDays + 1); i < marketDays.Count; i++)
            {
                var currentDay = marketDays[i];
                var backlogDays = marketDays.Where(x => x.GetTradingOpenTimeUtc() < currentDay.GetSessionOpenTimeUtc()).TakeLast(_settings.LookBackMarketDays + 1).ToList();
                var marketContext = new MarketDayContext(currentDay, backlogDays);
                _logger.LogInformation($"Processing day {currentDay.GetTradingOpenTimeUtc().ToShortDateString()}");
                var result = await ProcessDay(assets, marketContext, capitolToTrade);
                capitolToTrade = result.RunningCapital;
                results.Add(result);
            }

            await _fileService.WriteJSONResourceToFile($"TrueFadeBacktest_{start.Year}-{start.Month}-{start.Day}_{end.Year}-{end.Month}-{end.Day}", results);
        }

        public async Task<DailyResult> ProcessDay(AssetContext assets, MarketDayContext marketDayContext, decimal capitolToTrade)
        {
            var positions = await _processor.ScreenAssets(assets, marketDayContext, capitolToTrade, logPositions: false);
            _logger.NewLine();

            var tradeData = await _dataLayer.GetAggregateDataMulti(
                positions.Select(x => x.Signal.Symbol),
                marketDayContext.LookbackDays.Last().GetTradingOpenTimeUtc(),
                marketDayContext.Today.GetTradingCloseTimeUtc(),
                BarTimeFrame.Day);

            foreach (var position in positions)
            {
                if (!tradeData.TryGetValue(position.Signal.Symbol, out var data))
                    continue;

                var bar = data.Last();

                // Pessimistic stoploss
                if (bar.High >= bar.Open + position.Signal.AverageTrueRange)
                {
                    position.PerStockProfit -= position.Signal.AverageTrueRange;
                }
                else
                {
                    position.PerStockProfit += bar.Open - bar.Close;
                }

                position.Price = bar.Open;
                position.TotalPrice = position.Price * position.PositionSize; // OPG order gets opening price
                position.Commision = Math.Min(position.TotalPrice * 0.01m, Math.Max(0.0035m * position.PositionSize, 0.35m));
                position.TotalProfit = position.PerStockProfit * position.PositionSize;
                position.GrossProfit = position.TotalProfit - position.Commision;

                _logger.LogInformation($"{position.Signal.Symbol} {position.Price} Total Cost: {position.TotalPrice} Commision: {position.Commision} Gross Profit: {position.GrossProfit}      Position size: {position.PositionSize}, Average volume {position.Signal.AverageVolume}, ATR: {position.Signal.AverageTrueRange}, ATR Multiple: {position.Signal.MultipleATR}");
            }

            var runningCapital = (capitolToTrade += positions.Sum(x => x.GrossProfit)).RoundToMoney();
            var result = new DailyResult(
                marketDayContext.Today.GetTradingOpenTimeUtc(), 
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
}

public record DailyResult(DateTime Day, decimal CapitalTraded, decimal TotalCommision, decimal GrossProfit, decimal RunningCapital);
