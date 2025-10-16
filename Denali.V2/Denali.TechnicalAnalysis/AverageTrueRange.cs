using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.TechnicalAnalysis
{
    public class AverageTrueRange
    {
        public static decimal CalculateAverageTrueRange(int lookback, IEnumerable<IBar> data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var bars = data.ToList();

            var trueRanges = new List<decimal>();
            for (int i = 1; i < bars.Count; i++)
            {
                trueRanges.Add(CalculateTrueRange(bars[i - 1], bars[i]));
            }

            return trueRanges.TakeLast(lookback).Average();
        }

        public static decimal CalculateTrueRange(IBar previous, IBar current)
        {
            decimal highLow = current.High - current.Low;
            decimal highPrevClose = Math.Abs(current.High - previous.Close);
            decimal lowPrevClose = Math.Abs(current.Low - previous.Close);

            return Math.Max(highLow, Math.Max(highPrevClose, lowPrevClose));
        }
    }
}
