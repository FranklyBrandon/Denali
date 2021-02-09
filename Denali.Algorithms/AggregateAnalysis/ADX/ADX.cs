using Denali.Algorithms.AggregateAnalysis.TR;
using Denali.Models.Polygon;
using Denali.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Algorithms.AggregateAnalysis.ADX
{
    public class ADX
    {
        private readonly TrueRange _trueRange;

        public IList<ADXResult> InitialADXResults { get; set; }

        public ADX()
        {
            _trueRange = new TrueRange();
        }

        /// <summary>
        /// Initiate the ADX calculation by giving a lookback period of history
        /// </summary>
        /// <param name="history"></param>
        public void Initiate(IEnumerable<IAggregateData> history)
        {
            InitialADXResults = new List<ADXResult>();

            var length = history.Count();
            var backlog = length / 2;

            //Start at index one because there is no previous value to use
            for (int i = 1; i < length; i++)
            {
                ADXResult result;
 
                var previous = history.ElementAtOrDefault(i - 1);
                var current = history.ElementAtOrDefault(i);

                var tr = _trueRange.Analyze(previous, current);
                var dms = CalculateDirectionalMovements(previous, current);

                result = new ADXResult
                {
                    DMPlus = dms.Item1,
                    DMMinus = dms.Item2,
                    TrueRange = tr,
                    Time = current.Time
                };

                if (i <= backlog)
                    continue;

                //Day 15
                if (i == backlog + 1)
                {
                    var trueRangeSum = InitialADXResults.Sum(x => x.TrueRange);
                    var dmpSum = InitialADXResults.Sum(x => x.DMPlus);
                    var dmmSum = InitialADXResults.Sum(x => x.DMMinus);

                    result.SmoothedTrueRange = GetSmoothedValue(trueRangeSum, result.TrueRange, backlog);
                    result.SmoothedDMPlus = GetSmoothedValue(dmpSum, result.DMPlus, backlog);
                    result.SmoothedDMMinus = GetSmoothedValue(dmmSum, result.DMMinus, backlog);
                }

                var previousResult = InitialADXResults.ElementAt(i - 1);
                result.SmoothedTrueRange = GetSmoothedValue(previousResult.SmoothedTrueRange, result.TrueRange, backlog);
                result.SmoothedDMPlus = GetSmoothedValue(previousResult.SmoothedDMPlus, result.DMPlus, backlog);
                result.SmoothedDMMinus = GetSmoothedValue(previousResult.SmoothedDMMinus, result.DMMinus, backlog);
            }
        }

        public void Analyze(IEnumerable<IAggregateData> barData)
        {


            

        }

        public void AnalyzeAll(IEnumerable<IAggregateData> barData, int lookbackPeriod)
        {

        }

        private (decimal, decimal) CalculateDirectionalMovements(IAggregateData previous, IAggregateData current)
        {
            var highDifference = current.HighPrice - previous.HighPrice;
            var lowDifference = previous.LowPrice - current.LowPrice;

            var DMPlus = highDifference > lowDifference ?
                Math.Max(highDifference, 0) : 0;
            var DMMinus = lowDifference > highDifference ?
                Math.Max(lowDifference, 0) : 0;

            return (DMPlus, DMMinus);
        }

        private decimal GetSmoothedValue(decimal previousValue, decimal currentValue, int backlog)
        {
            return previousValue - (previousValue / backlog) + currentValue;
        }
    }
}
