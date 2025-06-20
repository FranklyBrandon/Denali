using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Denali.Processors;
using Denali.Shared.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Denali.Worker
{
    public class DenaliWorker : BackgroundService
    {
        private readonly ILogger<DenaliWorker> _logger;
        private readonly ServiceProvider _provider;
        private CancellationTokenSource _proccessTokenSource;

        public DenaliWorker(ILogger<DenaliWorker> logger, ServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
            _proccessTokenSource = new CancellationTokenSource();
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _provider.CreateScope())
            {
                _logger.LogInformation("Starting Denali Worker Process", DateTimeOffset.Now);
                var processor = scope.ServiceProvider.GetRequiredService<IAnalysisProcessor>();
                await processor.Process(DateTime.UtcNow.Date, null, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _proccessTokenSource.Cancel();
        }
    }
}
