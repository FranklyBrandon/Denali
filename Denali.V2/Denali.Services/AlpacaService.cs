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
            _secretKey = new SecretKey(configuration["Alpaca:API-Key"], configuration["Alpaca:API-Secret"]);
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

        public async Task<IEnumerable<IIntervalCalendar>> GeOpenMarketDays(int pastDays, DateTime day)
        {
            var calenders = await _alpacaTradingClient.ListIntervalCalendarAsync(new CalendarRequest().SetInclusiveTimeInterval(day.AddDays(-pastDays), day));
            return calenders.OrderByDescending(x => x.GetTradingDate());
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
