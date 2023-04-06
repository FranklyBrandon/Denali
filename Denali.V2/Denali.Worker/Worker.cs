using Alpaca.Markets;
using Denali.Processors;
using Denali.Processors.ElephantStrategy;
using Denali.Processors.GapMomentum;
using Denali.Processors.StatArb;
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
                //var processor = scope.ServiceProvider.GetService<GapMomentumAnalysis>();

                //await processor.Process("VTI", DateTime.Parse("12/09/2022"), DateTime.Parse("12/16/2022"), stoppingToken);

                //var processor = scope.ServiceProvider.GetService<LiveTradingProcessor>();
                //await processor.Process(stoppingToken, "SPY");
                var laa = TradingViewMessages.ChartResolveSymbol("SESSIONID", "AMEX", "SPY");

                var la = new TradingViewService(new TradingViewSettings());
                await la.ConnectToTradingView();
            }

            stoppingToken.WaitHandle.WaitOne();         
        }
    }
}