using Denali.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.TechnicalAnalysis.StatisticalArbitrage
{
    public record PairSpread(double varienceMean, double standardDeviation, double zScore, double spread, DateTime timeUTC);

    public class PairSpreadCalculation
    {
        public IList<PairSpread> PairSpreads { get; }

        private readonly int _backlog;
        private readonly SimpleMovingAverageDouble _spreadAverage;
        private readonly StandardDeviation _std;

        public PairSpreadCalculation(int backlog)
        {
            _backlog = backlog;
            _spreadAverage = new SimpleMovingAverageDouble(backlog);
            _std = new StandardDeviation();
            PairSpreads = new List<PairSpread>();
        }

        public void Initialize(IEnumerable<AggregateBar> tickerAData, IEnumerable<AggregateBar> tickerBData)
        {
            var length = tickerAData.Count() - 1;
            if (length < _backlog - 1)
                return;

            // Skip the first bar, because spread is calculated using the previous bar
            int start = 1;

            for (int i = start; i < length; i++)
            {
                var originalA = tickerAData.ElementAt(i - 1);
                var currentA = tickerAData.ElementAt(i);
                var originalB = tickerBData.ElementAt(i - 1);
                var currentB = tickerBData.ElementAt(i);

                // TODO: In this scenario we should get historic quote price
                if (!originalA.TimeUtc.Equals(originalB.TimeUtc))
                {

                }

                var spread = CaclulateSpread(originalA, currentA, originalB, currentB);
                _spreadAverage.Analyze(spread);

                if (_spreadAverage.RawValues.Count < _backlog)
                    continue;

                var zScore = CalculateZScore(originalA.TimeUtc);
                PairSpreads.Add(zScore);
            }
        }

        public void AnalyzeStep(AggregateBar originalA, AggregateBar currentA, AggregateBar originalB, AggregateBar currentB)
        {
            var spread = CaclulateSpread(originalA, currentA, originalB, currentB);
            _spreadAverage.Analyze(spread);

            var zScore = CalculateZScore(originalA.TimeUtc);
            PairSpreads.Add(zScore);
        }

        private double CaclulateSpread(AggregateBar originalA, AggregateBar currentA, AggregateBar originalB, AggregateBar currentB)
        {
            var percentageChangeA = PercentageDifference(originalA.Close, currentA.Close);
            var percentageChangeB = PercentageDifference(originalB.Close, currentB.Close);

            return percentageChangeA - percentageChangeB;
            //return (double)(currentA.Close - currentB.Close);
        }

        private PairSpread CalculateZScore(DateTime timeUTC)
        {
            var mean = _spreadAverage.MovingAverages.Last();
            var std = _std.CalculateStandardDeviation(_spreadAverage.RawValues, mean, _backlog);
            var zScore = (_spreadAverage.RawValues.Last() - mean) / std;

            return new PairSpread(mean, std, zScore, _spreadAverage.RawValues.Last(), timeUTC);
        }

        // TODO: Why is Math.Abs here? Bug?
        private double PercentageDifference(decimal originalValue, decimal newValue) =>
            (double)((originalValue - newValue) / originalValue) * 100;

    }
}
