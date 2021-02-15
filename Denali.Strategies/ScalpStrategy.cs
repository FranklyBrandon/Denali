using Denali.Algorithms.AggregateAnalysis;
using Denali.Algorithms.AggregateAnalysis.ParabolicSAR;
using Denali.Models.Shared;
using Denali.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Strategies
{
    public class ScalpStrategy : IAggregateStrategy
    {
        private readonly SMA _sma;
        private readonly TimeUtils _timeUtils;

        public ScalpStrategy()
        {
            _sma = new SMA(9);
            _timeUtils = new TimeUtils();
        }

        public void Initialize(IEnumerable<IAggregateData> aggregateData)
        {
            var currentBarData = new List<IAggregateData>();
            for (int i = 0; i <= aggregateData.Count() - 1; i++)
            {
                currentBarData.Add(aggregateData.ElementAt(i));
                _sma.Analyze(currentBarData);
            }
        }

        public void ProcessTick(IEnumerable<IAggregateData> aggregateData)
        {
            _sma.Analyze(aggregateData);

            if (_sma.MovingAverages.Count() < 1)
                return;

            var latestData = aggregateData.Last();
            var latestSMA = _sma.MovingAverages.Last();

            if (latestData.OpenPrice < latestSMA && latestData.ClosePrice > latestSMA)
            {
                var time = _timeUtils.GetETDatetimefromUnixMS(latestData.Time);
                Console.WriteLine($"CROSS OVER at ${time.ToString()}");
            }
        }
    }
}
