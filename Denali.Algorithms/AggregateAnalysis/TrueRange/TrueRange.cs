using Denali.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Algorithms.AggregateAnalysis.TR
{
    public class TrueRange
    {
        public decimal Analyze(IAggregateData previousBar, IAggregateData currentBar)
        {
            var highMinusLow = Math.Abs(currentBar.HighPrice - currentBar.LowPrice);
            var highMinusLastClose = Math.Abs(currentBar.HighPrice - previousBar.ClosePrice);
            var lowMinusLastClose = Math.Abs(currentBar.LowPrice - previousBar.ClosePrice);

            return Math.Max(highMinusLow, Math.Max(highMinusLastClose, lowMinusLastClose));
        }
    }
}
