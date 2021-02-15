using Denali.Models.Polygon;
using Denali.Services.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;

namespace Denali.Services.Polygon
{
    public class PolygonStreamingClient
    {
        private readonly PolygonSettings _polygonSettings;
        private readonly AuthenticationSettings _authSettings;
        private Action<string> _handleMessageAction;
        private ClientWebSocket _webSocket;
        private readonly ILogger<PolygonStreamingClient> _logger;

        public PolygonStreamingClient(PolygonSettings polygonSettings, AuthenticationSettings authSettings, ILogger<PolygonStreamingClient> logger)
        {  
            this._polygonSettings = polygonSettings;
            this._authSettings = authSettings;
            this._handleMessageAction = HandleAuthFlow;
            this._logger = logger;
        }

        public async Task ConnectToPolygonStreams(CancellationToken token)
        {
            await ConnectToPolygon(token);
            ReceiveAsync(token);
            await AuthenticateSocket(token);
        }

        public async Task ConnectToPolygon(CancellationToken token)
        {
            this._webSocket = new ClientWebSocket();
            await _webSocket.ConnectAsync(new Uri(_polygonSettings.WebsocketUrl), token);
        }

        public async Task AuthenticateSocket(CancellationToken token)
        {
            var request = new PolygonWebsocketRequest
            {
                Action = Models.Polygon.Action.Authorize,
                Params = _authSettings.APIKey
            };

            await Send(token, JsonSerializer.Serialize(request));
        }

        public async Task SubscribeToChannel(Channel channel, CancellationToken token, string ticker)
        {
            var request = new PolygonWebsocketRequest
            {
                Action = Models.Polygon.Action.Subscribe,
                Params = channel.ToString() + "." + ticker
            };

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
            //Fragments must be less than 1024 bytes
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

        private void HandleAuthFlow(string data)
        {
            try
            {
                var responses = JsonSerializer.Deserialize<List<PolygonWebSocketResponse>>(data);
                if (responses.Any(x => x.Status == PolygonWebsocketStatus.Connected))
                {
                    _logger.LogInformation("Connected to Polygon Websocket successfully");
                }
                else if (responses.Any(x => x.Status == PolygonWebsocketStatus.AuthSuccess))
                {
                    _logger.LogInformation("Socket Authentication Successful");
                    _handleMessageAction = DataResponse;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void DataResponse(string data)
        {
            _logger.LogInformation(data);
        }
    }
}
