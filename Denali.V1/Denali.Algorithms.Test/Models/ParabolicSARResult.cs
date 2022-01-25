using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Algorithms.Test.Models
{
    public class ParabolicSARResult
    {
        public decimal SAR { get; set; }
        public decimal ExtremePoint { get; set; }
        public decimal AccelerationFactor { get; set; }
        public long Time { get; set; }
    }
}
