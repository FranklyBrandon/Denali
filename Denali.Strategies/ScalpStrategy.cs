using Denali.Algorithms.AggregateAnalysis;
using Denali.Algorithms.AggregateAnalysis.ParabolicSAR;
using Denali.Algorithms.AggregateAnalysis.SMA;
using Denali.Models.Shared;
using Denali.Shared.Utility;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ScalpStrategy> _logger;

        public ScalpStrategy(ILogger<ScalpStrategy> logger)
        {
            _sma = new SMA(9);
            _timeUtils = new TimeUtils();
            _logger = logger;
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

        public bool ProcessTick(IEnumerable<IAggregateData> aggregateData)
        {
            _sma.Analyze(aggregateData);

            if (_sma.MovingAverages.Count() < 1)
                return false;

            var latestData = aggregateData.Last();
            var latestSMA = _sma.MovingAverages.Last();

            if (latestData.OpenPrice < latestSMA && latestData.ClosePrice > latestSMA)
            {
                var time = _timeUtils.GetETDatetimefromUnixMS(latestData.Time);
                _logger.LogInformation($"===SMA cross over at===");
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
