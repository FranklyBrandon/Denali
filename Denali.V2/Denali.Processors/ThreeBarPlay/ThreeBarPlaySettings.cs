using Denali.TechnicalAnalysis.ElephantBars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Denali.Processors.ThreeBarPlay
{
    public class ThreeBarPlaySettings
    {
        public static string Settings = "ThreeBarPlaySettings";

        [JsonPropertyName("ElephantBarSettings")]
        public ElephantBarSettings ElephantBarSettings { get; set; }
    }
}
