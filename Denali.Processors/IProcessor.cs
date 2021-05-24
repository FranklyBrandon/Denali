using Denali.Models.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public interface IProcessor
    {
        Task Process(DateTime startTime, CancellationToken stoppingToken);
        Task ShutDown(CancellationToken stoppingToken);
        Task OnBarReceived(IAggregateData barData);
    }
}
