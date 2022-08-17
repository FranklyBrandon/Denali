using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Denali.Services.YahooFinanceService
{
    public class QuotesResponse
    {
        [JsonPropertyName("chart")]
        public Chart Chart { get; set; }
    }

    public class Chart
    {
        [JsonPropertyName("result")]
        public List<Result> Results { get; set; }
    }

    public class Result
    {
        [JsonPropertyName("meta")]
        public Meta Meta { get; set; }


        [JsonPropertyName("indicators")]
        public Indicator Indicators { get; set; }

        [JsonPropertyName("timestamp")]
        public List<long> TimeStamps { get; set; }
    }

    public class Meta
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }
    }
    public class Indicator
    {
        [JsonPropertyName("quote")]
        public List<Quote> Quotes { get; set; }
    }

    public class Quote
    {
        [JsonPropertyName("close")]
        public List<decimal?> Close { get; set; }

        [JsonPropertyName("high")]
        public List<decimal?> High { get; set; }

        [JsonPropertyName("open")]
        public List<decimal?> Open { get; set; }

        [JsonPropertyName("low")]
        public List<decimal?> Low { get; set; }

        [JsonPropertyName("volume")]
        public List<int?> Volume { get; set; }
    }
}
