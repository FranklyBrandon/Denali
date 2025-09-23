using Alpaca.Markets;

namespace Denali.Models
{
    public record DenaliClimbEntrySignal
    {
        public decimal StopLoss { get; set; }
        public decimal TakeProfit { get; set; }

        public bool FirstPullback { get; set; }
        public DateTime FirstPullbackTime { get; set; }

        public decimal OpeningRangeHigh { get; set; }
        public bool OpeningRangeBreakout { get; set; } = false;
        public DateTime OpeningRangeBreakoutTime { get; set; }

        public bool ConfirmationPullback { get; set; } = false;
        public DateTime ConfirmationPullbackTime { get; set; }

        public bool Signal { get; set; } = false;
        public IBar SignalBar { get; set; }
    }
}
