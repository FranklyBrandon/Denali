using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors.TrueFadeStrategy
{
    public class TrueFadeStrategySettings
    {
        public const string Settings = "TrueFadeStrategySettings";
        public int LookBackMarketDays { get; set; }
        public decimal CapitalToTrade { get; set; }
        public int MinimumAverageTrueRangeMultiple { get; set; }
        public int MaxAssetCount { get; set; }
        public int MaximumVolumePercentage { get; set; }
    }
}
