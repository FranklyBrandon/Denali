namespace Denali.Processors.TrueFadeStrategy
{
    public static class TrueFadeAllocater
    {
        public static IEnumerable<TrueFadePosition> Allocate(IEnumerable<TrueFadeSignal> signals, decimal capitalToTrade, decimal maximumVolumePercentage)
        {
            IEnumerable<TrueFadePosition> positions = signals.Select(x => new TrueFadePosition(x)).ToList();
            bool allocate = true;
            while (allocate)
            {
                bool allocatedThisRound = false;
                foreach (var position in positions)
                {
                    if ((position.PositionSize + 1) / position.Signal.AverageVolume > maximumVolumePercentage / 100)
                        continue;

                    if (capitalToTrade > position.Signal.EstimatedPrice)
                    {
                        position.PositionSize++;
                        capitalToTrade -= position.Signal.EstimatedPrice;
                        allocatedThisRound = true;
                    }
                    else
                    {
                        allocate = false;
                        break;
                    }
                }

                // If no allocatations, everything is volume capped
                if (!allocatedThisRound)
                    allocate = false;
            }

            return positions;
        }
    }
}
