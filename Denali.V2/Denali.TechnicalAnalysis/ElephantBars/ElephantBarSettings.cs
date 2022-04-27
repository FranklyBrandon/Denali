using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.TechnicalAnalysis.ElephantBars
{
    public class ElephantBarSettings
    {
        public static string Settings = "ElephantBarSettings";
        public int RangeAveragesBacklog { get; set; }
        public decimal OverAverageThreshold { get; set; }
        public decimal BodyPercentageThreshold { get; set; }
    }
}
