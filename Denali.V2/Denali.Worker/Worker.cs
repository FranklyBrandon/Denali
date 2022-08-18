

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
                var processor = scope.ServiceProvider.GetService<PairTradeStrategy>();
                var barTimeFrame = new Alpaca.Markets.BarTimeFrame(5, Alpaca.Markets.BarTimeFrameUnit.Minute);

                await processor.Initialize("VTI", "VUG", DateTime.Now, "5m", 20, stoppingToken);
            }

            stoppingToken.WaitHandle.WaitOne();         
        }
    }
}