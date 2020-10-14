using Denali.Algorithms.Bar;
using Denali.Processors;
using Denali.Services;
using Denali.Services.Data;
using Denali.Services.Data.Alpaca;
using Denali.Services.Google;
using Denali.Services.Polygon;
using Denali.Services.Settings;
using Denali.Services.Utility;
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

            AddLogging();
            AddProcessors();
            AddAppSettingsConfigs();
            AddHttpClients();
            AddUserServices();

            var provider = _services.BuildServiceProvider();
            _services.AddSingleton<ServiceProvider>((ctx) =>
            {
                return provider;
            });
        }

        private void AddProcessors()
        {
            switch (_configuration["status"])
            {
                case "live":
                    _services.AddScoped<IProcessor, LiveTradingProcessor>();
                    break;
                case "historic":
                    _services.AddScoped<IProcessor, HistoricTradingProcessor>();
                    break;
                default:
                    throw new ArgumentException("No processor status provided");

            }
        }

        private void AddLogging()
        {
            _services.AddLogging(loggingBuilder => loggingBuilder
                .AddConsole()
                .AddDebug()
                .SetMinimumLevel(LogLevel.Debug));
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

        }

        private void AddUserServices()
        {
            _services.AddScoped<DenaliSheetsService>();
            _services.AddScoped<GoogleSheetsService>();
            _services.AddSingleton<TimeUtils>();
            _services.AddScoped<BarAlgorithmAnalysis>();
            _services.AddScoped<PolygonService>();
        }
    }
}
