using Denali.Algorithms.AggregateAnalysis.EMA;
using Denali.Models.Shared;
using Denali.Shared.Utility;
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
        public IList<MarketEvent> MarketEvents;
        public IList<Average> EMAs;
        private EMA _ema9;

        public void Initialize(IList<IAggregateData> aggregateData)
        {
            MarketEvents = new List<MarketEvent>();
            EMAs = new List<Average>();
            _ema9 = new EMA(9);
            _aggregateData = aggregateData;
            _ema9.Analyze(_aggregateData);
        }

        public async Task<MarketAction> ProcessTick(IAggregateData aggregateBar, ITradingContext context)
        {
            _aggregateData.Add(aggregateBar);
            _ema9.Analyze(_aggregateData);

            var currentEma = _ema9.MovingAverages.Last();
            EMAs.Add(new Average(currentEma, aggregateBar.Time));

            // If the stock is in an uptrend
            if (aggregateBar.HighPrice > currentEma)
            {
                var currentBar = _aggregateData.Last();
                var previousBar = _aggregateData.ElementAtOrDefault(_aggregateData.Count - 2);

                if (previousBar is null)
                    return MarketAction.None;

                if (previousBar.HighPrice > _aggregateData.Last().HighPrice)
                {
                    MarketEvents.Add(new MarketEvent
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
