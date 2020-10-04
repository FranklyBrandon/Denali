using Denali.Services.Utility;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Worker
{
    public static class DenaliConfiguration
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<DenaliWorker>();
            services.AddSingleton<TimeUtils>();

            var provider = services.BuildServiceProvider();
            services.AddSingleton<ServiceProvider>((ctx) =>
            {
                return provider;
            });
        }
    }
}
