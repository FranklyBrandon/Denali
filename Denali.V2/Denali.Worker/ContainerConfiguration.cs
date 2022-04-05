using Denali.Processors.ThreeBarPlay;
using Denali.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Worker
{
    internal class ContainerConfiguration
    {
        internal static void Configure(IServiceCollection services)
        {
            services.AddHostedService<Worker>();
            services.AddScoped<FileService>();
            services.AddScoped<ThreeBarPlayAnalysis>();

            // Register a service provider so we can create scopes and resolve instances dynamically
            services.AddSingleton<ServiceProvider>((context) =>
            {
                return services.BuildServiceProvider();
            });
        }         
    }
}
