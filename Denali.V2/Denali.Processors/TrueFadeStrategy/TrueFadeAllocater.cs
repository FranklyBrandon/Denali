namespace Denali.Processors.TrueFadeStrategy
{
    public static class TrueFadeAllocater
    {
        public static IEnumerable<TrueFadeRecord> Allocate(IEnumerable<TrueFadeRecord> records, decimal capitalToTrade, decimal maximumVolumePercentage)
        {
            bool allocate = true;
            while (allocate)
            {
                bool allocatedThisRound = false;
                foreach (var record in records)
                {
                    if ((record.PositionSize + 1) / record.AverageVolume > maximumVolumePercentage / 100)
                        continue;

                    if (capitalToTrade > record.Price)
                    {
                        record.PositionSize++;
                        record.TotalCost = record.Price * record.PositionSize;
                        capitalToTrade -= record.Price;
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

            return records;
        }
    }
}
