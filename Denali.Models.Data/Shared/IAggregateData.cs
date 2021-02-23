using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Models.Shared
{
    public interface IAggregateData
    {
        public string Symbol { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal ClosePrice { get; set; }
        public decimal LowPrice { get; set; }
        public decimal HighPrice { get; set; }
        public decimal Volume { get; set; }
        public long Time { get; set; }

        public bool IsOpen
        {
            get
            {
                if (OpenPrice < ClosePrice)
                    return true;
                return false;
            }
        }

        public bool IsClosed
        {
            get
            {
                if (ClosePrice < OpenPrice)
                    return true;
                return false;
            }
        }
    }
}
