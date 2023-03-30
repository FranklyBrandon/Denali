

using Alpaca.Markets;
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
            using (var scope = _provider.CreateScope())
            {
                var processor = scope.ServiceProvider.GetService<LiveTradingProcessor>();
                var barTimeFrame = new BarTimeFrame(5, BarTimeFrameUnit.Minute);

                await processor.Process(stoppingToken, "SPY");
            }

            stoppingToken.WaitHandle.WaitOne();         
        }
    }
}