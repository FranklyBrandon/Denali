using Denali.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Denali.Algorithms.AggregateAnalysis.CandlestickPattern
{
    public class Engulfing
    {
        public bool IsEngulfing(IEnumerable<IAggregateData> barData, MarketSide side)
        {
            var size = barData.Count() - 1;
            var previous = barData.ElementAtOrDefault(size - 1);
            var current = barData.ElementAtOrDefault(size);

            if (previous == null || current == null)
                return false;

            return IsEngulfing(previous, current, side);
        }

        private bool IsEngulfing(IAggregateData previous, IAggregateData current, MarketSide side)
        {
            if (side == MarketSide.Bullish && previous.IsClosed && current.IsOpen)
            {
                return (previous.OpenPrice < current.ClosePrice && previous.ClosePrice > current.OpenPrice);
            }
            else if (side == MarketSide.Bearish && previous.IsOpen && current.IsClosed)
            {
                return (previous.ClosePrice < current.OpenPrice && previous.OpenPrice > current.ClosePrice);
            }

            return false;
        }
    }
}
