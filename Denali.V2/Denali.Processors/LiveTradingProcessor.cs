using Alpaca.Markets;
using Denali.Processors.ElephantStrategy;
using Denali.Services;
using Denali.TechnicalAnalysis.ElephantBars;
using InteractiveBrokers.API;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public class LiveTradingProcessor
    {
        private readonly AlpacaService _alpacaService;
        private readonly ElephantRideStrategy _elephantRideStrategy;
        private readonly IBService _ibService;
        private readonly ILogger _logger;
        public LiveTradingProcessor(IBService ibService, ILogger<LiveTradingProcessor> logger)
        {
            _ibService = ibService;
            _logger = logger;
        }

        public async Task Process(CancellationToken stoppingToken, DateTime date)
        {
            await _ibService.Start(stoppingToken);



        }
    }
}
