using Denali.Processors;
using Denali.Processors.DenaliClimbStrategy;
using Denali.Processors.DenaliDescentStrategy;

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
                var service = scope.ServiceProvider.GetService<DenaliClimbHistoricProcessor>();
                await  service.ProcessRange(new(2025, 10, 6), new(2025, 10, 6), stoppingToken);
                /*
                var service = scope.ServiceProvider.GetService<DenaliClimbProcessor>();
                await service.Initialize();
                await service.Process(new(2025, 10, 6), stoppingToken);
                await service.StartTimeScheduledTask.InvokeManual();
                */

                /*
                var service = scope.ServiceProvider.GetService<GapUpScreenTest>();
                await service.Initialize();
                await service.Process(new(2025, 10, 6), stoppingToken);
                stoppingToken.WaitHandle.WaitOne();
                */
                var service = scope.ServiceProvider.GetService<TrueFadeStrategy>();
                await service.ProcessRange(new(2025, 10, 14), new(2025, 10, 14), stoppingToken);

            }
        }
    }
}