using Denali.App;
using Denali.App.Widgets;
using Denali.App.Widgets.HistoricAnalysis;
using Denali.Services;
using Denali.Services.Analysis;
using Denali.Services.FinnHub;
using Denali.Services.Utility;
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
            services.AddSingleton<FinnHubClientSettings>((ctx) =>
            {
                return configuration.GetSection(FinnHubClientSettings.Key).Get<FinnHubClientSettings>();
            });
            services.AddHttpClient<FinnHubClient>();
            #endregion

            services.AddTransient<FinnHubService>();
            services.AddTransient<HistoricAnalysisService>();

            services.AddTransient<HistoricAnalysisWidget>();

            services.AddTransient<WidgetFactory>();

            services.AddTransient<MainWindow>();
            services.AddTransient<TimeUtils>();

            return ServiceProvider = services.BuildServiceProvider(true);
        }
    }
}
