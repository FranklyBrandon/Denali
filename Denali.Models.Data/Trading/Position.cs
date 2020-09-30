using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Models.Data.Trading
{
    public class Position
    {
        public string Symbol { get; }
        public Signal Signal { get; }
        public double OpenPrice { get; }
        public double ClosePrice { get; set; }
        public long OpenTimestamp { get; set; }
        public long CloseTimestamp { get; set; }
        public bool Open { get; set; }
        public double Profit { get; set; }

        public Position(string symbol, double openPrice, Signal signal)
        {
            this.Symbol = symbol;
            this.OpenPrice = openPrice;
            this.Signal = signal;
            this.Open = true;
            this.OpenTimestamp = signal.StartTime;
        }

        public void Close(long timeStamp, double price)
        {
            Open = false;
            CloseTimestamp = timeStamp;
            ClosePrice = price;
            Profit = (ClosePrice - OpenPrice);
        }
    }
}
