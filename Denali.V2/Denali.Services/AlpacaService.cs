using Alpaca.Markets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Denali.Services
{
    public class AlpacaService
    {
        public IAlpacaStreamingClient AlpacaStreamingclient => _alpacaStreamingclient;
        public IAlpacaDataStreamingClient AlpacaDataStreamingClient => _alpacaDataStreamingClient;
        public IAlpacaDataClient AlpacaDataClient => _alpacaDataClient;
        public IAlpacaTradingClient AlpacaTradingClient => _alpacaTradingClient;

        private IAlpacaStreamingClient _alpacaStreamingclient;
        private IAlpacaDataStreamingClient _alpacaDataStreamingClient;
        private IAlpacaDataClient _alpacaDataClient;
        private IAlpacaTradingClient _alpacaTradingClient;

        private SecretKey _paperSecretKey;
        private SecretKey _liveSecretKey;
        private IHostEnvironment _hostEnvironment;

        private ILogger _logger;
        public AlpacaService(IHostEnvironment hostEnvironment, IConfiguration configuration, ILogger<AlpacaService> logger)
        {
            _hostEnvironment = hostEnvironment;
            _logger = logger;
            // Best to keep these in 'User Secrets' on local and not any plain text readable configurations
            _paperSecretKey = new SecretKey(configuration["Alpaca:Paper:API-Key"], configuration["Alpaca:Paper:API-Secret"]);
            _liveSecretKey = new SecretKey(configuration["Alpaca:Live:API-Key"], configuration["Alpaca:Live:API-Secret"]);

            _alpacaDataClient = BuildDataclient();
            _alpacaTradingClient = BuildTradingClient();
        }

        public async Task InitializeStreamingClient()
        {
            _alpacaStreamingclient = BuildStreamingclient();
            var authStatus = await _alpacaStreamingclient.ConnectAndAuthenticateAsync();
            _logger.LogInformation($"Streaming Client Auth Status: {authStatus}");
        }

        public async Task InitializeDataStreamingClient()
        {
            _alpacaDataStreamingClient = BuildDataStreamingClient();
            var authStatus = await _alpacaDataStreamingClient.ConnectAndAuthenticateAsync();
            _logger.LogInformation($"Data Streaming Client Auth Status: {authStatus}");
        }

        public async Task InittializeTradingClientAuth()
        {
            _logger.LogInformation("Initializing Alpaca trading client...");
            var response = await _alpacaTradingClient.GetAccountConfigurationAsync();
            _logger.LogInformation($"Trading client HTTP connected: {response != null}");
           
            _logger.LogInformation("Initializing Alpaca data client...");
            var response2 = await _alpacaDataClient.ListExchangesAsync();
            _logger.LogInformation($"Data client HTTP connected: {response2 != null}");

            //_logger.LogInformation("Initializing Alapca data streaming client...");
            //await InitializeDataStreamingClient();

            _logger.LogInformation("Initializing Alpaca trading streaming client...");
            await InitializeStreamingClient();
        }

        private IAlpacaStreamingClient BuildStreamingclient() => _hostEnvironment.IsProduction()
            ? Alpaca.Markets.Environments.Live.GetAlpacaStreamingClient(_liveSecretKey) 
            : Alpaca.Markets.Environments.Paper.GetAlpacaStreamingClient(_paperSecretKey);

        private IAlpacaDataStreamingClient BuildDataStreamingClient() => 
            Alpaca.Markets.Environments.Live.GetAlpacaDataStreamingClient(_liveSecretKey); // Always use live key for data sub

        private IAlpacaDataClient BuildDataclient() =>
            Alpaca.Markets.Environments.Live.GetAlpacaDataClient(_liveSecretKey); // Always use live key for data sub

        private IAlpacaTradingClient BuildTradingClient() => _hostEnvironment.IsProduction()
            ? Alpaca.Markets.Environments.Live.GetAlpacaTradingClient(_paperSecretKey)
            : Alpaca.Markets.Environments.Paper.GetAlpacaTradingClient(_paperSecretKey);
    }
}
