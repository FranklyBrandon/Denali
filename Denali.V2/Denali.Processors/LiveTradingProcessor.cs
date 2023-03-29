using Alpaca.Markets;
using Denali.Processors.ElephantStrategy;
using Denali.Services;
using Denali.Shared;
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
        private readonly ILogger _logger;

        private DateTime _marketOpen;
        public LiveTradingProcessor(AlpacaService alpacaService, ILogger<LiveTradingProcessor> logger)
        {
            _alpacaService = alpacaService;
            _logger = logger;
        }

        public async Task Process(CancellationToken stoppingToken, string symbol)
        {
            _logger.LogInformation("== Iniatializing Alpaca clients ==");
            await _alpacaService.InitializeDataStreamingClient();
            _alpacaService.InitializeTradingclient();

            _logger.LogInformation("== Fetching current calender day ==");
            var calendars = await _alpacaService.AlpacaTradingClient.ListIntervalCalendarAsync(
                CalendarRequest.GetForSingleDay(
                    DateOnly.FromDateTime(
                        TimeUtils.GetNewYorkTime(DateTime.UtcNow)
                    )
                )
            ).ConfigureAwait(false);

            var calenderDay = calendars.Single();
            _marketOpen = calenderDay.GetTradingOpenTimeUtc();

            _logger.LogInformation("== Subscribing to data feeds ==");
            var tradeSubscription = _alpacaService.AlpacaDataStreamingClient.GetTradeSubscription(symbol);
            tradeSubscription.Received += OnTradeReceived;

            await _alpacaService.AlpacaDataStreamingClient.SubscribeAsync(tradeSubscription);          
        }

        public void OnTradeReceived(ITrade trade)
        {
            if (trade.TimestampUtc < _marketOpen)
                _logger.LogInformation($"Pre Market Trade: {trade.Price}");
            else
                _logger.LogInformation($"Trade: {trade.Price}");
        }
    }
}
