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
            Math.Round(value, 4, MidpointRounding.AwayFromZero);
    }
}
