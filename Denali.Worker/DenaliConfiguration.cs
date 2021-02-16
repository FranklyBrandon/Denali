using Denali.Algorithms.AggregateAnalysis;
using Denali.Algorithms.AggregateAnalysis.ADX;
using Denali.Models.Mapping;
using Denali.Processors;
using Denali.Services;
using Denali.Services.Data;
using Denali.Services.Data.Alpaca;
using Denali.Services.Google;
using Denali.Services.Polygon;
using Denali.Services.Settings;
using Denali.Shared.Utility;
using Denali.Strategies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Worker
{
    public class DenaliConfiguration
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceCollection _services;

        public DenaliConfiguration(IConfiguration configuration, IServiceCollection services)
        {
            this._configuration = configuration;
            this._services = services;
        }
        public void ConfigureServices()
        {
            _services.AddHostedService<DenaliWorker>();
 
            AddProcessors();
            AddAppSettingsConfigs();
            AddHttpClients();
            AddUserServices();

            _services.AddAutoMapper(typeof(LiveTradingProfile));

            var provider = _services.BuildServiceProvider();
            _services.AddSingleton<ServiceProvider>((ctx) =>
            {
                return provider;
            });
        }

        private void AddProcessors()
        {
            _services.AddScoped<IProcessor, LiveTradingProcessor>();
        }

        private void AddAppSettingsConfigs()
        {
            _services.AddScoped<DenaliSettings>((ctx) =>
            {
                return _configuration.GetSection(DenaliSettings.Key).Get<DenaliSettings>();
            });
            _services.AddScoped<AlpacaClientSettings>((ctx) =>
            {
                return _configuration.GetSection(AlpacaClientSettings.Key).Get<AlpacaClientSettings>();
            });
            _services.AddScoped<AuthenticationSettings>((ctx) =>
            {
                return _configuration.GetSection(AuthenticationSettings.Key).Get<AuthenticationSettings>();
            });
            _services.AddScoped<PolygonSettings>((ctx) =>
            {
                return _configuration.GetSection(PolygonSettings.Key).Get<PolygonSettings>();
            });
        }

        private void AddHttpClients()
        {
            _services.AddHttpClient<PolygonClient>();
        }

        private void AddUserServices()
        {
            _services.AddScoped<DenaliSheetsService>();
            _services.AddScoped<GoogleSheetsService>();
            _services.AddScoped<BarAlgorithmAnalysis>();
            _services.AddScoped<PolygonService>();
            _services.AddScoped<PolygonStreaming>();
            _services.AddScoped<TimeUtils>();
            _services.AddScoped<IAggregateStrategy, ScalpStrategy>();
        }
    }
}
