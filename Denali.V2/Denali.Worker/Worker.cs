using Denali.Processors.DenaliClimbStrategy;

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
                var service = scope.ServiceProvider.GetService<DenaliClimbProcessor>();
                await service.Initialize();
                await service.Process(new(2025, 9, 5), stoppingToken);
                await service.StartTimeScheduledTask.InvokeManual();
                */

                var historicService = scope.ServiceProvider.GetService<DenaliClimbHistoricProcessor>();
                await historicService.ProcessRange(new(2025, 9, 8), new(2025, 9, 12), stoppingToken);
                
            }

            stoppingToken.WaitHandle.WaitOne();         
        }
    }
}