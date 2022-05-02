using Denali.Models.Mapping;
using Denali.Processors.ThreeBarPlay;
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



            services.AddOptions<ThreeBarPlaySettings>()
                .Bind(configuration.GetSection(ThreeBarPlaySettings.Settings));
            services.AddOptions<ElephantBarSettings>()
                .Bind(configuration.GetSection(ElephantBarSettings.Settings));

            services.AddScoped<ThreeBarPlayAnalysis>();
            services.AddAutoMapper(typeof(DenaliMapper));

            // Register a service provider so we can create scopes and resolve instances dynamically
            services.AddSingleton<ServiceProvider>((context) =>
            {
                return services.BuildServiceProvider();
            });
        }
    }
}
