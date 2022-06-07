using Denali.Services.InteractiveBrokers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Services.IB
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
            int port = 1447;
            int clientId = 127;
            string host = "127.0.0.1";

            await _ibClient.ConnectAndProcess(clientId, host, port, stoppingToken);
        }

        public void OnConnectionClose() =>
            ConnectionClosed.Invoke(this, EventArgs.Empty);

        public void OnConnectionAcknowledged() =>
            ConnectionAcknowledged.Invoke(this, EventArgs.Empty);

    }
}
