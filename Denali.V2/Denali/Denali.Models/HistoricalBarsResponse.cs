using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Denali.Models
{
    internal class HistoricalBarsResponse
    {
        [JsonPropertyName("bars")]
        internal List<Bar> Bars { get; set; }
        [JsonPropertyName("symbol")]
        internal String Symbol { get; set; }
    }
}
