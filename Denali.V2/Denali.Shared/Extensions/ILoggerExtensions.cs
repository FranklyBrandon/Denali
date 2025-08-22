using Microsoft.Extensions.Logging;

namespace Denali.Shared.Extensions
{
    public static class ILoggerExtensions
    {
        public static void NewLine(this ILogger logger)
        {
            logger.LogInformation(string.Empty);
        }
    }
}
