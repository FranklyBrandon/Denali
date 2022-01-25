using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Models.Shared
{
    public class Transaction
    {
        public Transaction(decimal buyPrice, long buyTime)
        {
            this.BuyPrice = buyPrice;
            this.BuyTime = buyTime;
        }

        public decimal BuyPrice { get; set; }
        public decimal SellPrice { get; set; }
        public long BuyTime { get; set; }
        public long SellTime { get; set; }
        public decimal High { get; set; }

        public decimal NetGain => SellPrice - BuyPrice;

    }
}
