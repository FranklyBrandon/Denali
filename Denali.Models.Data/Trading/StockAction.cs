using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Models.Data.Trading
{
    public class StockAction
    {
        public string StockSymbol { get; set; }
        public Signal Signal { get; set; }
        public MarketSide Side { get; set; }
        public IExitStrategy ExitStrategy { get; set; }
    }
}
