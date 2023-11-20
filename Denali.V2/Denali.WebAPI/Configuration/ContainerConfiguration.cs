using Denali.Services;
using System.Net;

namespace Denali.WebAPI.Configuration
{
    internal class ContainerConfiguration
    {
        internal static void Configure(IServiceCollection services, IWebHostEnvironment environment)
        {
            services.AddSingleton<AlpacaService>();
            // Register a service provider so we can create scopes and resolve instances dynamically
            services.AddSingleton((context) =>
            {
                return services.BuildServiceProvider();
            });
        }
    }
}
