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
        private readonly PolygonClient _polygonClient;

        public PolygonService(PolygonStreamingClient streamingClient, PolygonClient polygonClient)
        {
            this._streamingClient = streamingClient;
            this._polygonClient = polygonClient;
        }

        public void GetAggregateData()
        {

        }
    }
}
