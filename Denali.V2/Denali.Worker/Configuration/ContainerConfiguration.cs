using Denali.Models.Mapping;
using Denali.Processors.DenaliClimbStrategy;
using Denali.Processors.VolatileUniverse;
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

            services.AddOptions<DenaliClimbStrategySettings>()
                .Bind(configuration.GetSection(DenaliClimbStrategySettings.Settings));
            services.AddScoped<DenaliClimbIBProcessor>();


            // Register a service provider so we can create scopes and resolve instances dynamically
            services.AddSingleton((context) =>
            {
                return services.BuildServiceProvider();
            });
        }
    }
}
