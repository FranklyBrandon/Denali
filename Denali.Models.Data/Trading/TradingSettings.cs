using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Models.Data.Trading
{
    public class TradingSettings
    {
        public List<string> Symbols { get; set; }
        public bool Trading { get; set; }
    }
}
