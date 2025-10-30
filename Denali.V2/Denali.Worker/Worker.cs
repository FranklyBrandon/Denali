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
                var service = scope.ServiceProvider.GetService<TrueFadeIBProcessor>();
                await service.Process(DateTime.UtcNow);
                */



                var service = scope.ServiceProvider.GetService<TrueFadeIBHistoricProcessor>();
                await service.ProcessRange(new(2024, 10, 29), new(2025, 10, 29));
            }
        }
    }
}