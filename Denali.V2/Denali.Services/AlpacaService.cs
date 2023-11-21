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
        private SecretKey _secretKey;
        private IHostEnvironment _hostEnvironment;

        private ILogger _logger;
        public AlpacaService(IHostEnvironment hostEnvironment, IConfiguration configuration, ILogger<AlpacaService> logger)
        {
            _hostEnvironment = hostEnvironment;
            _logger = logger;
            // Best to keep these in 'User Secrets' on local and not any plain text readable configurations
            _secretKey = new SecretKey(configuration["Alpaca:API-Key"], configuration["Alpaca:API-Secret"]);
            InitializeDataClient();
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

        public void InitializeDataClient() => _alpacaDataClient = BuildDataclient();

        public void InitializeTradingclient() => _alpacaTradingClient = BuildTradingClient();

        public async Task<List<IBar>> GetAggregateData(string symbol, DateTime startTime, DateTime endTime, BarTimeFrame timeFrame, uint pageSize = 10000)
        {
            string? pageToken = default;
            List<IBar> bars = new List<IBar>();

            do
            {
                var request = new HistoricalBarsRequest(
                        symbol,
                        startTime,

                        endTime,
                        timeFrame
                ).WithPageSize(pageSize);

                if (!string.IsNullOrWhiteSpace(pageToken))
                    request.WithPageToken(pageToken);

                var response = await _alpacaDataClient.GetHistoricalBarsAsync(request).ConfigureAwait(false);
                pageToken = response.NextPageToken;
                bars.AddRange(response.Items[symbol]);

            } while (!string.IsNullOrWhiteSpace(pageToken));

            return bars;
        }

        private IAlpacaStreamingClient BuildStreamingclient() => _hostEnvironment.IsProduction()
            ? Alpaca.Markets.Environments.Live.GetAlpacaStreamingClient(_secretKey) 
            : Alpaca.Markets.Environments.Paper.GetAlpacaStreamingClient(_secretKey);

        private IAlpacaDataStreamingClient BuildDataStreamingClient() => _hostEnvironment.IsProduction()
            ? Alpaca.Markets.Environments.Live.GetAlpacaDataStreamingClient(_secretKey)
            : Alpaca.Markets.Environments.Paper.GetAlpacaDataStreamingClient(_secretKey);

        private IAlpacaDataClient BuildDataclient() => _hostEnvironment.IsProduction()
            ? Alpaca.Markets.Environments.Live.GetAlpacaDataClient(_secretKey)
            : Alpaca.Markets.Environments.Paper.GetAlpacaDataClient(_secretKey);

        private IAlpacaTradingClient BuildTradingClient() => _hostEnvironment.IsProduction()
            ? Alpaca.Markets.Environments.Live.GetAlpacaTradingClient(_secretKey)
            : Alpaca.Markets.Environments.Paper.GetAlpacaTradingClient(_secretKey);
    }
}
