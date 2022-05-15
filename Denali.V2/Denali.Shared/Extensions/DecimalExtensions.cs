namespace Denali.Shared.Extensions
{
    public static class DecimalExtensions
    {
        public static decimal RoundToMoney(this decimal value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }
    }
}