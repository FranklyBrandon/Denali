using Denali.Models.Mapping;
using Denali.Services;

namespace Denali.WebAPI.Configuration
{
    internal class ContainerConfiguration
    {
        internal static void Configure(IServiceCollection services, IWebHostEnvironment environment)
        {
            services.AddAutoMapper(typeof(DenaliMapper));
            services.AddSingleton<AlpacaService>();
            // Register a service provider so we can create scopes and resolve instances dynamically
            services.AddSingleton((context) =>
            {
                return services.BuildServiceProvider();
            });
        }
    }
}
