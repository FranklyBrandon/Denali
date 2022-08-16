using Alpaca.Markets;
using Denali.Models.Mapping;
using Denali.Processors;
using Denali.Processors.ElephantStrategy;
using Denali.Processors.StatArb;
using Denali.Services;
using Denali.Services.Aggregators;
using Denali.Services.AlphaAdvantage;
using Denali.Services.PythonInterop;
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
            services.AddScoped<BarAggregator>();
            services.AddScoped<TradeAggregator>();
            services.AddScoped<PairAnalysis>();

            services.AddScoped<IBService>();
            services.AddScoped<IBClient>();

            services.AddHttpClient<IPythonInteropClient, PythonInteropClient>();
            services.AddOptions<PythonInteropClientSettings>()
                .Bind(configuration.GetSection(PythonInteropClientSettings.Settings));

            services.AddHttpClient<IAlphaAdvanatgeClient, AlphaAdvanatgeClient>();
            services.AddOptions<AlphaAdvantageClientSettings>()
                .Bind(configuration.GetSection(AlphaAdvantageClientSettings.Settings));

            services.AddScoped<PairTradeStrategy>();


            // Register a service provider so we can create scopes and resolve instances dynamically
            services.AddSingleton((context) =>
            {
                return services.BuildServiceProvider();
            });
        }
    }
}
