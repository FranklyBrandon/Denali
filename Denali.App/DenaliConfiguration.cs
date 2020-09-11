using Denali.App;
using Denali.Services;
using Denali.Services.Analysis;
using Denali.Services.FinnHub;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Denali.Runner
{
    internal static class DenaliConfiguration
    {
        internal static IServiceProvider ServiceProvider;
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

            services.AddSingleton<FinnHubService>();
            services.AddSingleton<DenaliAppServices>();
            services.AddSingleton<HistoricAnalysisService>();

            services.AddSingleton<MainWindow>();
            return ServiceProvider = services.BuildServiceProvider(true);
        }
    }
}
