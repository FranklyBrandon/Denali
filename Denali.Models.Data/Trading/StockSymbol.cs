using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Models.Trading
{
    public class StockSymbol
    {
        public string Symbol { get; set; }
        public bool Trading { get; set; }

        public StockSymbol(object symbol, object trading)
        {
            this.Symbol = symbol.ToString();
            this.Trading = trading.ToString().Equals("TRUE");
        }
    }
}
