

using Denali.Processors;
using Denali.Processors.ElephantStrategy;
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
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _provider.CreateScope())
                {
                    var processor = scope.ServiceProvider.GetService<PairAnalysis>();
                    var startTime = new DateTime(2022, 7, 25);
                    var endTime = new DateTime(2022, 8, 12);
                    var barTimeFrame = new Alpaca.Markets.BarTimeFrame(5, Alpaca.Markets.BarTimeFrameUnit.Minute);

                    await processor.Process("VTI", "VOO", startTime, endTime, barTimeFrame, stoppingToken);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}