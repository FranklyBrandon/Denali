using Alpaca.Markets;

namespace Denali.Services.Extensions
{
    public static class IBarExtensions
    {
        public static bool IsGreen(this IBar bar) => bar.Close >= bar.Open;
        public static bool HasMoved(this IBar bar) => bar.Open != bar.Close && bar.High != bar.Low;
    }
}
