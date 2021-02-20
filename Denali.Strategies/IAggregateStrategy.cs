using Denali.Models.Shared;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Denali.Strategies
{
    public interface IAggregateStrategy
    {
        void Initialize(IEnumerable<IAggregateData> aggregateData);
        MarketAction ProcessTick(IEnumerable<IAggregateData> aggregateData, ITadingContext context);
    }

    public interface ITadingContext
    {
        public bool BuyOpen { get; set; }
        public bool SellOpen { get; set; }

    }

    public enum MarketAction
    {
        Buy,
        Sell,
        None
    }
}
