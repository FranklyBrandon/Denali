using Denali.Models.Mapping;
using Denali.Processors;
using Denali.Processors.Analysis;
using Denali.Services;
using Denali.Services.Alpaca;
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

        }

        private void AddHttpClients()
        {

        }

        private void AddAppSettingsConfigs()
        {
            _services.AddScoped<DenaliSettings>((ctx) =>
            {
                return _configuration.GetSection(DenaliSettings.Key).Get<DenaliSettings>();
            });
            _services.AddScoped<AlpacaSettings>((ctx) =>
            {
                return _configuration.GetSection(AlpacaSettings.Key).Get<AlpacaSettings>();
            });
        }

        private void AddUserServices()
        {
            _services.AddScoped<IAnalysisProcessor, BarOverBarAnalysisProcessor>();
        }
    }
}
