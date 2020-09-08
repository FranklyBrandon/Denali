using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Models.Data.Chart.CandleStick
{
    public class CandleStickChart
    {
        public string Symbol { get; }
        public DateTimeWithZone From { get; }
        public DateTimeWithZone To { get; }
        public IEnumerable<CandleStick> CandleSticks { get; }

        public CandleStickChart(string symbol, DateTimeWithZone from, DateTimeWithZone to, IEnumerable<CandleStick> candleSticks)
        {
            Symbol = symbol;
            From = from;
            To = to;
            CandleSticks = candleSticks;
        }
    }
}
