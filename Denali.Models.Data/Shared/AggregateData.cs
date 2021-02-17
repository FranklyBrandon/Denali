using Denali.Models.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Denali.Models.Shared
{
    public class AggregateData : IAggregateData
    {
        [JsonPropertyName("o")]
        public decimal OpenPrice { get; set; }
        [JsonPropertyName("c")]
        public decimal ClosePrice { get; set; }
        [JsonPropertyName("l")]
        public decimal LowPrice { get; set; }
        [JsonPropertyName("h")]
        public decimal HighPrice { get; set; }
        [JsonPropertyName("v")]
        public decimal Volume { get; set; }
        [JsonPropertyName("vw")]
        public decimal VolumeWeightedAverage { get; set; }
        [JsonPropertyName("t")]
        public long Time { get; set; }
        [JsonPropertyName("n")]
        public int Number { get; set; }
    }
}
