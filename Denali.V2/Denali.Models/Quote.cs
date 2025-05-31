using Denali.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Models
{
    public class Quote
    {
        public string Symbol { get; set; }
        public DateTime TimeStampUTC { get; set; }
        public decimal AskPrice { get; set; }
        public decimal BidPrice { get; set; }

        public decimal MidPoint => (AskPrice + BidPrice) / 2;

        public void RoundToSeconds()
        {
            TimeStampUTC = TimeStampUTC.Truncate(TimeSpan.FromSeconds(1));
        }
    }
}
