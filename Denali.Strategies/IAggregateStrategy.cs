using Denali.Models.Shared;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Denali.Strategies
{
    public interface IAggregateStrategy
    {
        void Initialize(IList<IAggregateData> aggregateData);
        MarketAction ProcessTick(IAggregateData aggregateData, ITradingContext context);
    }

    public interface ITradingContext
    {
        public bool LongOpen { get; set; }
        public decimal EntryPrice { get; set; }
        public bool BuyOpen { get; set; }
        public bool SellOpen { get; set; }
        public Transaction Transaction { get; set; }

    }

    public class TradingContext : ITradingContext
    {
        public bool BuyOpen { get; set; }
        public decimal EntryPrice { get; set; }
        public bool SellOpen { get; set; }
        public bool LongOpen { get; set; }
        public Transaction Transaction { get; set; }
    }

    public enum MarketAction
    {
        Buy,
        Sell,
        None
    }

    public class MarketEvent
    {
        public string Event { get; set; }
        public string Date { get; set; }
    }
}
