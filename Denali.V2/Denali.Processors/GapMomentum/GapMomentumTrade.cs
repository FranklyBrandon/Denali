using Alpaca.Markets;
using Denali.Models;

namespace Denali.Processors.GapMomentum
{
    public record struct GapMomentumTrade(
        MarketAction action,
        int direction = 0
    )
    {
        public IEnumerable<AggregateBar> AggregateMinuteBars { get; set; } = Enumerable.Empty<AggregateBar>();
        public int Shares = 0;
        public decimal Profit = 0;
        public decimal EntryPrice = 0;
        public bool HighWaterMet = false;
    }
}
