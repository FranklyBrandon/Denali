using System.Net.WebSockets;
using System.Text;

namespace Denali.TradingView
{
    public class TextWebSocket : IDisposable
    {
        private ClientWebSocket _clientSocket;
        private readonly int _bufferSize;
        private readonly StringBuilder _message;

        public event Action<string> OnMessage;

        public TextWebSocket(int bufferSize = 256)
        {
            _bufferSize = bufferSize;
            _message = new StringBuilder();
        }
        public async Task Connect(Uri url, IDictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            if (_clientSocket is null)
                _clientSocket = new ClientWebSocket();

            if (headers is not null)
            {
                foreach (var header in headers)
                    _clientSocket.Options.SetRequestHeader(header.Key, header.Value);
            }

            await _clientSocket.ConnectAsync(url, cancellationToken);
            Task.Run(() => BeginReceive(cancellationToken), cancellationToken);
        }

        private async Task BeginReceive(CancellationToken cancellationToken = default)
        {
            byte[] buffer = new byte[_bufferSize];
            while (_clientSocket.State == WebSocketState.Open)
            {
                var result = await _clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                if (result.MessageType == WebSocketMessageType.Close)
                    await _clientSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken);
                else
                    HandleMessage(buffer, result);
            }
        }

        private void HandleMessage(byte[] buffer, WebSocketReceiveResult? result)
        {
            _message.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));

            if (result.EndOfMessage)
                OnEndOfMessage();
        }

        public async Task SendAsync(string message, CancellationToken cancellationToken = default)
        {
            var encoded = Encoding.UTF8.GetBytes(message);
            var buffer = new ArraySegment<Byte>(encoded, 0, encoded.Length);
            await _clientSocket.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken);
            //Console.ForegroundColor = ConsoleColor.Red;
            //Console.WriteLine("Sent:");
            //Console.ForegroundColor = ConsoleColor.White;
            //Console.WriteLine(message);
        }

        private void OnEndOfMessage()
        {
            OnMessage.Invoke(_message.ToString());
            _message.Clear();
        }

        public void Dispose()
        {
            _clientSocket.Dispose();
        }
    }
}