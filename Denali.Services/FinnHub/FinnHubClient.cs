using Denali.Models.Data.FinnHub;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Denali.Services.FinnHub
{
    public class FinnHubClient
    {
        private readonly HttpClient _httpClient;
        private readonly FinnHubClientSettings _settings;

        public FinnHubClient(HttpClient httpClient, FinnHubClientSettings settings)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        }

        public async Task<CandleResponse> GetCandleData(string symbol, string resolution, string from, string to)
        {
            var path = new StringBuilder(_settings.CandlePath)
                .Append(BuildParameter("symbol", symbol, first: true))
                .Append(BuildParameter("resolution", resolution))
                .Append(BuildParameter("from", from))
                .Append(BuildParameter("to", to))
                .Append(BuildParameter("token", _settings.APIKey))
                .ToString();

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.GetAsync(path);
            }
            catch (Exception ex)
            {
                throw;
            }

            if (response.IsSuccessStatusCode)
                return JsonSerializer.Deserialize<CandleResponse>(await response.Content.ReadAsStringAsync());


            throw new Exception();
        }

        private string BuildParameter(string parameter, string value, bool first = false) => $"{(first ? "" : "&")}{parameter}={value}";
    }
}
