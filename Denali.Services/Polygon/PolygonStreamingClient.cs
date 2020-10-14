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
    public class PolygonStreamingClient
    {
        private readonly PolygonSettings _polygonSettings;
        private readonly AuthenticationSettings _authSettings;
        private readonly Action<string> _handleMessageAction;
        private ClientWebSocket _webSocket;

        public PolygonStreamingClient(PolygonSettings polygonSettings, AuthenticationSettings authSettings)
        {
            this._polygonSettings = polygonSettings;
            this._authSettings = authSettings;
        }

        public async Task ConnectToPolygonStreams(CancellationToken token)
        {
            await ConnectToPolygon(token);
            await AuthenticateSocket(token);
        }

        public async Task ConnectToPolygon(CancellationToken token)
        {
            this._webSocket = new ClientWebSocket();
            await _webSocket.ConnectAsync(new Uri(_polygonSettings.WebsocketUrl), token);
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

        private async Task Send(CancellationToken token, string data)
        {
            await _webSocket.SendAsync(Encoding.UTF8.GetBytes(data), WebSocketMessageType.Text, true, token);
        }

        /// <summary>
        /// https://thecodegarden.net/websocket-client-dotnet
        /// </summary>
        /// <param name="token"></param>
        public async void ReceiveAsync(CancellationToken token)
        {
            var buffer = new ArraySegment<byte>(new byte[1024]);
            do
            {
                WebSocketReceiveResult result;
                using (var ms = new MemoryStream())
                {
                    do
                    {
                        result = await _webSocket.ReceiveAsync(buffer, token);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    } while (!result.EndOfMessage);

                    //Break out of inner loop to attempt a reconnect
                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    ms.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                        _handleMessageAction(await reader.ReadToEndAsync());
                }
            } while (!token.IsCancellationRequested);
        }
    }
}
