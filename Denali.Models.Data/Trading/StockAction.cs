using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Models.Trading
{
    public class StockAction
    {
        public string StockSymbol { get; set; }
        public Signal Signal { get; set; }
        public MarketSide Side { get; set; }
        public IExitStrategy ExitStrategy { get; set; }
    }
}
