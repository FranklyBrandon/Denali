using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Models
{
    public class GapUpStock
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Last { get; set; }
        public string Change { get; set; }
        public string ChangePercent { get; set; }
        public string GapUp { get; set; }
        public string GapUpPercent { get; set; }
        public string High { get; set; }
        public string Low { get; set; }
        public string Volume { get; set; }
    }
}
