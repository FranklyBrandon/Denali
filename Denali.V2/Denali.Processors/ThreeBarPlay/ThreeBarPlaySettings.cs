using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors.ThreeBarPlay
{
    public class ThreeBarPlaySettings
    {
        public static string Settings = "ThreeBarPlaySettings";
        public int AveragesBacklog { get; set; }
        public ThreeBarPlayDirectionSettings LongSettings { get; set; }
        public ThreeBarPlayDirectionSettings ShortSettings { get; set; }
    }

    public class ThreeBarPlayDirectionSettings
    {
        public decimal IgnitingBarPercentageThreshold { get; set; }
        public decimal ConsolidationRangePercentageThreshold { get; set; }
    }
}
