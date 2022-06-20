using IBApi;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace InteractiveBrokers.API
{
    public class IBService
    {
        private readonly IBClient _ibClient;
        private readonly ILogger<IBService> _logger;

        #region Events
        public event EventHandler ConnectionAcknowledged;
        public event EventHandler ConnectionClosed;
        #endregion

        public IBService(ILogger<IBService> logger, ILogger<IBClient> ibClientLogger)
        {
            _logger = logger;
            _ibClient = new IBClient(ibClientLogger);
        }

        public async Task Start(CancellationToken stoppingToken)
        {
            int port = 7497;
            int clientId = 127;
            string host = "127.0.0.1";

            await _ibClient.ConnectAndProcess(clientId, host, port, stoppingToken);
        }

        public async Task GetHistoricalData(int requestId, Contract contract, DateTime endDate, CancellationToken cancellationToken = default)
        {
            _ibClient.ClientSocket.reqHistoricalData(requestId, contract, endDate.ToString("yyyyMMdd HH:mm:ss"), "1 D", "1 min", "MIDPOINT", 1, 1, false, new List<TagValue>());
        }

        public void OnConnectionClose() =>
            ConnectionClosed.Invoke(this, EventArgs.Empty);

        public void OnConnectionAcknowledged() =>
            ConnectionAcknowledged.Invoke(this, EventArgs.Empty);

    }
}
