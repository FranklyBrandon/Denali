using Denali.Algorithms.AggregateAnalysis.SMA;
using Denali.Models.Shared;
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

        public MarketAction ProcessTick(IEnumerable<IAggregateData> aggregateData, ITadingContext context)
        {
            var previousSma13 = _sma13.MovingAverages.Last();

            _sma5.Analyze(aggregateData);
            _sma8.Analyze(aggregateData);
            _sma13.Analyze(aggregateData);

            var currentSma13 = _sma13.MovingAverages.Last();

            if (context.BuyOpen)
            {
                // If the SMA13 is below the stop loss, sell to mitigate risk.
                if (currentSma13 < _stopLossSMA)
                {
                    return MarketAction.Sell;
                }
                else if (currentSma13 < previousSma13 && currentSma13 > _beginSMA + _graceThreshold)
                {
                    return MarketAction.Sell;
                }

                return MarketAction.None;
            }
            else if (context.SellOpen)
            {
                return MarketAction.None;
            }
            else if (!context.BuyOpen && !context.SellOpen)
            {
                if (RibbonsOverlapThreshold(_sma5.MovingAverages.Last(), _sma8.MovingAverages.Last(), currentSma13))
                {
                    //Uptrend signaled
                    if (previousSma13 <= currentSma13)
                    {
                        _beginSMA = currentSma13;
                        return MarketAction.Buy;
                    }
                }
            }

            return MarketAction.None;
        }

        private bool RibbonsOverlapThreshold(decimal sma5, decimal sma8, decimal sma13)
        {
            var test1 = Math.Abs(sma5 - sma8) <= _ribbonThreshold;
            var test2 = Math.Abs(sma8 - sma13) <= _ribbonThreshold;
            var test3 = Math.Abs(sma13 - sma5) <= _ribbonThreshold;

            return test1 && test2 && test3;
        }
    }
}
