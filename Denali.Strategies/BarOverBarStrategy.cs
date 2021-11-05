using Denali.Algorithms.AggregateAnalysis.EMA;
using Denali.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Strategies
{
    public class BarOverBarStrategy : IAggregateStrategy
    {
        private IList<IAggregateData> _aggregateData;
        private IList<MarketEvent> _marketEvents;
        private EMA _ema9;

        public void Initialize(IList<IAggregateData> aggregateData)
        {
            _marketEvents = new List<MarketEvent>();
            _ema9 = new EMA(9);
            _aggregateData = aggregateData;
            _ema9.Analyze(_aggregateData);
        }

        public MarketAction ProcessTick(IAggregateData aggregateBar, ITradingContext context)
        {
            _aggregateData.Add(aggregateBar);
            _ema9.Analyze(_aggregateData);

            var currentEma = _ema9.MovingAverages.Last();

            // If the stock is in an uptrend
            if (aggregateBar.HighPrice > currentEma)
            {
                var currentBar = _aggregateData.Last();
                var previousBar = _aggregateData.ElementAtOrDefault(_aggregateData.Count - 2);

                if (previousBar is null)
                    return MarketAction.None;

                if (previousBar.HighPrice > _aggregateData.Last().HighPrice)
                {
                    _marketEvents.Add(new MarketEvent
                    {
                        Event = "BarOverBar",
                        Date = currentBar.Time
                    });
                }
            }

            return MarketAction.None;
        }
    }
}
