using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Denali.Services.AlphaAdvantage
{
    public interface IAlphaAdvanatgeClient
    {
        Task<QuotesResponse> GetQuote(string symbol);
    }

    public class AlphaAdvanatgeClient : IAlphaAdvanatgeClient
    {
        private readonly HttpClient _httpClient;
        private readonly AlphaAdvantageClientSettings _settings;

        public AlphaAdvanatgeClient(HttpClient httpClient, IOptions<AlphaAdvantageClientSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _httpClient.BaseAddress = new Uri(_settings.BasePath);
        }

        public async Task<QuotesResponse> GetQuote(string symbol)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(string.Format(_settings.QuotePath, symbol, _settings.APIKey), UriKind.Relative)
            };

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                    return JsonSerializer.Deserialize<QuotesResponse>(await response.Content.ReadAsStringAsync());

                throw new HttpRequestException($"Bad response from Algo Advantage: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
