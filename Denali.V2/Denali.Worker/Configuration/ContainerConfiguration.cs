using Alpaca.Markets;
using Denali.Models.Mapping;
using Denali.Processors;
using Denali.Processors.ElephantStrategy;
using Denali.Services;
using Denali.TechnicalAnalysis.ElephantBars;
using InteractiveBrokers.API;
using Microsoft.Extensions.Hosting;

namespace Denali.Worker.Configuration
{
    internal class ContainerConfiguration
    {
        internal static void Configure(IConfiguration configuration, IHostEnvironment hostEnvironmnet, IServiceCollection services)
        {
            services.AddHostedService<Worker>();
            services.AddScoped<FileService>();


            services.AddOptions<ElephantStrategySettings>()
                .Bind(configuration.GetSection(ElephantStrategySettings.Settings));
            services.AddOptions<ElephantBarSettings>()
                .Bind(configuration.GetSection(ElephantBarSettings.Settings));

            services.AddScoped<ElephantStrategyAnalysis>();
            services.AddScoped<LiveTradingProcessor>();
            services.AddScoped<AlpacaService>();
            services.AddScoped<ElephantRideStrategyAnalysis>();
            services.AddScoped<ElephantRideStrategy>();
            services.AddAutoMapper(typeof(DenaliMapper));
            services.AddScoped<ElephantRestStrategyAnalysis>();

            services.AddScoped<IBService>();
            services.AddScoped<IBClient>();


            // Register a service provider so we can create scopes and resolve instances dynamically
            services.AddSingleton((context) =>
            {
                return services.BuildServiceProvider();
            });
        }
    }
}
