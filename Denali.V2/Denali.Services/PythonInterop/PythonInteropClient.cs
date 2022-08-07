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
        Task<LinearRegressionResult> GetOLSCalculation(IEnumerable<double> seriesX, IEnumerable<double> seriesY);
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

        public async Task<LinearRegressionResult> GetOLSCalculation(IEnumerable<double> seriesX, IEnumerable<double> seriesY)
        {
            var json = JsonSerializer.Serialize(new
            {
                x = seriesX,
                y = seriesY
            });

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_settings.OLSPath),
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
