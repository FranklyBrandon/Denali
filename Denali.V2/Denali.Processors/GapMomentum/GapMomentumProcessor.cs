using Alpaca.Markets;
using AutoMapper;
using Denali.Processors.Exceptions;
using Denali.Services;
using Denali.Shared.Time;
using Microsoft.Extensions.Logging;

namespace Denali.Processors.GapMomentum
{
    public class GapMomentumProcessor : StrategyProcessorBase
    {
        private string _preMarketSymbol;
        private string _tradSymbol;

        private ScheduledTask _scheduledTask;

        private readonly ILogger _logger;
        private const int BACKLOG_MARKET_DAYS = 5;

        public GapMomentumProcessor(AlpacaService alpacaService, 
            IMapper mapper, 
            ILogger<GapMomentumProcessor> logger
        ) : base(alpacaService, mapper)
        {
            _logger = logger;
        }

        public async Task Process(string preMarketSymbol, string tradeSymbol, CancellationToken stoppingToken)
        {
            _preMarketSymbol = preMarketSymbol;
            _tradSymbol = tradeSymbol;

            var backlogMarketDays = await InitializeProcessor();
            var previousDayAggregate = await GetPreviousTradingDayAggregate(backlogMarketDays);
            ScheduleBeforeMarketOpensTask(backlogMarketDays.Last().GetTradingOpenTimeUtc(), previousDayAggregate);
        }

        private async Task<IEnumerable<IIntervalCalendar>> InitializeProcessor()
        {
            _logger.LogInformation("====> Initializing Alpaca clients");
            await _alpacaService.InitializeDataStreamingClient();
            _alpacaService.InitializeTradingclient();
            _alpacaService.InitializeDataClient();
            _logger.LogInformation("====> Alpaca Initializing Complete");

            _logger.LogInformation("====> Fetching current market day");
            var today = TimeUtils.GetNewYorkTime(DateTime.UtcNow);
            var calendars = await GetPastMarketDays(BACKLOG_MARKET_DAYS, today).ConfigureAwait(false);

            if (calendars?.LastOrDefault()?.GetSessionOpenTimeUtc().Day != today.Day)
            {
                _logger.LogInformation($"No trading window detected for day {today.Day}");
                throw new NoTradingWindowException();
            }

            return calendars;
        }

        private async Task<IBar> GetPreviousTradingDayAggregate(IEnumerable<IIntervalCalendar> backlogMarketDays)
        {
            _logger.LogInformation("====> Fetching backlog data");
            // The last day in the backlog list is the current market day
            var from = backlogMarketDays.Reverse().Skip(2).FirstOrDefault();
            var to = backlogMarketDays.Reverse().Skip(1).FirstOrDefault();

            var previousTradingDayAggregate = await _alpacaService.AlpacaDataClient.ListHistoricalBarsAsync(
                new HistoricalBarsRequest(
                    _preMarketSymbol,
                    from.GetTradingCloseTimeUtc(),
                    to.GetTradingCloseTimeUtc(), 
                    BarTimeFrame.Day
                )
            ).ConfigureAwait(false);

            return previousTradingDayAggregate.Items.Single();
        }

        private void ScheduleBeforeMarketOpensTask(DateTime utcMarketOpen, IBar previousDayAggregate)
        {
            _scheduledTask = new ScheduledTask(utcMarketOpen.AddMinutes(-1), async (alertTime) =>
            {
                _logger.LogInformation("====> Pre Market Checks");
                _logger.LogInformation("====> Fetching Latest Quote");

                // TODO: Is this delayed in free data plan? Why is this value wrong!?
                var latestTrade = await _alpacaService.AlpacaDataClient.
                    GetLatestTradeAsync(new LatestMarketDataRequest(_preMarketSymbol))
                    .ConfigureAwait(false);

                _logger.LogInformation($"Latest Trade: {latestTrade.Price}");

                // Determine if Gap
                // Place trades if necessary
            });
        }
        private async Task SubscribeToTrades(string symbol)
        {
            _logger.LogInformation("== Subscribing to data feeds ==");
            var tradeSubscription = _alpacaService.AlpacaDataStreamingClient.GetTradeSubscription(symbol);
            //tradeSubscription.Received += OnTradeReceived;

            await _alpacaService.AlpacaDataStreamingClient.SubscribeAsync(tradeSubscription);
        }

    }
}
