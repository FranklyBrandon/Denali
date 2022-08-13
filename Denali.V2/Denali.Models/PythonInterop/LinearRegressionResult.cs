using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Denali.Models.PythonInterop
{
    public class LinearRegressionResult
    {
        [JsonPropertyName("results")]
        public List<OLSResult> Results { get; set; }
    }

    public class OLSResult
    {
        [JsonPropertyName("spread")]
        public double Spread { get; set; }

        [JsonPropertyName("timeUTC")]
        public DateTime TimeUTC { get; set; }

        [JsonPropertyName("zscore")]
        public double ZScore { get; set; }

        [JsonPropertyName("beta")]
        public double Beta { get; set; }
    }
}
