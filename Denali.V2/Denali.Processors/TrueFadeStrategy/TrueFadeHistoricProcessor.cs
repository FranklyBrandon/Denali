using Alpaca.Markets;
using Denali.Services;
using Denali.Shared.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors.TrueFadeStrategy
{
    public record DailyRecord(decimal profit, decimal capitol);
    public class TrueFadeHistoricProcessor
    {
        private readonly TrueFadeScreener _screener;
        private readonly DataLayerComponent _dataLayer;
        private readonly ILogger _logger;

        private const int LOOKBACK = 10;

        public TrueFadeHistoricProcessor(TrueFadeScreener screener, DataLayerComponent dataLayer, ILogger<TrueFadeHistoricProcessor> logger)
        {
            _screener = screener;
            _dataLayer = dataLayer;
            _logger = logger;
        }

        public async Task ProcessRange(DateTime start, DateTime end)
        {
            var assets = await _dataLayer.GetAllTradableAssets();

            var workingDays = await _dataLayer.GetMarketDays(start.AddDays(-(LOOKBACK * 2)), end);
            var lookbackDays = workingDays.Where(x => x.GetTradingOpenTimeUtc().Date < start.Date);
            var forwardDays = workingDays.Where(x => x.GetTradingOpenTimeUtc().Date >= start.Date);

            List<IIntervalCalendar> marketDays = new List<IIntervalCalendar>();
            marketDays.AddRange(lookbackDays.TakeLast(LOOKBACK + 1));
            marketDays.AddRange(forwardDays);

            decimal capitol = 25000m;
            List<DailyResult> results = new List<DailyResult>();
            for (int i = (LOOKBACK + 1); i < marketDays.Count; i++)
            {
                var currentDay = marketDays[i];
                var backlogDays = marketDays.Where(x => x.GetTradingOpenTimeUtc() < currentDay.GetSessionOpenTimeUtc()).TakeLast(LOOKBACK + 1).ToList();
                _logger.LogInformation($"Processing day {currentDay.GetTradingOpenTimeUtc().ToShortDateString()}");
                var result = await ProcessDay(assets, currentDay, backlogDays, capitol);
                capitol = result.RunningCapital;
                results.Add(result);
                _logger.LogInformation($"Running Capitol: {capitol}");
                _logger.NewLine();
            }

            _logger.LogInformation($"TOTAL CAPITOL: {capitol}");
            _logger.LogInformation($"TOTAL PROFIT: {capitol - 25000}");
        }

        public async Task<DailyResult> ProcessDay(List<IAsset> assets, IIntervalCalendar currentDay, List<IIntervalCalendar> backlogDays, decimal capitolToTrade)
        {
            var screenedAssets = await _screener.ScreenTrueFade(assets, currentDay.GetTradingOpenTimeUtc(), backlogDays, 3, 100);

            var result = new DailyResult(currentDay.GetTradingOpenTimeUtc());

            if (!screenedAssets.Any())
                return result;

            var allocatedRecords = TrueFadeAllocater.Allocate(screenedAssets, capitolToTrade, 3m);

            var tradeData = await _dataLayer.GetAggregateDataMulti(
                allocatedRecords.Select(x => x.Signal.Symbol),
                backlogDays.Last().GetTradingOpenTimeUtc(),
                currentDay.GetTradingCloseTimeUtc(),
                BarTimeFrame.Day);

            foreach (var record in allocatedRecords)
            {
                if (tradeData.TryGetValue(record.Signal.Symbol, out var data))
                {
                    if (data.Count == 0)
                        continue;

                    var bar = data.Last();

                    // Pessimistic stoploss
                    if (bar.High >= bar.Open + record.Signal.AverageTrueRange)
                    {
                        record.PerStockProfit -= record.Signal.AverageTrueRange;
                    }
                    else
                    {
                        record.PerStockProfit += bar.Open - bar.Close;
                    }

                    record.TotalProfit = record.PerStockProfit * record.Signal.PositionSize;
                    _logger.LogInformation($"{record.Signal.Symbol} {record.Signal.EstimatedPrice}, Position size: {record.Signal.PositionSize}, Average volume {bar.Volume}, ATR: {record.Signal.AverageTrueRange}, ATR Multiple: {record.Signal.MultipleATR}, Per stock profit: {record.PerStockProfit}, Total profit: {record.TotalProfit}");
                    result.TotalProfit += record.TotalProfit;
                }     
            }

            _logger.LogInformation($"Daily Cost: {result.TotalCost}");
            _logger.LogInformation($"Daily Profit: {result.TotalProfit}");
            result.RunningCapital = capitolToTrade + result.TotalProfit;
            return result;
        }
    }
}
