using Denali.Models.Polygon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace Denali.Services.Polygon
{
    public class PolygonService
    {
        public delegate void MessageHandler(string message);
        public event MessageHandler MessageReceived;

        private readonly PolygonSettings _settings;
        private readonly ClientWebSocket _webSocket;

        public PolygonService(PolygonSettings settings)
        {
            this._settings = settings;
        }

        public async void ConnectToPolygon(CancellationToken token)
        {
            await _webSocket.ConnectAsync(new Uri(_settings.WebsocketUrl), token);
        }

        public async void AuthenticateSocket()
        {
            var request = new WebsocketRequest { Action = Models.Polygon.Action.Authenticate, Params = "API key" };
        }

        public async void SubscribeToChannel(Channel channel)
        {
            
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
                        OnMessage(await reader.ReadToEndAsync());
                }
            } while (!token.IsCancellationRequested);
        }

        private async void Send(CancellationToken token, string data)
        {
            await _webSocket.SendAsync(Encoding.UTF8.GetBytes(data), WebSocketMessageType.Text, true, token);
        }

        private void OnMessage(string data)
        {
            MessageReceived(data);
        }
    }
}
