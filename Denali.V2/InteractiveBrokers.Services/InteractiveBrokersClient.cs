using InteractiveBrokers.Models.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteractiveBrokers.Services
{
    public interface IInteractiveBrokersClient
    {
        Task Ping();
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
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/120.0.0.0 Safari/537.36");
        }

        public async Task Ping()
        {
            var response = await _httpClient.GetAsync(_settings.PingGateway);
            var content = await response.Content.ReadAsStringAsync();
        }

        public async Task BrokerageInit()
        {
            var response = await _httpClient.PostAsync(_settings.BrokerageInit, null);
        }

        public async Task HMDSInit()
        {
            var response = await _httpClient.PostAsync(_settings.HMDSInit, null);
        }
    }
}
