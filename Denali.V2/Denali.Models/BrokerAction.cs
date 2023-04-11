using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Models
{
    public record BrokerAction(
        MarketAction action, 
        MarketSide side = MarketSide.None, 
        OrderType orderType = OrderType.None, 
        decimal price = 0.0m)

    {
        public MarketSide Close() =>
            side switch
            {
                MarketSide.Buy => MarketSide.Sell,
                MarketSide.Sell => MarketSide.Buy,
                MarketSide.None => throw new ArgumentException("Market side of 'None' has no inverse"),
                _ => throw new ArgumentException("Unkown market side")
            };
        
    }
}
