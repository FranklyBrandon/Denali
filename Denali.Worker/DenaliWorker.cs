using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            var today = _timeUtils.GetNYSEDateTime();
            if (_tradingDays.Contains(today.Date.DayOfWeek)
                && today.TimeOfDay >= _timeUtils.GetNYSEOpen()
                && today.TimeOfDay <= _timeUtils.GetNYSEClose())
            {
                //Fetch Tickers and should trade from Google drive
                //Enter process loop
                _logger.LogInformation("Starting Denali Worker Process", DateTimeOffset.Now);
            }
            else
            {
                _logger.LogInformation("No trading window available. Exiting Denali Worker");
            }
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //}
        }
    }
}
