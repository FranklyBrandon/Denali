using Alpaca.Markets;
using Denali.Models.Polygon;
using Denali.Services.Settings;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Denali.Services.Polygon
{
    public class PolygonService
    {
        private readonly PolygonSettings _settings;
        public IPolygonStreamingClient StreamingClient;
        public IPolygonDataClient DataClient;

        public PolygonService(PolygonSettings settings)
        {
            this._settings = settings;
        }

        public void InitializeStreamingClient()
        {
            if (StreamingClient != null)
            {
                StreamingClient.Dispose();
                StreamingClient = null;
            }

            var config = new PolygonStreamingClientConfiguration
            {
                ApiEndpoint = new Uri(_settings.WebsocketUrl),
                KeyId = _settings.APIKey
            };

            StreamingClient = new PolygonStreamingClient(config);
        }

        public void InitializeDataClient()
        {
            if (DataClient != null)
            {
                DataClient.Dispose();
                DataClient = null;
            }

            var config = new PolygonDataClientConfiguration
            {
                ApiEndpoint = new Uri(_settings.APIUrl),
                KeyId = _settings.APIKey
            };

            DataClient = new PolygonDataClient(config);
        }

        public async Task DisconnectStreamingClient()
        {
            await StreamingClient.DisconnectAsync();
            StreamingClient.Dispose();
            StreamingClient = null;
        }

        //public async Task<AggregateResponse> GetAggregateData(string ticker, int multiplier, BarTimeSpan timeSpan, long from, long to, int limit)
        //{
        //    return await _polygonClient.GetAggregateData(ticker, from, to, multiplier: multiplier, timeFrame: timeSpan, limit: limit);
        //}
    }
}
