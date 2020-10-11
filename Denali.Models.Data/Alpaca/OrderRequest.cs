using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Denali.Models.Alpaca
{
    public class OrderRequest
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }
        [JsonPropertyName("qty")]
        public int Quantity { get; set; }
        [JsonPropertyName("side")]
        public OrderSide Side { get; set; }
        [JsonPropertyName("type")]
        public OrderType Type { get; set; }
        [JsonPropertyName("time_in_force")]
        public TimeInForce TimeInForce { get; set; }
        [JsonPropertyName("limit_price")]
        public double LimitPrice { get; set; }
        [JsonPropertyName("stop_price")]
        public double StopPrice { get; set; }
        [JsonPropertyName("trail_price")]
        public double TrailPrice { get; set; }
        [JsonPropertyName("trail_precent")]
        public double TrailPrecent { get; set; }
        [JsonPropertyName("extended_hours")]
        public bool ExtendedHours { get; set; }
        [JsonPropertyName("client_order_id")]
        public string ClientOrderId { get; set; }
        [JsonPropertyName("order_class")]
        public OrderClass OrderClass { get; set; }
        [JsonPropertyName("take_profit")]
        public TakeProfitModel TakeProfit { get; set; }
        [JsonPropertyName("stop_loss")]
        public StopLossModel StopLoss { get; set; }
    }

    public enum OrderType
    {
        [EnumMember(Value = "market")]
        Market,
        [EnumMember(Value = "limit")]
        Limit,
        [EnumMember(Value = "stop_limit")]
        StopLimit,
        [EnumMember(Value = "trailing_stop")]
        TrailingStop
    }

    public enum OrderSide
    {
        [EnumMember(Value = "buy")]
        Buy,
        [EnumMember(Value = "sell")]
        Sell
    }

    public enum TimeInForce
    {
        [EnumMember(Value = "day")]
        Day,
        [EnumMember(Value = "gtc")]
        GoodUntilCanceled,
        [EnumMember(Value = "opg")]
        OPG,
        [EnumMember(Value = "cls")]
        CLS,
        [EnumMember(Value = "ioc")]
        ImmediateOrCanceled,
        [EnumMember(Value = "fok")]
        FillOrKill
    }

    public enum OrderClass
    {
        [EnumMember(Value = "simple")]
        Simple,
        [EnumMember(Value = "bracket")]
        Bracket,
        [EnumMember(Value = "oco")]
        OCO,
        [EnumMember(Value = "oto")]
        OTO
    }

    public class TakeProfitModel
    {
        [JsonPropertyName("limit_price")]
        public double LimitPrice { get; set; }
    }

    public class StopLossModel
    {
        [JsonPropertyName("stop_loss")]
        public double StopLoss { get; set; }
    }
}
