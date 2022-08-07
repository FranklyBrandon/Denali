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
        [JsonPropertyName("beta")]
        public double Beta { get; set; }

        [JsonPropertyName("spreads")]
        public List<double> Spreads { get; set; }

        [JsonPropertyName("zscores")]
        public List<double> ZScores { get; set; }
    }
}
