using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Denali.Models.Polygon
{
    public class Bar
    {
        [JsonPropertyName("o")]
        public double OpenPrice { get; set; }
        [JsonPropertyName("c")]
        public double ClosePrice { get; set; }
        [JsonPropertyName("l")]
        public double LowPrice { get; set; }
        [JsonPropertyName("h")]
        public double HighPrice { get; set; }
        [JsonPropertyName("v")]
        public double Volume { get; set; }
        [JsonPropertyName("vw")]
        public double VolumeWeightedAverage { get; set; }
        [JsonPropertyName("t")]
        public long Time { get; set; }
        [JsonPropertyName("n")]
        public int Number { get; set; }
    }
}
