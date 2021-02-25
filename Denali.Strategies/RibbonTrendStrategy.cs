using Denali.Algorithms.AggregateAnalysis.SMA;
using Denali.Models.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Strategies
{
    public class RibbonTrendStrategy : IAggregateStrategy
    {
        private readonly SMA _sma5;
        private readonly SMA _sma8;
        private readonly SMA _sma13;

        private decimal _stopLossSMA = 0.00M;
        private decimal _beginSMA = 0.00M;
        private decimal _graceThreshold = 0.10M;

        private readonly decimal _ribbonThreshold;

        public RibbonTrendStrategy()
        {
            _sma5 = new SMA(5);
            _sma8 = new SMA(8);
            _sma13 = new SMA(13);
        }
        public void Initialize(IEnumerable<IAggregateData> aggregateData)
        {
            var currentBarData = new List<IAggregateData>();
            for (int i = 0; i <= aggregateData.Count() - 1; i++)
            {
                currentBarData.Add(aggregateData.ElementAt(i));
                _sma5.Analyze(currentBarData);
                _sma8.Analyze(currentBarData);
                _sma13.Analyze(currentBarData);
            }
        }

        public MarketAction ProcessTick(IEnumerable<IAggregateData> aggregateData, ITradingContext context)
        {
            _sma5.Analyze(aggregateData);
            _sma8.Analyze(aggregateData);
            _sma13.Analyze(aggregateData);

            var currentSma5 = _sma5.MovingAverages.Last();
            var currentSma8 = _sma8.MovingAverages.Last();
            var currentSma13 = _sma13.MovingAverages.Last();

            if (currentSma5 > currentSma13 && currentSma8 > currentSma13)
            {
                if (context.LongOpen == false)
                {
                    return MarketAction.Buy;
                }      
            }
            
            if (currentSma5 <= currentSma13 && currentSma8 <= currentSma13)
            {
                if (context.LongOpen)
                {
                    return MarketAction.Sell;
                }
            }

            return MarketAction.None;
        }

        private bool RibbonsOverlapThreshold(decimal sma5, decimal sma8, decimal sma13)
        {
            var diff1 = Math.Abs(sma5 - sma8);
            var diff2 = Math.Abs(sma8 - sma13);
            var diff3 = Math.Abs(sma13 - sma5);

            var test1 = diff1 <= _ribbonThreshold;
            var test2 = diff2 <= _ribbonThreshold;
            var test3 = diff3 <= _ribbonThreshold;
            var test4 = (new[] { diff1, diff2, diff3 }.Max() - new[] { diff1, diff2, diff3 }.Min()) <= _ribbonThreshold;

            return test1 && test2 && test3 && test4;
        }
    }
}
