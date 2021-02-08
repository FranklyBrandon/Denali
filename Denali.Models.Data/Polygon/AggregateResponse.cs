using Denali.Models.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Denali.Models.Polygon
{
    public class AggregateResponse
    {
        [JsonPropertyName("ticker")]
        public string Ticker { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("queryCount")]
        public int QueryCount { get; set; }
        [JsonPropertyName("resultsCount")]
        public int ResultsCount { get; set; }
        [JsonPropertyName("adjusted")]
        public bool Adjusted { get; set; }
        [JsonPropertyName("results")]
        public IEnumerable<Bar> Bars { get; set; }
        [JsonPropertyName("request_id")]
        public string RequestId { get; set; }
    }
}
