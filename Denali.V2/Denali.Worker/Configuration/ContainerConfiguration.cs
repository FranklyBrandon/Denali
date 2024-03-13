using Alpaca.Markets;
using Denali.Models.Mapping;
using Denali.Processors;
using Denali.Processors.GapMomentum;
using Denali.Processors.MartingaleBasis;
using Denali.Processors.StatArb;
using Denali.Processors.VolatileUniverse;
using Denali.Services;
using Denali.Services.Aggregators;
using Denali.Services.PythonInterop;
using Denali.Services.YahooFinanceService;
using Denali.TechnicalAnalysis.ElephantBars;

namespace Denali.Worker.Configuration
{
    internal class ContainerConfiguration
    {
        internal static void Configure(IConfiguration configuration, IHostEnvironment hostEnvironmnet, IServiceCollection services)
        {
            services.AddHostedService<Worker>();
            services.AddScoped<FileService>();

            services.AddScoped<AlpacaService>();
            services.AddAutoMapper(typeof(DenaliMapper));
            services.AddScoped<ScalpingAnalysicProcessor>();
            services.AddScoped<VolatileUniverseAnalysisProcessor>();
            services.AddOptions<NASDAQ100Settings>()
                .Bind(configuration.GetSection(NASDAQ100Settings.Settings));


            // Register a service provider so we can create scopes and resolve instances dynamically
            services.AddSingleton((context) =>
            {
                return services.BuildServiceProvider();
            });
        }
    }
}
