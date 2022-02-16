using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Denali.Models
{
    public class HistoricalBarsResponse
    {
        [JsonPropertyName("bars")]
        public List<Bar> Bars { get; set; }
        [JsonPropertyName("symbol")]
        public String Symbol { get; set; }
    }
}
