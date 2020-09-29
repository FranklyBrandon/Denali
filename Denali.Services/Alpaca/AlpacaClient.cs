using Denali.Models;
using Denali.Models.Data.Alpaca;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Denali.Services.Alpaca
{
    public class AlpacaClient
    {
        private readonly HttpClient _httpClient;
        private readonly AlpacaClientSettings _settings;

        public AlpacaClient(HttpClient httpClient, AlpacaClientSettings settings)
        {
            _httpClient = httpClient;
            _settings = settings;
            _httpClient.BaseAddress = new Uri(settings.DataUrl);
            httpClient.DefaultRequestHeaders.Add("APCA-API-KEY-ID", _settings.APIKey);
            httpClient.DefaultRequestHeaders.Add("APCA-API-SECRET-KEY", _settings.APISecretKey);
        }

        public async Task<Dictionary<string, List<Candle>>> GetBars(string resolution, int limit, string start, string end, params string[] symbols)
        {
            var path = BuildBarsUri(resolution, limit, start, end, symbols);

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
            {
                return JsonSerializer.Deserialize<Dictionary<string, List<Candle>>>(await response.Content.ReadAsStringAsync());
            }


            throw new Exception();
        }

        private string BuildBarsUri(string resolution, int limit, string start, string end, params string[] symbols)
        {
            var path = new StringBuilder(_settings.BarsPath)
               .Append($"{resolution}?");

            if (symbols == null || symbols.Length == 0)
                throw new ArgumentException("At least one symbol must be provided");

            path.Append(BuildCommaParameters("symbols", symbols));

            if (limit != 0)
                path.Append(BuildParameter("limit", limit.ToString()));

            if (!String.IsNullOrWhiteSpace(start))
                path.Append(BuildParameter("start", start));

            if (!String.IsNullOrWhiteSpace(end))
                path.Append(BuildParameter("end", end));

            return path.ToString();
        }
        private string BuildParameter(string parameter, string value, bool first = false) => $"{(first ? "" : "&")}{parameter}={value}";
        private string BuildCommaParameters(string parameter, params string[] values)
        {
            return $"{parameter}={String.Join(',', values)}";
        }
    }
}
