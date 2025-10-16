using Alpaca.Markets;
using Denali.Services;
using Denali.Services.Extensions;
using Denali.Shared.Extensions;
using Denali.TechnicalAnalysis;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public record RangeRecord(string Symbol, decimal multipleATR);

    public class TrueFadeStrategy
    {
        private readonly DataLayerComponent _dataLayer;
        private readonly ILogger _logger;

        private const int LOOKBACK = 14;

        public TrueFadeStrategy(DataLayerComponent dataLayer, ILogger<TrueFadeStrategy> logger)
        {
            _dataLayer = dataLayer;
            _logger = logger;
        }

        public async Task ProcessRange(DateTime startDate, DateTime endDate, CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Denali Descent HISTORIC RUN from {startDate.ToShortDateString()} to {endDate.ToShortDateString()}");
            await _dataLayer.Initialize();

            var assets = await _dataLayer.GetAllTradableAssets();

            var pastDays = await _dataLayer.GetPastMarketDays(startDate.AddDays(-1), 4);
            var forwardDays = await _dataLayer.GetMarketDays(startDate, endDate);
            List<IIntervalCalendar> marketDays = new() { pastDays.Last() };
            marketDays.AddRange(forwardDays);

            for (int i = 1; i < marketDays.Count; i++)
            {
                var currentDay = marketDays[i];
                var previousDay = marketDays[i - 1];
                var backlogStart = previousDay.GetTradingOpenTimeUtc().AddDays(-15);
                var aggregateData = await _dataLayer.GetAggregateDataMulti(assets.Select(x => x.Symbol), backlogStart, currentDay.GetTradingOpenTimeUtc(), BarTimeFrame.Day);

                List<RangeRecord> ranges = new List<RangeRecord>();
                foreach (var data in aggregateData)
                {
                    var bars = data.Value;

                    if (bars.Count < 11) 
                        continue;

                    var lastBar = bars.GetHistoricValue(0);
                    var penUltimateBar = bars.GetHistoricValue(1);

                    if (!lastBar.IsGreen())
                        continue;

                    var atrAverage = AverageTrueRange.CalculateAverageTrueRange(11, bars.Take(11));
                    var atr = AverageTrueRange.CalculateTrueRange(penUltimateBar, lastBar);

                    if (atrAverage == 0 || atr == 0)
                        continue;

                    var signal = (atr / atrAverage);
                    if (signal > 3)
                    {
                        ranges.Add(new RangeRecord(data.Key, signal.RoundToMoney()));
                    }
                }

                var ordered = ranges.OrderByDescending(x => x.multipleATR);
                foreach (var range in ordered)
                {
                    _logger.LogInformation($"{range.Symbol} {range.multipleATR}");
                }
            }
        }
    }
}
