using Denali.Models.Shared;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public interface IProcessor
    {
        Task Process(DateTime startTime, IEnumerable<string> tickers, CancellationToken stoppingToken);
    }
    public interface ILiveProcessor : IProcessor
    {
        Task ShutDown(CancellationToken stoppingToken);
        void OnBarReceived(IAggregateData barData);
    }

    public interface IAnalysisProcessor : IProcessor
    {

    }
}
