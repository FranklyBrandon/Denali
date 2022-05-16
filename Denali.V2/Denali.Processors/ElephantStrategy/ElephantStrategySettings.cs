using Denali.TechnicalAnalysis.ElephantBars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Denali.Processors.ElephantStrategy
{
    public class ElephantStrategySettings
    {
        public static string Settings = "ThreeBarPlaySettings";

        [JsonPropertyName("ElephantBarSettings")]
        public ElephantBarSettings ElephantBarSettings { get; set; }

        public int MaxRetracementPeriodLength { get; set; }
    }
}
