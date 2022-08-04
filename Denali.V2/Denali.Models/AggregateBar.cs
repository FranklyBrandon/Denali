using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Models
{
    public interface IAggregateBar
    {
        string Symbol { get; }

        public decimal Open { get; set; }

        public decimal Close { get; set; }

        public decimal Low { get; set; }

        public decimal High { get; set; }

        public decimal Volume { get; set; }

        public DateTime TimeUtc { get; set; }

        public decimal Vwap { get; set; }

        public ulong TradeCount { get; set; }

        public void SetSymbol(string symbol);

        public decimal BodyRange();

        public decimal TotalRange();

        public decimal PercentageChange();
        public bool Green();
    }

    public class AggregateBar : IAggregateBar
    {
        public string Symbol { get; private set; } = String.Empty;

        public decimal Open { get; set; }

        public decimal Close { get; set; }

        public decimal Low { get; set; }

        public decimal High { get; set; }

        public decimal Volume { get; set; }

        public DateTime TimeUtc { get; set; }

        public decimal Vwap { get; set; }

        public ulong TradeCount { get; set; }
        public double Returns { get; set; }

        public void SetSymbol(string symbol) => Symbol = symbol;

        public decimal BodyRange() => Math.Abs(Open - Close);

        public decimal TotalRange() => (High - Low);

        public decimal PercentageChange() => (Close - Open) / Open * 100;
        public bool Green() => (Close > Open);
    }
}
