using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Denali.Services.YahooFinanceService
{
    public interface IYahooFinanceClient
    {
        Task<QuotesResponse> GetLatestQuote(string symbol);
        Task<QuotesResponse> GetQuotes(string symbol, string interval, string range);
    }

    public class YahooFinanceClient : IYahooFinanceClient
    {
        private readonly HttpClient _httpClient;
        private readonly YahooFinanceClientSettings _settings;

        public YahooFinanceClient(HttpClient httpClient, IOptions<YahooFinanceClientSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _httpClient.BaseAddress = new Uri(_settings.BasePath);
        }

        public async Task<QuotesResponse> GetLatestQuote(string symbol) =>
            await GetQuotes(symbol, "1m", "1d");
        

        public async Task<QuotesResponse> GetQuotes(string symbol, string interval, string range)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(string.Format(_settings.QuotePath, symbol, interval, range), UriKind.Relative)
            };

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                    return JsonSerializer.Deserialize<QuotesResponse>(await response.Content.ReadAsStringAsync());

                throw new HttpRequestException($"Bad response from Yahoo Finance: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
