using Denali.Models.Polygon;
using Denali.Services.Settings;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Denali.Services.Polygon
{
    public class PolygonService
    {
        public delegate void MessageHandler(string message);
        public event MessageHandler MessageReceived;

        private readonly PolygonStreamingClient _streamingClient;

        public PolygonService(PolygonStreamingClient streamingClient)
        {
            this._streamingClient = streamingClient;
        }
    }
}
