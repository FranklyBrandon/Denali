using Denali.Processors.DenaliClimbStrategy;
using Denali.Processors.DenaliDescentStrategy;
using Denali.Processors.TrueFadeStrategy;

namespace Denali.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ServiceProvider _provider;

        public Worker(ILogger<Worker> logger, ServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
        } 

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _provider.CreateScope())
            {
                /*
                var service = scope.ServiceProvider.GetService<TrueFadeStrategy>();
                await service.ProcessRange(new(2025, 10, 20), new(2025, 10, 20), stoppingToken);
                */

                var service = scope.ServiceProvider.GetService<TrueFadeHistoricProcessor>();
                await service.ProcessRange(new(2025, 1, 2), new(2025, 10, 20));
            }
        }
    }
}