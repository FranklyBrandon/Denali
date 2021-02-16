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

        public DenaliWorker(ILogger<DenaliWorker> logger, ServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _provider.CreateScope())
            {
                _logger.LogInformation("Starting Denali Worker Process", DateTimeOffset.Now);
                IProcessor processor = scope.ServiceProvider.GetRequiredService<IProcessor>();
                await processor.Process(stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {

        }
    }
}
