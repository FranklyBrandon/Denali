using Denali.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.TechnicalAnalysis.StatisticalArbitrage
{
    public record PairSpread(double varienceMean, double standardDeviation, double zScore);

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
        }

        public void Analyze(IEnumerable<AggregateBar> tickerAData, IEnumerable<AggregateBar> tickerBData)
        {
            var length = tickerAData.Count() - 1;
            if (length < _backlog - 1)
                return;

            // Skip the first bar
            int start = length - _backlog + 1;

            List<double> spreadValues = new List<double>();

            for (int i = start; i < length; i++)
            {
                var originalA = tickerAData.ElementAt(i - 1);
                var originalB = tickerBData.ElementAt(i - 1);
                var newA = tickerAData.ElementAt(i);
                var newB = tickerBData.ElementAt(i);

                var percentageChangeA = PercentageDifference(originalA.Close, newA.Close);
                var percentageChangeB = PercentageDifference(originalB.Close, newB.Close);

                var spread = percentageChangeA - percentageChangeB;
                spreadValues.Add(spread);

                _spreadAverage.Analyze(spread);

                if (spreadValues.Count < _backlog)
                    continue;

                var mean = _spreadAverage.MovingAverages.Last();
                var std = _std.CalculateStandardDeviation(spreadValues, mean, 100);
                var zScore = (spread - mean) / std;

                PairSpreads.Add(new PairSpread(mean, std, zScore));
            }
        }

        private double PercentageDifference(decimal originalValue, decimal newValue) =>
            (double)(Math.Abs(originalValue - newValue) / originalValue) * 100;

    }
}
