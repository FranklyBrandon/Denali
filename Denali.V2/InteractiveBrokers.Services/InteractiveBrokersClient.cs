using InteractiveBrokers.Models.Configuration;
using InteractiveBrokers.Models.Response;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace InteractiveBrokers.Services
{
    public interface IInteractiveBrokersClient
    {
        Task<Ping> Ping();
        Task<AuthStatus> BrokerageInit();
        Task<AuthStatus> HMDSInit();
        Task<HistoricAggregateResponse> GetHistoricAggregates(int conId, DateTime startDateTime, string period = "1d", string bar = "mins", string barType = "Last", bool outsideRth = false);
        Task<List<Contract>> GetAllContractsByExchange(string exchange);
    }

    public class InteractiveBrokersClient : IInteractiveBrokersClient
    {
        private readonly InteractiveBrokersSettings _settings;
        private readonly HttpClient _httpClient;

        public InteractiveBrokersClient(IOptions<InteractiveBrokersSettings> settings)
        {
            _settings = settings.Value;
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
        public async Task<HistoricAggregateResponse> GetHistoricAggregates(int conId, DateTime startDateTime, string period = "1d", string bar = "mins", string barType = "Last", bool outsideRth = false)
        {
            var startTimeString = startDateTime.ToString("yyyMMdd-HH:mm:ss");
            var url = $"{_settings.HistoricAggregate}?conid={conId}&startTime={startTimeString}&period={period}&bar={bar}&barType={barType}&outsideRth={outsideRth}";
            return await Get<HistoricAggregateResponse>(url);
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
