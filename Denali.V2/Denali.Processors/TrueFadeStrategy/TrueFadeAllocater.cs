using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors.TrueFadeStrategy
{
    public static class TrueFadeAllocater
    {
        public static IEnumerable<TrueFadeRecord> Allocate(IEnumerable<TrueFadeRecord> records, decimal capitalToTrade)
        {
            bool allocate = true;
            while (allocate)
            {
                foreach (var record in records)
                {
                    if (capitalToTrade > record.Price)
                    {
                        record.PositionSize++;
                        capitalToTrade -= record.Price;
                    }
                    else
                    {
                        allocate = false;
                        break;
                    }
                }
            }

            return records;
        }
    }
}
