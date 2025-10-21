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
            for (int i = (LOOKBACK + 1); i < marketDays.Count; i++)
            {
                var currentDay = marketDays[i];
                var backlogDays = marketDays.Where(x => x.GetTradingOpenTimeUtc() < currentDay.GetSessionOpenTimeUtc()).TakeLast(LOOKBACK + 1).ToList();
                _logger.LogInformation($"Processing day {currentDay.GetTradingOpenTimeUtc().ToShortDateString()}");
                capitol = await ProcessDay(assets, currentDay, backlogDays, capitol);
                _logger.LogInformation($"Running Capitol: {capitol}");
                _logger.NewLine();
            }

            _logger.LogInformation($"TOTAL CAPITOL: {capitol}");
            _logger.LogInformation($"TOTAL PROFIT: {capitol - 25000}");
        }

        public async Task<decimal> ProcessDay(List<IAsset> assets, IIntervalCalendar currentDay, List<IIntervalCalendar> backlogDays, decimal capitolToTrade)
        {
            var screenedAssets = await _screener.ScreenTrueFade(assets, currentDay.GetTradingOpenTimeUtc(), backlogDays, 3);
            screenedAssets = screenedAssets.Take(100);

            if (!screenedAssets.Any())
                return capitolToTrade;

            var allocatedRecords = TrueFadeAllocater.Allocate(screenedAssets, capitolToTrade);

            var tradeData = await _dataLayer.GetAggregateDataMulti(
                allocatedRecords.Select(x => x.Symbol),
                backlogDays.Last().GetTradingOpenTimeUtc(),
                currentDay.GetTradingCloseTimeUtc(),
                BarTimeFrame.Day);

            decimal dailyProfit = 0;
            foreach (var record in allocatedRecords)
            {
                decimal profit = 0;
                if (tradeData.TryGetValue(record.Symbol, out var data))
                {
                    if (data.Count == 0)
                        continue;

                    var bar = data.Last();

                    if (bar.High >= bar.Open + record.AverageTrueRange)
                    {
                        profit -= record.AverageTrueRange;
                    }
                    else
                    {
                        profit += bar.Open - bar.Close;
                    }

                    _logger.LogInformation($"{record.Symbol} {record.Price} ({record.Price * record.PositionSize}) ATR Multiple: {record.MultipleATR} Position: {record.PositionSize} Profit: {profit} ({profit * record.PositionSize})");
                    dailyProfit += profit * record.PositionSize;
                }     
            }

            _logger.LogInformation($"Daily Profit: {dailyProfit}");
            return capitolToTrade + dailyProfit;
        }
    }
}
