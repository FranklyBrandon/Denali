using System.Threading;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public interface IProcessor
    {
        void Process(CancellationToken stoppingToken);
    }
}
