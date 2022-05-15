using Denali.Models.Mapping;
using Denali.Processors.ElephantStrategy;
using Denali.Services;
using Denali.TechnicalAnalysis.ElephantBars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Worker.Configuration
{
    internal class ContainerConfiguration
    {
        internal static void Configure(IConfiguration configuration, IServiceCollection services)
        {
            services.AddHostedService<Worker>();
            services.AddScoped<FileService>();



            services.AddOptions<ElephantStrategySettings>()
                .Bind(configuration.GetSection(ElephantStrategySettings.Settings));
            services.AddOptions<ElephantBarSettings>()
                .Bind(configuration.GetSection(ElephantBarSettings.Settings));

            services.AddScoped<ElephantStrategyAnalysis>();
            services.AddAutoMapper(typeof(DenaliMapper));

            // Register a service provider so we can create scopes and resolve instances dynamically
            services.AddSingleton<ServiceProvider>((context) =>
            {
                return services.BuildServiceProvider();
            });
        }
    }
}
