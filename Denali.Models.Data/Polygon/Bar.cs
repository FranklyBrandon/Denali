using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Denali.Models.Polygon
{
    public class Bar
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

        public bool IsOpen
        {
            get
            {
                if (OpenPrice < ClosePrice)
                    return true;
                return false;
            }
        }

        public bool IsClosed
        {
            get
            {
                if (ClosePrice < OpenPrice)
                    return true;
                return false;
            }
        }
    }
}
