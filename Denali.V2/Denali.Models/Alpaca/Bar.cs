﻿using Alpaca.Markets;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Denali.Models.Alpaca
{
    public class Bar : IBar
    {
        [JsonIgnore]
        public String Symbol { get; private set; } = String.Empty;

        [JsonPropertyName("o")]
        public Decimal Open { get; set; }

        [JsonPropertyName("c")]
        public Decimal Close { get; set; }

        [JsonPropertyName("l")]
        public Decimal Low { get; set; }

        [JsonPropertyName("h")]
        public Decimal High { get; set; }

        [JsonPropertyName("v")]
        public Decimal Volume { get; set; }

        [JsonPropertyName("t")]
        public DateTime TimeUtc { get; set; }

        [JsonPropertyName("vw")]
        public Decimal Vwap { get; set; }

        [JsonPropertyName("n")]
        public UInt64 TradeCount { get; set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSymbol(String symbol) => Symbol = symbol;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public decimal BodyRange() => Math.Abs(Open - Close);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public decimal TotalRange() => (High - Low);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public decimal PercentageChange() => (Close - Open) / Open * 100;
    }
}