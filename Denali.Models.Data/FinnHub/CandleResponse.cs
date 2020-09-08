using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Denali.Models.Data.FinnHub
{
    public class CandleResponse
    {
        [JsonPropertyName("o")]
        public IList<double> OpenPrice { get; set; }
        [JsonPropertyName("c")]
        public IList<double> ClosePrice { get; set; }
        [JsonPropertyName("l")]
        public IList<double> LowPrice { get; set; }
        [JsonPropertyName("h")]
        public IList<double> HighPrice { get; set; }
        [JsonPropertyName("v")]
        public IList<int> Volume { get; set; }
        [JsonPropertyName("t")]
        public IList<int> Timestamp { get; set; }
        [JsonPropertyName("s")]
        public string Status { get; set; }

    }


}
