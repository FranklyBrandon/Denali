using Denali.Algorithms.AggregateAnalysis.EMA;
using Denali.Algorithms.AggregateAnalysis.SMAStrategy;
using Denali.Models.Shared;
using Denali.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Strategies
{
    public class RibbonTrendStrategy : IAggregateStrategy
    {
        private readonly EMA _ema9;
        private readonly EMA _ema21;
        private readonly EMA _ema55;

        private readonly decimal _ribbonThreshold;
        private readonly TimeUtils _timeUtils;

        /// <summary>
        /// use EMAs, 9,21,55 If bar closes above 9ema and 9ema > 21 ema and > 55ema and 21ema - 9ema > 21ema-55 ema
        /// </summary>
        public RibbonTrendStrategy()
        {
            _ema9 = new EMA(9);
            _ema21 = new EMA(21);
            _ema55 = new EMA(55);
            _timeUtils = new TimeUtils();
        }
        public void Initialize(IEnumerable<IAggregateData> aggregateData)
        {
            var currentBarData = new List<IAggregateData>();
            for (int i = 0; i <= aggregateData.Count() - 1; i++)
            {
                currentBarData.Add(aggregateData.ElementAt(i));
                _ema9.Analyze(currentBarData);
                _ema21.Analyze(currentBarData);
                _ema55.Analyze(currentBarData);
            }
        }

        public MarketAction ProcessTick(IEnumerable<IAggregateData> aggregateData, ITradingContext context)
        {
            _ema9.Analyze(aggregateData); 
            _ema21.Analyze(aggregateData);
            _ema55.Analyze(aggregateData);

            var currentEMA9 = _ema9.MovingAverages.LastOrDefault();
            var currentEMA21 = _ema21.MovingAverages.LastOrDefault();
            var currentEMA55 = _ema55.MovingAverages.LastOrDefault();

            Console.WriteLine($"Time: {_timeUtils.GetETDatetimefromUnixS(aggregateData.Last().Time)}");
            Console.WriteLine($"EMA9: {currentEMA9}");
            Console.WriteLine($"EMA21: {currentEMA21}");
            Console.WriteLine($"EMA55: {currentEMA55}");

            if (currentEMA9 is 0m || currentEMA21 is 0m || currentEMA55 is 0m)
                return MarketAction.None;

            var previousSma9 = _ema9.MovingAverages.ElementAtOrDefault(_ema9.MovingAverages.Count - 2);
            var previousSma21 = _ema21.MovingAverages.ElementAtOrDefault(_ema21.MovingAverages.Count - 2);

            if (previousSma9 is 0m || previousSma21 is 0m)
                return MarketAction.None;

            var currentBar = aggregateData.LastOrDefault();
            var previousBar = aggregateData.ElementAtOrDefault(aggregateData.Count() - 2);
            var previousEMA9 = _ema9.MovingAverages.ElementAtOrDefault(_ema9.MovingAverages.Count() - 2);

            if (currentBar is null || previousBar is null || previousEMA9 is 0m)
                return MarketAction.None;

            if (currentEMA9 > currentEMA21 && currentEMA21 > currentEMA55)
            {
                if ((currentEMA9 - currentEMA21) < (currentEMA21 - currentEMA55))
                {
                    if (currentBar.ClosePrice > currentEMA9)
                    {
                        return MarketAction.Buy;
                    }
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
