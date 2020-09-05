using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Denali.Models.Data.FinnHub
{
    public class CandleResponse
    {
        [JsonPropertyName("o")]
        public IEnumerable<double> OpenPrice { get; set; }
        [JsonPropertyName("c")]
        public IEnumerable<double> ClosePrice { get; set; }
        [JsonPropertyName("l")]
        public IEnumerable<double> LowPrice { get; set; }
        [JsonPropertyName("h")]
        public IEnumerable<double> HighPrice { get; set; }
        [JsonPropertyName("v")]
        public IEnumerable<int> Volume { get; set; }
        [JsonPropertyName("t")]
        public IEnumerable<int> Timestamp { get; set; }
        [JsonPropertyName("s")]
        public string Status { get; set; }

    }


}
