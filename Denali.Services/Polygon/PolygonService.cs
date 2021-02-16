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
        public delegate void MessageHandler(string message);
        public event MessageHandler MessageReceived;

        private readonly PolygonStreaming _streamingClient;
        private readonly PolygonClient _polygonClient;

        public PolygonService(PolygonStreaming streamingClient, PolygonClient polygonClient)
        {
            this._streamingClient = streamingClient;
            this._polygonClient = polygonClient;
        }

        public async Task<AggregateResponse> GetAggregateData(string ticker, int multiplier, BarTimeSpan timeSpan, long from, long to, int limit)
        {
            return await _polygonClient.GetAggregateData(ticker, from, to, multiplier: multiplier, timeFrame: timeSpan, limit: limit);
        }
    }
}
