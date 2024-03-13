using Denali.Processors;
using Denali.Processors.VolatileUniverse;

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
                var processor = scope.ServiceProvider.GetService<VolatileUniverseAnalysisProcessor>();

                await processor.Process(DateTime.Parse("03/11/2024"), stoppingToken);
            }

            stoppingToken.WaitHandle.WaitOne();         
        }
    }
}