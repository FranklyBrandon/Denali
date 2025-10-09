using Denali.Processors.DenaliClimbStrategy;
using Denali.Services;

namespace Denali.API.Configuration
{
    internal class ContainerConfiguration
    {
        internal static void RegisterServices(IConfiguration configuration, IWebHostEnvironment hostEnvironmnet, IServiceCollection services)
        {
            services.AddSingleton<AlpacaService>();
            services.AddSingleton<DataLayerComponent>();
            services.AddScoped<AggregateDataService>();
            services.AddScoped<GapUpStreamer>();
            services.AddScoped<GapUpScreener>();
            services.AddOptions<DenaliClimbStrategySettings>()
                .Bind(configuration.GetSection(DenaliClimbStrategySettings.Settings));
        }
    }
}
