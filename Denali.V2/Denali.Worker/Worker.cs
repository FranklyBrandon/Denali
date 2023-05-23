using Alpaca.Markets;
using Denali.Processors;
using Denali.Processors.ElephantStrategy;
using Denali.Processors.GapMomentum;
using Denali.Processors.MartingaleBasis;
using Denali.Processors.StatArb;
using Denali.Services;
using Denali.TradingView;

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
                var processor = scope.ServiceProvider.GetService<MartingaleAverageAnalysisProcessor>();

                await processor.Process("VTI", DateTime.Parse("05/18/2022"), DateTime.Parse("05/18/2023"), stoppingToken);
            }

            stoppingToken.WaitHandle.WaitOne();         
        }
    }
}