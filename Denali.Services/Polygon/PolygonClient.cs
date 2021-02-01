using Denali.Models.Polygon;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Denali.Shared;
using System.Text.Json;
using System.Threading.Tasks;

namespace Denali.Services.Polygon
{
    public class PolygonClient
    {
        private readonly HttpClient _httpClient;
        private readonly PolygonSettings _polygonSettings;

        public PolygonClient(HttpClient httpClient, PolygonSettings polygonSettings)
        {
            this._httpClient = httpClient;
            this._polygonSettings = polygonSettings;
            _httpClient.BaseAddress = new Uri(_polygonSettings.APIUrl);
        }

        public async Task<AggregateResponse> GetAggregateData(string ticker, long fromDate, long toDate, int multiplier = 1, BarTimeSpan timeFrame = BarTimeSpan.Minute, string sort = "asc", bool unadjusted = false, int limit = 120)
        {
            var url = string.Format(_polygonSettings.AggregatePath, ticker, multiplier, timeFrame.ToEnumMemberAttrValue(), fromDate, toDate, sort, unadjusted, limit, _polygonSettings.APIKey);
            var response = await _httpClient.GetAsync(url);

            return JsonSerializer.Deserialize<AggregateResponse>(await response.Content.ReadAsStringAsync());
        }
    }
}
