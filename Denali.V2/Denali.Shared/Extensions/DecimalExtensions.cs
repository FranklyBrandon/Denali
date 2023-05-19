namespace Denali.Shared.Extensions
{
    public static class DecimalExtensions
    {
        public static decimal RoundToMoney(this decimal value) =>
            Math.Round(value, 2, MidpointRounding.AwayFromZero);

        public static decimal Round(this decimal value, int places) =>
            Math.Round(value, places, MidpointRounding.AwayFromZero);
    }
}