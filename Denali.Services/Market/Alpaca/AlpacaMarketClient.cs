using Denali.Models.Data.Alpaca;
using Denali.Services.Data.Alpaca;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Denali.Services.Market.Alpaca
{
    public class AlpacaMarketClient
    {
        private readonly HttpClient _httpClient;
        private readonly AlpacaClientSettings _settings;

        public AlpacaMarketClient(HttpClient httpClient, AlpacaClientSettings settings)
        {
            _httpClient = httpClient;
            _settings = settings;
            _httpClient.BaseAddress = new Uri(settings.MarketUrl);
            _httpClient.DefaultRequestHeaders.Add(_settings.APIKey, _settings.APIKey);
            _httpClient.DefaultRequestHeaders.Add(_settings.APISecretKey, _settings.APISecretKey);
        }

        public void PostOrder(OrderRequest orderRequest)
        {
            var content = new StringContent(JsonSerializer.Serialize(orderRequest));
            _httpClient.PostAsync(_settings.MarketUrl, content);
        }

    }
}
