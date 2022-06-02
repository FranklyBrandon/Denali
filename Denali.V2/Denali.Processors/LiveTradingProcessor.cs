using Alpaca.Markets;
using Denali.Services;
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
        public LiveTradingProcessor(AlpacaService alpacaService, ILogger<LiveTradingProcessor> logger)
        {
            _alpacaService = alpacaService;
            _logger = logger;
        }

        public async Task Process(CancellationToken stoppingToken)
        {
            _logger.LogInformation("===== Starting Live Trading for Ticker AAPL =====");

            _logger.LogInformation("Loading Data Backlog");
            await _alpacaService.InitializeTradingclient();
            // Check the last 5 days (to account for weekends and holidys)
            var lastMarketDate = await GeOpenMarketDays(5);
            // Use the last two days for market data
            var bracketDates = lastMarketDate.Take(2);
            var startDate = bracketDates.Last();
            var endDate = bracketDates.First();

            await _alpacaService.InitializeDataClient();
            var backlogData = await _alpacaService.AlpacaDataClient.ListHistoricalBarsAsync(
                new HistoricalBarsRequest("AAPL", startDate.TradingOpenTimeUtc, endDate.TradingCloseTimeUtc, new BarTimeFrame(5, BarTimeFrameUnit.Minute)));

        }

        private async Task<IEnumerable<ICalendar>> GeOpenMarketDays(int pastDays)
        {
            var today = DateTime.Now;
            var calenders = await _alpacaService.AlpacaTradingClient.ListCalendarAsync((new CalendarRequest().SetInclusiveTimeInterval(today.AddDays(-pastDays), today)));
            return calenders.OrderByDescending(x => x.TradingDateEst).Take(pastDays);
        }
    }
}
