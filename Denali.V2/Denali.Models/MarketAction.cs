using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Models
{
    public enum MarketSide
    {
        None,
        Buy,
        Sell
    }
    public enum MarketAction
    {
        None,
        Trade
    }

    public enum OrderType
    {
        None,
        Market,
        Limit,
        StopLimit
    }
}
