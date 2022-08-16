using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Denali.Services.AlphaAdvantage
{
    public class QuotesResponse
    {
        [JsonPropertyName("Global Quote")]
        public Quote Quote { get; set; }
    }

    public class Quote
    {
        [JsonPropertyName("01. symbol")]
        public string Symbol { get; set; }


        [JsonPropertyName("05. price")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal Price { get; set; }
    }
}
