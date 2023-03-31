using Denali.Models;

namespace Denali.TechnicalAnalysis
{
    public class Gap
    {
        public bool FullGap { get; set; }

        public Gap(bool fullGap)
        {
            FullGap = fullGap;
        }

        public bool IsGapUp(IAggregateBar currentBar, IAggregateBar previousBar)
        {
            var previousPrice = FullGap ? previousBar.High : Math.Max(previousBar.Open, previousBar.Close);
            if (currentBar.Open > previousPrice)
                return true;

            return false;
        }

        public bool IsGapDown(IAggregateBar currentBar, IAggregateBar previousBar)
        {
            var previousPrice = FullGap ? previousBar.Low : Math.Min(previousBar.Open, previousBar.Close);
            if (currentBar.Open < previousPrice)
                return true;

            return false;
        }
    }
}
