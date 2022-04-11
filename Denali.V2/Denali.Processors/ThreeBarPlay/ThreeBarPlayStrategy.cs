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
