using Denali.Models;
using Denali.Models.PythonInterop;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Denali.Services.PythonInterop
{
    public interface IPythonInteropClient
    {
        Task<LinearRegressionResult> GetOLSCalculation(IEnumerable<IAggregateBar> seriesX, IEnumerable<IAggregateBar> seriesY, int backlog);
    }

    public class PythonInteropClient : IPythonInteropClient
    {
        private readonly HttpClient _httpClient;
        private readonly PythonInteropClientSettings _settings;

        public PythonInteropClient(HttpClient httpClient, IOptions<PythonInteropClientSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _httpClient.BaseAddress = new Uri(_settings.BaseAddress);
        }

        public async Task<LinearRegressionResult> GetOLSCalculation(IEnumerable<IAggregateBar> seriesX, IEnumerable<IAggregateBar> seriesY, int backlog)
        {
            var movingXReturns = seriesX.Skip(seriesX.Count() - backlog).Select(x => new { value = x.Close, timeUTC = x.TimeUtc });
            var movingYReturns = seriesY.Skip(seriesY.Count() - backlog).Select(x => new { value = x.Close, timeUTC = x.TimeUtc });
            var olsRequestBody = new { x = movingXReturns, y = movingYReturns };

            var json = JsonSerializer.Serialize(olsRequestBody);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_settings.OLSPath, UriKind.Relative),
                Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json),
            };

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                    return JsonSerializer.Deserialize<LinearRegressionResult>(await response.Content.ReadAsStringAsync());

                throw new HttpRequestException($"Bad response from PythonInteroptClient: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
