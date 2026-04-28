using Alpaca.Markets;
using Denali.Models.Alpaca;
using Denali.Processors.DenaliClimbStrategy;
using Denali.Services;
using Denali.Shared.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

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
            await WriteBaseLine(start, end, _settings.CapitalToTrade);

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
                var result = await ProcessDayConfirm(assets, marketContext, capitolToTrade);
                capitolToTrade = result.RunningCapital;
                results.Add(result);
            }

            _logger.LogInformation($"Final result {capitolToTrade}");

            await _fileService.WriteJSONResourceToFile($"TrueFadeBacktest_{start.Year}-{start.Month}-{start.Day}_{end.Year}-{end.Month}-{end.Day}.JSON", results);
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
            await _fileService.WriteJSONResourceToFile($"BaseLine{start.Year}-{start.Month}-{start.Day}_{end.Year}-{end.Month}-{end.Day}.JSON", results);

        }

        public async Task<DailyResult> ProcessDay(AssetContext assets, MarketDayContext marketDayContext, decimal capitolToTrade)
        {
            var positions = await _processor.ScreenAssets(assets, marketDayContext, capitolToTrade, logPositions: false);
            if (!positions.Any())
                return new DailyResult(marketDayContext.Today.GetSessionOpenTimeUtc(), 0, 0, 0, capitolToTrade);
            _logger.NewLine();

            var tradeData = await _dataLayer.GetAggregateDataMulti(
                positions.Select(x => x.Signal.Symbol),
                marketDayContext.LookbackDays.Last().GetTradingOpenTimeUtc(),
                marketDayContext.Today.GetTradingOpenTimeUtc(),
                BarTimeFrame.Day);

            foreach (var position in positions)
            {
                if (!tradeData.TryGetValue(position.Signal.Symbol, out var data))
                    continue;

                var bar = data.Last();

                var stopLoss = bar.Open + position.Signal.AverageTrueRange;
                // Pessimistic stoploss
                if (bar.High >= stopLoss)
                {
                    position.PerStockProfit -= stopLoss - bar.Open;
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

        public async Task<DailyResult> ProcessDayConfirm(AssetContext assets, MarketDayContext marketDayContext, decimal capitolToTrade)
        {
            var positions = await _processor.ScreenAssets(assets, marketDayContext, capitolToTrade, logPositions: false);
            if (!positions.Any())
                return new DailyResult(marketDayContext.Today.GetSessionOpenTimeUtc(), 0, 0, 0, capitolToTrade);
            _logger.NewLine();

            var tradeData = await _dataLayer.GetAggregateDataMulti(
                positions.Select(x => x.Signal.Symbol),
                marketDayContext.LookbackDays.Last().GetTradingCloseTimeUtc(),
                marketDayContext.Today.GetTradingCloseTimeUtc(),
                BarTimeFrame.Minute);

            foreach (var position in positions)
            {
                if (!tradeData.TryGetValue(position.Signal.Symbol, out var data))
                    continue;

                if (data.Count < 2)
                    continue;

                var openingRangeTime = marketDayContext.Today.GetTradingOpenTimeUtc().AddMinutes(14);
                var firstfifteenBars = data.Where(x => x.TimeUtc >= marketDayContext.Today.GetTradingOpenTimeUtc() && x.TimeUtc <= openingRangeTime);
                if (!firstfifteenBars.Any())
                    continue;

                var red = firstfifteenBars.Last().Close < firstfifteenBars.First().Open;
                var dayBars = data.Where(x => x.TimeUtc > openingRangeTime).ToList();
                var openBar = dayBars.FirstOrDefault();

                if (!red || openBar == null)
                    continue;

                decimal entryPrice = dayBars.First().Open;

                // Fixed stop loss (for short trades)
                decimal stopLoss = firstfifteenBars.Max(x => x.High);

                // 0.5% trailing stop (starts inactive until trade is profitable)
                decimal trailingStopLoss = stopLoss;

                decimal profit = 0;
                decimal lowestPrice = entryPrice; // track lowest point since entry

                foreach (var bar in dayBars)
                {
                    // Update the lowest price reached since entry
                    if (bar.Low < lowestPrice)
                        lowestPrice = bar.Low;

                    // Update trailing stop (0.5% above lowest price)
                    decimal newTrailingStop = lowestPrice * 1.005m; // 0.5% trailing stop for a short
                    if (newTrailingStop < trailingStopLoss)
                        trailingStopLoss = newTrailingStop;

                    // Check if stop loss is hit (price goes against short position)
                    if (bar.High >= stopLoss)
                    {
                        profit = entryPrice - stopLoss; // negative value = loss
                        break;
                    }

                    // Check if trailing stop is hit
                    if (bar.High >= trailingStopLoss)
                    {
                        profit = entryPrice - trailingStopLoss;
                        break;
                    }

                    // Optionally: define a take profit or exit rule if desired
                    if (bar.TimeUtc == dayBars.Last().TimeUtc)
                        profit = entryPrice - bar.Close;
                }

                position.PerStockProfit = profit;
                position.Price = openBar.Open;
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
