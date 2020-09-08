using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Models.Data.Chart.CandleStick
{
    public class CandleStick
    {
        public double OpenPrice { get; }
        public double ClosePrice { get; }
        public double LowPrice { get; }
        public double HighPrice { get; }
        public double Volume { get; }
        public int Timestamp { get; }

        public CandleStick(double openPrice, double closePrice, double lowPrice, double highPrice, double volume, int timestamp)
        {
            OpenPrice = openPrice;
            ClosePrice = closePrice;
            LowPrice = lowPrice;
            HighPrice = highPrice;
            Volume = volume;
            Timestamp = timestamp;
        }
    }
}
