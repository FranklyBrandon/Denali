using Denali.Models.Mapping;
using Denali.Processors;
using Denali.Services;
using Denali.Services.Alpaca;
using Denali.Services.Google;
using Denali.Services.Polygon;
using Denali.Shared.Utility;
using Denali.Strategies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            _services.AddScoped<TradingProcessor>();
        }

        private void AddHttpClients()
        {
            _services.AddHttpClient<AlpacaLightWeightClient>();
        }

        private void AddAppSettingsConfigs()
        {
            _services.AddScoped<DenaliSettings>((ctx) =>
            {
                return _configuration.GetSection(DenaliSettings.Key).Get<DenaliSettings>();
            });
            _services.AddScoped<PolygonSettings>((ctx) =>
            {
                return _configuration.GetSection(PolygonSettings.Key).Get<PolygonSettings>();
            });
            _services.AddScoped<AlpacaSettings>((ctx) =>
            {
                return _configuration.GetSection(AlpacaSettings.Key).Get<AlpacaSettings>();
            });
        }

        private void AddUserServices()
        {
            _services.AddScoped<DenaliSheetsService>();
            _services.AddScoped<GoogleSheetsService>();
            _services.AddScoped<PolygonService>();
            _services.AddScoped<TimeUtils>();
            _services.AddScoped<AlpacaService>();
        }
    }
}
