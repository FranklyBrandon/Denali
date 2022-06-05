using Denali.Services.InteractiveBrokers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Services.IB
{
    public class IBService
    {
        private IBClient _ibClient;

        public IBService()
        {

        }

        public async Task Start(CancellationToken stoppingToken) 
        {
            _ibClient = new IBClient();

            int port = 1447;
            int clientId = 127;
            string host = "127.0.0.1";

            await _ibClient.ConnectAndProcess(clientId, host, port, stoppingToken);
        }
    }
}
