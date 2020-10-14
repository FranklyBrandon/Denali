using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace Denali.Models.Polygon
{
    public enum Channel
    {
        [EnumMember(Value="T")]
        Trades,
        [EnumMember(Value = "Q")]
        Quotes,
        [EnumMember(Value = "A")]
        AggregateSecond,
        [EnumMember(Value = "AM")]
        AggregateMinute,
        [EnumMember(Value = "LULD")]
        LimitUpLimitDown,
        [EnumMember(Value = "NOI")]
        Imbalances,
    }

    public enum Action
    {
        [EnumMember(Value = "auth")]
        Authorize,
        [EnumMember(Value = "subscribe")]
        Subscribe,
        [EnumMember(Value = "unsubscribe")]
        Unsubscribe
    }

    public class WebsocketRequest
    {
        [JsonPropertyName("action")]
        [JsonConverter(typeof(JsonExtendedEnumStringConverter))]
        public Action Action { get; set; }
        [JsonPropertyName("params")]
        public string Params { get; set; }
    }
}
