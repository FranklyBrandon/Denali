using Alpaca.Markets;
using Denali.Services;
using Denali.Services.Extensions;
using Denali.Shared.Extensions;
using Denali.TechnicalAnalysis;
using InteractiveBrokers.Models.Response;
using System.Collections.Concurrent;

namespace Denali.Processors.TrueFadeStrategy
{
    public record TrueFadeSignal(string Symbol, decimal EstimatedPrice, decimal MultipleATR, decimal AverageTrueRange, decimal AverageVolume)
    {
        public Contract IBContract { get; set; }
    }
     
    public record TrueFadePosition(TrueFadeSignal Signal)
    {
        public int PositionSize { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Commision {  get; set; }
        public decimal PerStockProfit { get; set; }
        public decimal TotalProfit { get; set; } // does not include commision
        public decimal GrossProfit { get; set; } // does include commision
    }

    public class TrueFadeScreener
    {
        private readonly DataLayerComponent _dataLayer;

        public TrueFadeScreener(DataLayerComponent dataLayer)
        {
            _dataLayer = dataLayer;
        }

        public async Task<IEnumerable<TrueFadeSignal>> ScreenTrueFade(
            IEnumerable<IAsset> assets, 
            DateTime currentDate, 
            List<IIntervalCalendar> marketDaysLookback, 
            int minimumATRMultiple,
            int takeCount)
        {
            var aggregateData = await _dataLayer.GetAggregateDataMulti(
                assets.Select(x => x.Symbol),
                marketDaysLookback.First().GetTradingOpenTimeUtc().AddDays(-1),
                marketDaysLookback.Last().GetTradingOpenTimeUtc(),
                BarTimeFrame.Day);

            List<TrueFadeSignal> fadeRecords = new List<TrueFadeSignal>();
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
                var averageVolume = bars.Average(x => x.Volume).RoundToMoney();

                if (averageTrueRange == 0 || trueRange == 0 || lastBar.Volume == 0)
                    continue;

                var multiple = trueRange / averageTrueRange;
                if (multiple > minimumATRMultiple)
                {
                    fadeRecords.Add(new TrueFadeSignal(data.Key, lastBar.Close, multiple.RoundToMoney(), averageTrueRange.RoundToMoney(), averageVolume));
                }
            }

            var rangeAssets = fadeRecords.Join(assets, x => x.Symbol, y => y.Symbol, (x, y) => new { x.Symbol, x.EstimatedPrice, x.MultipleATR, y.Shortable, x.AverageTrueRange, x.AverageVolume });
            var ordered = rangeAssets.Where(x => x.Shortable).OrderByDescending(x => x.MultipleATR).Take(takeCount);

            return ordered.Select(x => new TrueFadeSignal(x.Symbol, x.EstimatedPrice, x.MultipleATR, x.AverageTrueRange, x.AverageVolume)).ToList();
        }

        public async Task<IEnumerable<TrueFadeSignal>> ScreenTrueFadeIB(
            IEnumerable<string> symbols,
            DateTime currentDate,
            List<IIntervalCalendar> marketDaysLookback,
            int minimumATRMultiple,
            int minimumAverageVolume,
            int takeCount)
        {
            var aggregateData = await _dataLayer.GetAggregateDataMulti(
                symbols,
                marketDaysLookback.First().GetTradingOpenTimeUtc().AddDays(-1),
                marketDaysLookback.Last().GetTradingOpenTimeUtc(),
                BarTimeFrame.Day);

            List<TrueFadeSignal> fadeRecords = new List<TrueFadeSignal>();
            foreach (var data in aggregateData)
            {
                var bars = data.Value;

                if (bars.Count < marketDaysLookback.Count)
                    continue;

                var lastBar = bars.GetHistoricValue(0);
                var penUltimateBar = bars.GetHistoricValue(1);

                if (!lastBar.IsGreen() || lastBar.Open <= penUltimateBar.High || lastBar.Close >= 500m)
                    continue;

                if (lastBar.High - lastBar.Close < lastBar.Close - lastBar.Open)
                    continue;

                var averageTrueRange = AverageTrueRange.CalculateAverageTrueRange(marketDaysLookback.Count, bars.Take(marketDaysLookback.Count));
                var trueRange = AverageTrueRange.CalculateTrueRange(penUltimateBar, lastBar);
                var averageVolume = bars.Average(x => x.Volume).RoundToMoney();

                if (averageTrueRange == 0 || trueRange == 0 || lastBar.Volume == 0 || averageVolume < minimumAverageVolume)
                    continue;

                var multiple = trueRange / averageTrueRange;
                if (multiple > minimumATRMultiple)
                {
                    fadeRecords.Add(new TrueFadeSignal(data.Key, lastBar.Close, multiple.RoundToMoney(), averageTrueRange.RoundToMoney(), averageVolume));
                }
            }

            var ordered = fadeRecords.OrderByDescending(x => x.MultipleATR).Take(takeCount);

            return ordered.Select(x => new TrueFadeSignal(x.Symbol, x.EstimatedPrice, x.MultipleATR, x.AverageTrueRange, x.AverageVolume)).ToList();
        }
    }
}
