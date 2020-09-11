using Denali.Processors;
using Denali.Services.FinnHub;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Denali.Runner
{
    internal static class DenaliConfiguration
    {
        internal static IServiceProvider Startup() => BuildServiceProvider(BuildConfiguration());

        private static IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        private static IServiceProvider BuildServiceProvider(IConfiguration configuration)
        {
            IServiceCollection services = new ServiceCollection();

            #region Http Clients
            services.AddScoped<FinnHubClientSettings>((ctx) =>
            {
                return configuration.GetSection(FinnHubClientSettings.Key).Get<FinnHubClientSettings>();
            });
            services.AddHttpClient<FinnHubClient>();
            #endregion

            services.AddScoped<FinnHubService>();
            services.AddScoped<HistoricAnalysisPrcessor>();
            return services.BuildServiceProvider(true);
        }
    }
}
