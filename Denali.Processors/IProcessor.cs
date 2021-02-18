using System.Threading;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public interface IProcessor
    {
        Task Process(CancellationToken stoppingToken);
        Task ShutDown(CancellationToken stoppingToken);
    }
}
