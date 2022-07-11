using Alpaca.Markets;
using Denali.Models;
using Denali.Processors.Exceptions;
using Denali.Services;
using Denali.TechnicalAnalysis.ElephantBars;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Denali.Processors.ElephantStrategy
{
    public class ElephantRideStrategy
    {
        private readonly AlpacaService _alpacaService;
        private readonly ElephantBarSettings _elephantBarSettings;
        private readonly ILogger<ElephantRideStrategy> _logger;
        private readonly BarAggregator _barAggregator;
        private readonly List<ITrade> _trades;

        private const int BACKLOG_DAYS = 2;
        private const int BACKLOG_MARKET_DAYS = 5;

        public ElephantBars ElephantBars { get; private set; }
        public ElephantRideStrategy(
            AlpacaService alpacaService, 
            IOptions<ElephantBarSettings> elephantBarSettings, 
            BarAggregator barAggregator,
            ILogger<ElephantRideStrategy> logger)
        {
            _alpacaService = alpacaService ?? throw new ArgumentNullException(nameof(alpacaService));
            _elephantBarSettings = elephantBarSettings?.Value ?? throw new ArgumentNullException(nameof(elephantBarSettings));
            _barAggregator = barAggregator ?? throw new ArgumentNullException(nameof(barAggregator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _trades = new List<ITrade>();
        }

        /// <summary>
        /// Requires Trading Client Initialized. Requires Data Client Initialized
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task Setup(DateTime date)
        {
            ElephantBars = new ElephantBars(_elephantBarSettings);
            await _alpacaService.InitializeDataStreamingClient();
            _alpacaService.InitializeTradingclient();
            _alpacaService.InitializeDataClient();

            var marketDays = await _alpacaService.GeOpenMarketDays(BACKLOG_MARKET_DAYS, date);

            // Get backlog market days plus the current date
            var daysThatNeedData = marketDays.Skip(1).Take(BACKLOG_DAYS).Reverse();
            var currentDate = marketDays.First();
            if (currentDate.GetSessionOpenTimeUtc().Day != date.Day)
            {
                _logger.LogInformation($"No trading window detected for day {date.Day}");
                throw new NoTradingWindowException();
            }

            // Populate backlog data
            var backlogBars = new List<IBar>();
            foreach (var day in daysThatNeedData)
            {
                var response = await _alpacaService.AlpacaDataClient.ListHistoricalBarsAsync(
                    new HistoricalBarsRequest("AAPL", day.GetTradingOpenTimeUtc(), day.GetTradingCloseTimeUtc(), new BarTimeFrame(5, BarTimeFrameUnit.Minute)));
                
                if (!string.IsNullOrWhiteSpace(response.NextPageToken))
                    throw new Exception("More than one page");

                backlogBars.AddRange(response.Items);
            }

            // Subscribe to realtime trades and candlestick data
            var tradeSubscription = _alpacaService.AlpacaDataStreamingClient.GetTradeSubscription("AAPL");
            tradeSubscription.Received += OnTradePrice;

            var barSubscription = _alpacaService.AlpacaDataStreamingClient.GetMinuteBarSubscription("AAPL");
            barSubscription.Received += OnMinuteBar;

            await _alpacaService.AlpacaDataStreamingClient.SubscribeAsync(tradeSubscription);
            await _alpacaService.AlpacaDataStreamingClient.SubscribeAsync(barSubscription);

            // Initialize the bar aggregator to calculate the specified intervals
            _barAggregator.SetLastUpdateMinute((int)_barAggregator.Round(DateTime.UtcNow.Minute));
            _barAggregator.OnBar += OnIntervalBar;
        }

        public void OnTradePrice(ITrade trade)
        {
            _trades.Add(trade);
        }

        public void OnMinuteBar(IBar bar)
        {
            _logger.LogInformation($"Minute Bar received: Open: {bar.Open}, High: {bar.High}, Low: {bar.Low}, Close: {bar.Close}");
            _barAggregator.OnMinuteBar(bar);
        }

        public void OnIntervalBar(IAggregateBar bar)
        {
            _logger.LogInformation($"Interval Bar received: Open: {bar.Open}, High: {bar.High}, Low: {bar.Low}, Close: {bar.Close}");
        }
    }
}
