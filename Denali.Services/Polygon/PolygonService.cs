using Alpaca.Markets;
using System;
using System.Threading.Tasks;

namespace Denali.Services.Polygon
{
    public class PolygonService
    {
        public IPolygonStreamingClient StreamingClient { get; private set; }
        public IPolygonDataClient DataClient { get; private set; }

        private readonly PolygonSettings _settings;

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
    }
}
