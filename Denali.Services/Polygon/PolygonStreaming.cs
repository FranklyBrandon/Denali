using Alpaca.Markets;
using Denali.Models.Polygon;
using Denali.Services.Settings;
using System;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;

namespace Denali.Services.Polygon
{
    public class PolygonStreaming : IDisposable
    {
        private bool disposedValue;

        public PolygonStreamingClient Client;

        public PolygonStreaming(PolygonSettings settings)
        {
            var config = new PolygonStreamingClientConfiguration
            {
                ApiEndpoint = new Uri(settings.WebsocketUrl),
                KeyId = settings.APIKey
            };

            Client = new PolygonStreamingClient(config);
        }

        protected async virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    await Client.DisconnectAsync();
                    Client.Dispose();

                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }


        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
