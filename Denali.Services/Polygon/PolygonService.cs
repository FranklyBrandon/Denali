using Denali.Models.Polygon;
using Denali.Services.Settings;
using System;
using System.Collections.Generic;
using System.IO;
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

        private readonly PolygonSettings _polygonSettings;
        private readonly AuthenticationSettings _authSettings;
        private ClientWebSocket _webSocket;

        public PolygonService(PolygonSettings polygonSettings, AuthenticationSettings authSettings)
        {
            this._polygonSettings = polygonSettings;
            this._authSettings = authSettings;
        }



        public async Task AuthenticateSocket(CancellationToken token)
        {
            var request = new WebsocketRequest 
            { 
                Action = Models.Polygon.Action.Authorize, 
                Params = _authSettings.APIKey
            };

            var la = JsonSerializer.Serialize(request);

            await Send(token, JsonSerializer.Serialize(request));
        }

        public async Task SubscribeToChannel(Channel channel
            , CancellationToken token)
        {
            var request = new WebsocketRequest
            {
                Action = Models.Polygon.Action.Subscribe,
                Params = channel.ToString()
            };

            await Send(token, JsonSerializer.Serialize(request));
        }

        

        private async Task Send(CancellationToken token, string data)
        {
            await _webSocket.SendAsync(Encoding.UTF8.GetBytes(data), WebSocketMessageType.Text, true, token);
        }

        private void OnMessage(string data)
        {
            MessageReceived(data);
        }
    }
}
