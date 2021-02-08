using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Algorithms.AggregateAnalysis.ADX
{
    public class ADXResult
    {
        public decimal DIPlus { get; set; }
        public decimal DIMinus { get; set; }
        public decimal ADX { get; set; }
        public decimal TrueRange { get; set; }
    }
}
