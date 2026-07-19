using Denali.Processors;
using Denali.Processors.DenaliClimbStrategy;
using Denali.Processors.DenaliDescentStrategy;
using Denali.Processors.GapUpFadeStrategy;
using Denali.Processors.PreMarketFadeStrategy;
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
                var service = scope.ServiceProvider.GetService<PreMarketGainers>();
                await service.Process(new (2026, 7, 16), new(2026, 7, 17), stoppingToken);
                */
                var service = scope.ServiceProvider.GetService<PreMarketFadeLiveProcessor>();
                await service.Process();



            }
        }
    }
}