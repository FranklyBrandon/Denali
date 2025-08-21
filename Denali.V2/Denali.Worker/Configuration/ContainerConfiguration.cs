using Denali.Models.Mapping;
using Denali.Processors;
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

            services.AddScoped<AlpacaService>();
            services.AddAutoMapper(typeof(DenaliMapper));

            services.AddOptions<InteractiveBrokersSettings>()
                .Bind(configuration.GetSection(InteractiveBrokersSettings.Settings));
            services.AddScoped<IInteractiveBrokersClient, InteractiveBrokersClient>();
            services.AddScoped<IInteractiveBrokersService, InteractiveBrokersService>();

            services.AddScoped<DenaliClimbProcessor>();


            // Register a service provider so we can create scopes and resolve instances dynamically
            services.AddSingleton((context) =>
            {
                return services.BuildServiceProvider();
            });
        }
    }
}
