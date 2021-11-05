using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Denali.Models.Shared
{
    public class AlpacaAggregateData
    {
        [JsonPropertyName("bars")]
        public List<AggregateData> Bars { get; set; }
        [JsonPropertyName("symbol")]
        public string Ticker { get; set; }
        [JsonPropertyName("next_page_token")]
        public string NextPageToken { get; set; }
    }
}
