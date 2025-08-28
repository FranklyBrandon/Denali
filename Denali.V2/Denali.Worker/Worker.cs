using Denali.Processors;
using Denali.Processors.VolatileUniverse;
using InteractiveBrokers.Services;
using System.Diagnostics;

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
                var processor = scope.ServiceProvider.GetService<DenaliClimbProcessor>();

                await processor.Process(new(2025, 8, 20), stoppingToken);
                processor.StartTimeScheduledTask.InvokeManual();
                */
                

                var ibService = scope.ServiceProvider.GetService<DenaliClimbIBProcessor>();
                await ibService.Process(new(2025, 8, 20), stoppingToken);
                await ibService.StartTimeScheduledTask.InvokeManual();
                
            }

            stoppingToken.WaitHandle.WaitOne();         
        }
    }
}