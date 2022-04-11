using Denali.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors.ThreeBarPlay
{
    public class ThreeBarPlayStrategy
    {
        private bool _ignitionTripped = false;
        public void ProcessTick(List<Bar> barData)
        {
            if (_ignitionTripped)
                processConsolidation(barData);
            else
                proccessIgnition(barData);
        }

        private void processConsolidation(List<Bar> barData)
        {

        }

        private void proccessIgnition(List<Bar> bardata)
        {
            var currentBar = bardata.Last();

            if (currentBar.PercentageChange() > 0.50m)
                Console.WriteLine($"Ignition Candle Detected at {currentBar.TimeUtc} with change of {currentBar.PercentageChange()}!");
        }
    }
}

/*
 - The size of the candle is determined by comparing a range of previous candles (you can set the amount at your discretion)
- Search factor: by default 1.3, this means that all bars that have a range greater than the average range of previous candles + 30%, are considered elephant candles (can be configured at your discretion)
- Possibility to configure the percentage of the body that the elephant candle must have.
- Possibility of filtering up to 2 means with direction detection and color change (fully configurable)
- Possibility of filtering by mobile averages

https://www.tradingview.com/script/yV11ifDS-Elephant-Bar-by-Oliver-Velez/
 */
