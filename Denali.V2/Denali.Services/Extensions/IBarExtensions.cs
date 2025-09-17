using Alpaca.Markets;

namespace Denali.Services.Extensions
{
    public static class IBarExtensions
    {
        public static bool IsGreen(this IBar bar) => bar.Close >= bar.Open;
    }
}
