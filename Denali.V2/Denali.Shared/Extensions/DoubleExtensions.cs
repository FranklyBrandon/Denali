using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Shared.Extensions
{
    public static class DoubleExtensions
    {
        public static double RoundToFourPlaces(this double value) =>
            Round(value, 4);
        public static double Round(this double value, int places) =>
            Math.Round(value, places, MidpointRounding.AwayFromZero);
    }
}
