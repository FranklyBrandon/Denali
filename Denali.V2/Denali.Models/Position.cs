using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Models
{
    public record Position(string Symbol, MarketSide Side, DateTime DateTimeUTC, decimal EnterPrice, int Size = 1)
    {
        public decimal Net(decimal closePrice)
        {
            var net = (closePrice - EnterPrice) * Size;
            return Side == MarketSide.Buy ? net : net * -1;         
        }
    }
}
