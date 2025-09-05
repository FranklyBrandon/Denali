using Denali.Processors.DenaliClimbStrategy;
using Denali.Processors.VolatileUniverse;
using Denali.Services;
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

                //var startDate = new DateTime(2025, 8, 28, 13, 30, 0);
                //var dateTimeService = new MockDateTimeService();
                //dateTimeService.SetDateTime(startDate.AddMinutes(CONSTANTS.AFTER_OPEN_BUFFER_MINUTES));

                var ibService = scope.ServiceProvider.GetService<DenaliClimbIBProcessor>();
                //ibService.DateTimeService = dateTimeService;

                await ibService.Initialize();
                await ibService.Process(new(2025, 9, 5), stoppingToken);
                await ibService.StartTimeScheduledTask.InvokeManual();
                //await ibService.Process(new(2025, 9, 3), stoppingToken);
                //await ibService.StartTimeScheduledTask.InvokeManual();
                
            }

            stoppingToken.WaitHandle.WaitOne();         
        }
    }
}