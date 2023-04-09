using Alpaca.Markets;
using Denali.Models;

namespace Denali.Processors.GapMomentum
{
    public record struct GapMomentumTrade(
        string Symbol, 
        bool Up, 
        AggregateBar PreviousBar, 
        AggregateBar TradeBar, 
        IIntervalCalendar MarketDay
    )
    {
        public IEnumerable<AggregateBar> AggregateMinuteBars { get; set; } = Enumerable.Empty<AggregateBar>();
        public int Shares = 0;
        public decimal profit = 0;
    }
}
