using Alpaca.Markets;
using Denali.Processors.TrueFadeStrategy;
using Denali.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors.GapUpFadeStrategy
{
    public class GapUpFadeAllocator
    {
        //private readonly DataLayerComponent _dataLayer;

        public async Task<IEnumerable<GapUpFadePosition>> Allocate(IEnumerable<GapUpFadeSignal> signals, IIntervalCalendar currentDay, decimal capitalToTrade)
        {
            //var backlogData = await _dataLayer.GetAggregateDataMulti(signals.Select(x => x.Symbol), currentDay.GetTradingOpenTimeUtc().AddDays(-10), currentDay.GetTradingOpenTimeUtc(), BarTimeFrame.Day);

            IEnumerable<GapUpFadePosition> positions = signals.Select(x => new GapUpFadePosition(x)).ToList();

            bool allocate = true;
            while (allocate)
            {
                bool allocatedThisRound = false;
                foreach (var position in positions)
                {
                    //if ((position.PositionSize + 1) / position.Signal.AverageVolume > maximumVolumePercentage / 100)
                    //    continue;

                    if (capitalToTrade > position.Signal.LastPrice)
                    {
                        position.PositionSize++;
                        capitalToTrade -= position.Signal.LastPrice;
                        allocatedThisRound = true;
                    }
                    else
                    {
                        allocate = false;
                        break;
                    }
                }

                // If no allocatations, everything is volume capped
                if (!allocatedThisRound)
                    allocate = false;
            }

            return positions;

        }
    }

    public record GapUpFadePosition(GapUpFadeSignal Signal)
    {
        public int PositionSize { get; set; }
        public decimal AverageVolume { get; set; }

        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Commision { get; set; }
        public decimal PerStockProfit { get; set; }
        public decimal TotalProfit { get; set; } // does not include commision
        public decimal GrossProfit { get; set; } // does include commision
    }
}
