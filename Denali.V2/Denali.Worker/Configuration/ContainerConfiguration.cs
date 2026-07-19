using Denali.Models.Mapping;
using Denali.Processors;
using Denali.Processors.DenaliClimbStrategy;
using Denali.Processors.DenaliDescentStrategy;
using Denali.Processors.GapUpFadeStrategy;
using Denali.Processors.PreMarketFadeStrategy;
using Denali.Processors.TrueFadeStrategy;
using Denali.Services;
using InteractiveBrokers.Models.Configuration;
using InteractiveBrokers.Services;


namespace Denali.Worker.Configuration
{
    internal class ContainerConfiguration
    {
        internal static void Configure(IConfiguration configuration, IHostEnvironment hostEnvironmnet, IServiceCollection services)
        {
            services.AddHostedService<Worker>();
            services.AddScoped<FileService>();
            services.AddScoped<IDateTimeService, DateTimeService>();

            services.AddSingleton<AlpacaService>();
            services.AddAutoMapper(typeof(DenaliMapper));

            services.AddOptions<InteractiveBrokersSettings>()
                .Bind(configuration.GetSection(InteractiveBrokersSettings.Settings));
            services.AddSingleton<IInteractiveBrokersClient, InteractiveBrokersClient>();
            services.AddSingleton<IInteractiveBrokersService, InteractiveBrokersService>();

            services.AddSingleton<DataLayerComponent>();
            services.AddSingleton<BrokerageLayerComponent>();

            services.AddScoped<GapUpScreener>();
            services.AddScoped<GapUpStreamer>();
            services.AddScoped<TradeManager>();

            services.AddScoped<GapUpScreenTest>();

            services.AddOptions<DenaliClimbStrategySettings>()
                .Bind(configuration.GetSection(DenaliClimbStrategySettings.Settings));
            services.AddScoped<DenaliClimbProcessor>();
            services.AddScoped<DenaliClimbHistoricProcessor>();

            services.AddOptions<DenaliDescentStrategySettings>()
                .Bind(configuration.GetSection(DenaliClimbStrategySettings.Settings));
            services.AddScoped<DenaliDescentProcessor>();
            services.AddScoped<DenaliDescentHistoricProcessor>();

            services.AddOptions<TrueFadeStrategySettings>()
                .Bind(configuration.GetSection(TrueFadeStrategySettings.Settings));

            services.AddScoped<TrueFadeIBHistoricProcessor>();
            services.AddScoped<TrueFadeScreener>();

            services.AddScoped<TrueFadeIBProcessor>();

            services.AddScoped<GapUpFadeHistoricProcessor>();
            services.AddScoped<GapUpFadeAllocator>();

            services.AddScoped<MomentumScreen>();

            services.AddScoped<TimeofDayProcessor>();

            services.AddScoped<PreMarketHours>();
            services.AddScoped<PreMarketProcessor>();

            services.AddScoped<ElephantBackLook>();

            services.AddScoped<PreMarketGainers>();

            services.AddScoped<PreMarketFadeLiveProcessor>();



            // Register a service provider so we can create scopes and resolve instances dynamically
            services.AddSingleton((context) =>
            {
                return services.BuildServiceProvider();
            });
        }
    }
}
