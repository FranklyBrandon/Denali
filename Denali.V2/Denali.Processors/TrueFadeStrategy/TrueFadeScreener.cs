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

namespace Denali.Processors.TrueFadeStrategy
{
    public record TrueFadeRecord(string Symbol, decimal Price, decimal MultipleATR, decimal AverageTrueRange)
    {
        public int PositionSize { get; set; }
    }

    public class TrueFadeScreener
    {
        private readonly DataLayerComponent _dataLayer;

        public TrueFadeScreener(DataLayerComponent dataLayer)
        {
            _dataLayer = dataLayer;
        }

        public async Task<IEnumerable<TrueFadeRecord>> ScreenTrueFade(IEnumerable<IAsset> assets, DateTime currentDate, List<IIntervalCalendar> marketDaysLookback, int minimumATRMultiple)
        {
            var aggregateData = await _dataLayer.GetAggregateDataMulti(
                assets.Select(x => x.Symbol),
                marketDaysLookback.First().GetTradingOpenTimeUtc().AddDays(-1),
                marketDaysLookback.Last().GetTradingOpenTimeUtc(),
                BarTimeFrame.Day);

            List<TrueFadeRecord> fadeRecords = new List<TrueFadeRecord>();
            foreach (var data in aggregateData)
            {
                var bars = data.Value;

                if (bars.Count < marketDaysLookback.Count)
                    continue;

                var lastBar = bars.GetHistoricValue(0);
                var penUltimateBar = bars.GetHistoricValue(1);

                if (!lastBar.IsGreen() || lastBar.Open <= penUltimateBar.High || lastBar.Close >= 500m)
                    continue;

                var averageTrueRange = AverageTrueRange.CalculateAverageTrueRange(10, bars.Take(10));
                var trueRange = AverageTrueRange.CalculateTrueRange(penUltimateBar, lastBar);

                if (averageTrueRange == 0 || trueRange == 0)
                    continue;

                var multiple = trueRange / averageTrueRange;
                if (multiple > minimumATRMultiple)
                {
                    fadeRecords.Add(new TrueFadeRecord(data.Key, lastBar.Close, multiple.RoundToMoney(), averageTrueRange.RoundToMoney()));
                }
            }

            var rangeAssets = fadeRecords.Join(assets, x => x.Symbol, y => y.Symbol, (x, y) => new { x.Symbol, x.Price, x.MultipleATR, y.Shortable, x.AverageTrueRange });
            var ordered = rangeAssets.Where(x => x.Shortable).OrderByDescending(x => x.MultipleATR);

            return ordered.Select(x => new TrueFadeRecord(x.Symbol, x.Price, x.MultipleATR, x.AverageTrueRange)).ToList();
        }
    }
}
