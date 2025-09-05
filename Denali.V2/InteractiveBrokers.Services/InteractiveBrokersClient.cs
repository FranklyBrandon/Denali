using InteractiveBrokers.Models.Configuration;
using InteractiveBrokers.Models.Response;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;


namespace InteractiveBrokers.Services
{
    public interface IInteractiveBrokersClient
    {
        Task<Ping> Ping();
        Task<AuthStatus> BrokerageInit();
        Task<AuthStatus> HMDSInit();
        Task<HistoricAggregateResponse> GetHistoricAggregates(Contract contract, DateTime startDateTime);
        Task<HistoricAggregateResponse> GetHistoricAggregatesBeta(int conId, DateTime startDateTime, string period = "1d", string bar = "mins", string barType = "Last", bool outsideRth = false);
        Task<List<Contract>> GetAllContractsByExchange(string exchange);
    }

    public class InteractiveBrokersClient : IInteractiveBrokersClient
    {
        private readonly InteractiveBrokersSettings _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public InteractiveBrokersClient(IOptions<InteractiveBrokersSettings> settings, ILogger<InteractiveBrokersClient> logger)
        {
            _settings = settings.Value;
            _logger = logger;
            var httpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, error) =>
                {
                    // Bypass SSL certificate for IB gateway
                    if (message.RequestUri.Host == "localhost")
                        return true;

                    return false;
                }
            };
            _httpClient = new HttpClient(httpHandler);
            _httpClient.BaseAddress = new Uri(_settings.GatewayBaseURL);
            // Impersonate a browser, needed for IB gateway otherwise a 403 is returned
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/120.0.0.0 Safari/537.36");
        }

        public async Task<Ping> Ping() => await Get<Ping>(_settings.PingGateway);
        public async Task<AuthStatus> BrokerageInit() => await Get<AuthStatus>(_settings.BrokerageInit);
        public async Task<AuthStatus> HMDSInit() => await Get<AuthStatus>(_settings.HMDSInit);

        public async Task<HistoricAggregateResponse> GetHistoricAggregates(Contract contract, DateTime startDateTime)
        {
            // conid=265598&exchange=AMEX&period=1d&bar=1min&outsideRth=false&direction=1&startTime=20250819-09:30:00
            var startTimeString = startDateTime.ToString("yyyMMdd-HH:mm:ss");
            var url = $"{_settings.HistoricAggregate}?conid={contract.conid}&exchange={contract.exchange}&startTime={startTimeString}&period=1d&bar=1min&outsideRth=false&direction=1";
            return await Get<HistoricAggregateResponse>(url);
        }
        public async Task<HistoricAggregateResponse> GetHistoricAggregatesBeta(int conId, DateTime startDateTime, string period = "1d", string bar = "mins", string barType = "Last", bool outsideRth = false)
        {
            var startTimeString = startDateTime.ToString("yyyMMdd-HH:mm:ss");
            var url = $"{_settings.HistoricAggregateBeta}?conid={conId}&startTime={startTimeString}&period={period}&bar={bar}&barType={barType}&outsideRth={outsideRth}";

            int attempt = 1;
            int maxRetries = 3;

            while (attempt < maxRetries)
            {
                try
                {
                    var response = await Get<HistoricAggregateResponse>(url);

                    if (response?.data == null || response.data.Count == 0)
                        throw new Exception();

                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"Failure fetching data on attempt {attempt}, retrying...");
                }

                attempt++;
                await Task.Delay(TimeSpan.FromSeconds(1 * attempt));
            }

            throw new HttpRequestException();
        }

        public async Task<List<Contract>> GetAllContractsByExchange(string exchange)
        {
            var url = $"{_settings.ContractIdsByExchange}?exchange={exchange}";
            return await Get<List<Contract>>(url);
        }

        private async Task<T> Get<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            return await ProcessResponse<T>(response);
        }

        private async Task<T> ProcessResponse<T>(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content);
        }
    }
}
