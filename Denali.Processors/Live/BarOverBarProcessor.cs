using Denali.Models.Shared;
using Denali.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public class BarOverBarProcessor : ILiveProcessor
    {
        private Dictionary<string, List<IAggregateData>> _stockData;

        public BarOverBarProcessor()
        {

        }
        public void OnBarReceived(IAggregateData barData)
        {
            throw new NotImplementedException();
        }

        public Task Process(DateTime startTime, IEnumerable<string> tickers, CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }

        public Task ShutDown(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
