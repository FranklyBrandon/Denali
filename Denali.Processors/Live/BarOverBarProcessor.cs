using Denali.Models.Shared;
using Denali.Services.Alpaca;
using Denali.Shared.Utility;
using Denali.Strategies;
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
        private IEnumerable<string> _subscribedTickers;
        private Dictionary<string, List<IAggregateStrategy>> _strategies;

        public BarOverBarProcessor()
        {

        }
        public void OnBarReceived(IAggregateData barData)
        {
            _strategies[barData.Ticker].ForEach(x => x.ProcessTick(null, null));
        }

        public Task Process(DateTime startTime, IEnumerable<string> tickers, CancellationToken stoppingToken)
        {
            // Get backlog data
            // Get today data
            // Subcribe to live streaming
            throw new NotImplementedException();
        }

        public Task ShutDown(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
