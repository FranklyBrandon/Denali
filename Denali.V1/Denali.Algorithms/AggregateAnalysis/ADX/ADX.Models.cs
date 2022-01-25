using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Algorithms.AggregateAnalysis.ADX
{
    public class ADXResult
    {
        public decimal DMPlus { get; set; }
        public decimal SmoothedDMPlus { get; set; }
        public decimal DMMinus { get; set; }
        public decimal SmoothedDMMinus { get; set; }
        public decimal DIPlus { get; set; }
        public decimal DIMinus { get; set; }
        public decimal DX { get; set; }
        public decimal ADX { get; set; }
        public decimal TrueRange { get; set; }
        public decimal SmoothedTrueRange { get; set; }
        public string Time { get; set; }
    }
}
