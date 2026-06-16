using Denali.Processors;
using Denali.Processors.DenaliClimbStrategy;
using Denali.Processors.DenaliDescentStrategy;
using Denali.Processors.GapUpFadeStrategy;
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

                /*
                var service = scope.ServiceProvider.GetService<TrueFadeIBHistoricProcessor>();
                await service.ProcessRange(new(2024, 1, 1), new(2025, 11, 14));
                */

                var service = scope.ServiceProvider.GetService<PreMarketHours>();
                await service.Process(new(2026, 5, 1), new(2026, 6, 15), stoppingToken);


                /*
                var service = scope.ServiceProvider.GetService<GapUpFadeHistoricProcessor>();
                await service.ProcessRange(new(2025, 1, 1), new(2025, 10, 31));
                */
            }
        }
    }
}