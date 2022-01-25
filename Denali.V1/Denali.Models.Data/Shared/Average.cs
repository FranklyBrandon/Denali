using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Denali.Models.Shared
{
    public class Average
    {
        [JsonPropertyName("Value")]
        public decimal Value { get; set; }
        [JsonPropertyName("Time")]
        public string Time { get; set; }

        public Average(decimal value, string time)
        {
            this.Value = value;
            this.Time = time;
        }
    }
}
