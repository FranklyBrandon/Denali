using Alpaca.Markets;
using Denali.Models.Mapping;
using Denali.Processors.ElephantStrategy;
using Denali.Services;
using Denali.TechnicalAnalysis.ElephantBars;
using Microsoft.Extensions.Hosting;

namespace Denali.Worker.Configuration
{
    internal class ContainerConfiguration
    {
        internal static void Configure(IConfiguration configuration, IHostEnvironment hostEnvironmnet, IServiceCollection services)
        {
            services.AddHostedService<Worker>();
            services.AddScoped<FileService>();


            services.AddOptions<ElephantStrategySettings>()
                .Bind(configuration.GetSection(ElephantStrategySettings.Settings));
            services.AddOptions<ElephantBarSettings>()
                .Bind(configuration.GetSection(ElephantBarSettings.Settings));

            services.AddScoped<ElephantStrategyAnalysis>();
            services.AddAutoMapper(typeof(DenaliMapper));

            RegisterAlpaca(configuration, hostEnvironmnet, services);

            // Register a service provider so we can create scopes and resolve instances dynamically
            services.AddSingleton((context) =>
            {
                return services.BuildServiceProvider();
            });
        }

        private static void RegisterAlpaca(IConfiguration configuration, IHostEnvironment hostEnvironment, IServiceCollection services)
        {
            var secretKey = new SecretKey(configuration["Alpaca:API-Key"], configuration["Alpaca:API-Secret"]);
            services.AddScoped<IAlpacaStreamingClient>((context) =>
            {
                return hostEnvironment.IsProduction() 
                    ? Alpaca.Markets.Environments.Live.GetAlpacaStreamingClient(secretKey) 
                    : Alpaca.Markets.Environments.Paper.GetAlpacaStreamingClient(secretKey);
            });
            services.AddScoped<IAlpacaDataStreamingClient>((context) =>
            {
                return hostEnvironment.IsProduction()
                    ? Alpaca.Markets.Environments.Live.GetAlpacaDataStreamingClient(secretKey)
                    : Alpaca.Markets.Environments.Paper.GetAlpacaDataStreamingClient(secretKey);
            });
            services.AddScoped<IAlpacaDataClient>((context) =>
            {
                return hostEnvironment.IsProduction()
                    ? Alpaca.Markets.Environments.Live.GetAlpacaDataClient(secretKey)
                    : Alpaca.Markets.Environments.Paper.GetAlpacaDataClient(secretKey);
            });
            services.AddScoped<IAlpacaTradingClient>((context) =>
            {
                return hostEnvironment.IsProduction()
                    ? Alpaca.Markets.Environments.Live.GetAlpacaTradingClient(secretKey)
                    : Alpaca.Markets.Environments.Paper.GetAlpacaTradingClient(secretKey);
            });
        }
    }
}
