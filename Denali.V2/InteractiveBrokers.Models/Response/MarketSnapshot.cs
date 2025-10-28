using System.Text.Json.Serialization;

namespace InteractiveBrokers.Models.Response
{
    public class MarketSnapshot
    {
        [JsonPropertyName("conid")]
        public int Conid { get; set; }

        [JsonPropertyName("7644")]
        public string ShortableStatus { get; set; }

        [JsonPropertyName("7636")]
        public string ShortableShares { get; set; }
    }
}
