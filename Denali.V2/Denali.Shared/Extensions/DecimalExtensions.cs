using System.Runtime.CompilerServices;

namespace Denali.Shared.Extensions
{
    public static class DecimalExtensions
    {
        public static decimal RoundToMoney(this decimal value) =>
            Round(value, 2);

        public static decimal Round(this decimal value, int places) =>
            Math.Round(value, places, MidpointRounding.AwayFromZero);
    }
}