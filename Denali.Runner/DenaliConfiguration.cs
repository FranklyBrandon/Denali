using Denali.Processors;
using Denali.Services.Alpaca;
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
            services.AddScoped<AlpacaClientSettings>((ctx) =>
            {
                return configuration.GetSection(AlpacaClientSettings.Key).Get<AlpacaClientSettings>();
            });
            services.AddHttpClient<AlpacaClient>();
            #endregion

            services.AddScoped<AlpacaService>();
            services.AddScoped<SignalAnalysisProcessor>();
            return services.BuildServiceProvider(true);
        }
    }
}
