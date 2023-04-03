using Alpaca.Markets;
using Denali.Processors;
using Denali.Processors.ElephantStrategy;
using Denali.Processors.GapMomentum;
using Denali.Processors.StatArb;

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
                //var processor = scope.ServiceProvider.GetService<GapMomentumAnalysis>();

                //await processor.Process("VTI", DateTime.Parse("3/24/2023"), DateTime.Parse("3/31/2023"), stoppingToken);

                var processor = scope.ServiceProvider.GetService<LiveTradingProcessor>();
                await processor.Process(stoppingToken, "VTI");
            }

            stoppingToken.WaitHandle.WaitOne();         
        }
    }
}