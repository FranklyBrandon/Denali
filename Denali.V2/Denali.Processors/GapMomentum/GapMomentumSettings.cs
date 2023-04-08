using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors.GapMomentum
{
    public class GapMomentumSettings
    {
        public bool FullGap { get; set; } = false;
        public Dictionary<decimal, decimal> StopProfitMap { get; set; } = new Dictionary<decimal, decimal>
        {
            { 0.30m, 0.15m },
            { 0.60m, 0.30m },
            { 1.00m, 0.60m },
            { 1.30m, 1.00m }
        };
    }
}
