using System.Net.WebSockets;
using System.Text;

namespace Denali.TradingView
{
    public class TradingViewWebSocket : IDisposable
    {
        private ClientWebSocket _clientSocket;
        private readonly int _bufferSize;
        private readonly Uri _url;
        private readonly StringBuilder _message;

        public event Action<string> OnMessage;

        public TradingViewWebSocket(Uri url, int bufferSize = 256)
        {
            _url = url;
            _bufferSize = bufferSize;
            _message = new StringBuilder();
        }
        public async Task Connect(IDictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            if (_clientSocket is null)
                _clientSocket = new ClientWebSocket();

            if (headers is not null)
            {
                foreach (var header in headers)
                    _clientSocket.Options.SetRequestHeader(header.Key, header.Value);
            }

            await _clientSocket.ConnectAsync(_url, cancellationToken);
            await Task.Run(() => BeginReceive(cancellationToken), cancellationToken);
        }

        public async Task BeginReceive(CancellationToken cancellationToken = default)
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

        private void OnEndOfMessage()
        {
            // TODO: Automatically send acknowledgment messages
            OnMessage.Invoke(_message.ToString());
            _message.Clear();
        }

        public void Dispose()
        {
            _clientSocket.Dispose();
        }
    }
}