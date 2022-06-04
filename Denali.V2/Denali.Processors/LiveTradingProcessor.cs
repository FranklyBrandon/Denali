using Alpaca.Markets;
using Denali.Processors.ElephantStrategy;
using Denali.Services;
using Denali.TechnicalAnalysis.ElephantBars;
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
        private readonly ILogger _logger;
        public LiveTradingProcessor(AlpacaService alpacaService, ElephantRideStrategy elephantRideStrategy, ILogger<LiveTradingProcessor> logger)
        {
            _alpacaService = alpacaService;
            _elephantRideStrategy = elephantRideStrategy;
            _logger = logger;
        }

        public async Task Process(CancellationToken stoppingToken, DateTime date)
        {
            _alpacaService.InitializeTradingclient();
            _alpacaService.InitializeDataClient();

            await _elephantRideStrategy.Setup(date.AddDays(-1));

        }
    }
}
