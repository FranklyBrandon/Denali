using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Denali.Processors;
using Denali.Services.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Denali.Worker
{
    public class DenaliWorker : BackgroundService
    {
        private readonly ILogger<DenaliWorker> _logger;
        private readonly ServiceProvider _provider;
        private readonly TimeUtils _timeUtils;
        private readonly HashSet<DayOfWeek> _tradingDays;

        public DenaliWorker(ILogger<DenaliWorker> logger, ServiceProvider provider, TimeUtils timeUtils)
        {
            _logger = logger;
            _provider = provider;
            _timeUtils = timeUtils;
            _tradingDays = new HashSet<DayOfWeek> { DayOfWeek.Monday
                , DayOfWeek.Tuesday
                , DayOfWeek.Wednesday
                , DayOfWeek.Thursday
                , DayOfWeek.Friday };

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _provider.CreateScope())
            {
                if (TradingToday())
                {
                    _logger.LogInformation("Starting Denali Worker Process", DateTimeOffset.Now);
                    IProcessor processor = scope.ServiceProvider.GetRequiredService<IProcessor>();
                    await processor.Process(stoppingToken);
                }
                else
                {
                    _logger.LogInformation("No trading window available. Exiting Denali Worker");
                    return;
                }
                //while (!stoppingToken.IsCancellationRequested)
                //{
                //}
            }

        }

        //Replace this with MarketOpen API call and CRON scheudling once we have Polygon access
        private bool TradingToday()
        {
            return true;
            var today = _timeUtils.GetNYSEDateTime();
            return _tradingDays.Contains(today.Date.DayOfWeek)
                    && today.TimeOfDay >= _timeUtils.GetNYSEOpen()
                    && today.TimeOfDay <= _timeUtils.GetNYSEClose();
        }
    }
}
