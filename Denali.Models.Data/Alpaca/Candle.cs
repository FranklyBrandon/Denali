using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Denali.Models.Alpaca
{
    public class Candle
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
        public int Volume { get; set; }
        [JsonPropertyName("t")]
        public int Timestamp { get; set; }

        public bool IsOpen
        {
            get
            {
                if (OpenPrice < ClosePrice)
                    return true;
                else
                    return false;
            }
        }

        public bool IsClosed
        {
            get
            {
                if (ClosePrice < OpenPrice)
                    return true;
                else
                    return false;
            }
        }
    }
}
