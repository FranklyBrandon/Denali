using Denali.Models.Data.Trading;
using Denali.Services;
using Denali.Services.Data.Alpaca;
using Denali.Services.Google;
using Denali.Services.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Worker
{
    public static class DenaliConfiguration
    {
        public static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            services.AddHostedService<DenaliWorker>();
            services.AddSingleton<TimeUtils>();

            var provider = services.BuildServiceProvider();
            services.AddSingleton<ServiceProvider>((ctx) =>
            {
                return provider;
            });

            AddAppSettingsConfigs(configuration, services);
            AddHttpClients(configuration, services);
            AddUserServices(configuration, services);
        }

        private static void AddAppSettingsConfigs(IConfiguration configuration, IServiceCollection services)
        {
            services.AddScoped<DenaliSettings>((ctx) =>
            {
                return configuration.GetSection(DenaliSettings.Key).Get<DenaliSettings>();
            });
        }

        private static void AddHttpClients(IConfiguration configuration, IServiceCollection services)
        {
            services.AddHttpClient<AlpacaClient>();
        }

        private static void AddUserServices(IConfiguration configuration, IServiceCollection services)
        {
            services.AddScoped<DenaliSheetsService>();
            services.AddScoped<GoogleSheetsService>();
        }
    }
}
