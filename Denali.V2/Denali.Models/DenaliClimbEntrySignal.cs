using Alpaca.Markets;

namespace Denali.Models
{
    public record DenaliClimbEntrySignal
    {
        public IBar SignalBar { get; set; }
        public decimal EntryPrice { get; set; }
        public decimal GapUpPercentage { get; set; }
        public decimal StopLoss { get; set; }
        public decimal TakeProfit { get; set; }

    }
}
