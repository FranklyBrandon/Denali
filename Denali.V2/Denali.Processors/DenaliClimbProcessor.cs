using Alpaca.Markets;
using AutoMapper;
using Denali.Services;
using Denali.Shared.Time;
using Denali.Shared.Extensions;
using Denali.TechnicalAnalysis;
using Microsoft.Extensions.Logging;
using Alpaca.Markets.Extensions;
using Denali.Models;

namespace Denali.Processors
{
    public static class CONSTANTS
    {
        public const int AFTER_OPEN_BUFFER_MINUTES = 30;
        public const int MARKET_TIME_BUFFER_MINUTES = 9;
        public const decimal MINIMUM_STOCK_PRICE = 10m;
        public const int SLOW_EMA_BACKLOG = 8;
        public const int FAST_EMA_BACKLOG = 3;
    }

    public class DenaliClimbProcessor : StrategyProcessorBase
    {
        public ScheduledTask StartTimeScheduledTask;
        public Dictionary<string, List<IAggregateBar>> StreamedData;
        public Dictionary<string, ExponentialMovingAverage> SlowEMA;
        public Dictionary<string, ExponentialMovingAverage> FastEMA;

        private readonly ILogger<DenaliClimbProcessor> _logger;

        private IList<string> _assets;
        private IIntervalCalendar _previousMarketCalenderDay;
        private IIntervalCalendar _currentMarketCalenderDay;

        public DenaliClimbProcessor(AlpacaService alpacaService, IMapper mapper, ILogger<DenaliClimbProcessor> logger) : base(alpacaService, mapper)
        {
            _logger = logger; 
        }

        public async Task Process(DateTime startDate, CancellationToken stoppingToken)
        {
            _logger.LogInformation("Initializing clients");
            _alpacaService.InitializeTradingclient();
            _alpacaService.InitializeDataClient();
            _logger.LogInformation("Clients initialized");

            _logger.LogInformation($"Processing day {startDate.ToShortDateString()}");

            _logger.LogInformation("Fetching Asset Universe");
            _assets = await GetAssetUniverse();
            _logger.LogInformation($"Total asset count: {_assets.Count}");

            _logger.LogInformation("Fetching previous market days");
            (_previousMarketCalenderDay, _currentMarketCalenderDay) = await GetMarketDays(startDate);
            _logger.LogInformation($"Previous market day: {_previousMarketCalenderDay.GetSessionOpenTimeUtc().ToShortDateString()}");
            _logger.LogInformation($"Current market day : {_currentMarketCalenderDay.GetSessionOpenTimeUtc().ToShortDateString()}");

            // Schedule time for market open + buffer minutes
            StartTimeScheduledTask = new ScheduledTask(
                _currentMarketCalenderDay.GetSessionOpenTimeUtc().AddMinutes(CONSTANTS.AFTER_OPEN_BUFFER_MINUTES), 
                () => OnStartTime(_currentMarketCalenderDay.GetSessionOpenTimeUtc().AddMinutes(CONSTANTS.AFTER_OPEN_BUFFER_MINUTES))
            );
        }

        private async Task OnStartTime(DateTime startTime)
        {
            // End of previous day's session, buffer previous market trading end to account for any missing aggregate bars
            var previousMarketTradingEnd = _previousMarketCalenderDay.GetTradingCloseTimeUtc().AddMinutes(-CONSTANTS.MARKET_TIME_BUFFER_MINUTES);
            var currentMarketTradingBegin = _currentMarketCalenderDay.GetTradingOpenTimeUtc(); // TODO: Make this StartTime with better Alpaca sub

            _logger.LogInformation($"Fetching aggregate data");
            var aggregateData = await GetAggregateDataMulti(
                _assets,
                previousMarketTradingEnd,
                currentMarketTradingBegin, 
                BarTimeFrame.Minute
            );

            _logger.LogInformation($"Analyzing price movements");
            // Filter out tickers by minimum price
            var symbols = aggregateData.Where(x => x.Value.Count > 0 && x.Value.Last().Close > CONSTANTS.MINIMUM_STOCK_PRICE).Select(x => x.Key).ToList();
            Dictionary<string, decimal> changePercentage = new();

            foreach (var symbol in symbols)
            {
                var previousBar = aggregateData[symbol].Where(x => x.TimeUtc <= previousMarketTradingEnd).LastOrDefault();
                var currentBar = aggregateData[symbol].Where(x => x.TimeUtc < currentMarketTradingBegin).LastOrDefault();
                if (previousBar != null && currentBar != null)
                {
                    changePercentage[symbol] = ChangePercentage.Calculate(previousBar.Close, currentBar.Close);
                }
            }

            // Filter by unrealsitic change percentage (janky way to account for reverse splits). Then order
            var orderedChanges = changePercentage.Where(x => x.Value <= 200).OrderByDescending(x => x.Value).Take(20).ToList();
            foreach (var change in orderedChanges)
            {
                var bars = aggregateData[change.Key];
                _logger.LogInformation($"Asset: {change.Key.PadRight(4)}, Change: {change.Value.Round(2)}, Price: {bars.Last().Close}, Bar Count: {bars.Count()}, Volume: {bars.Sum(x => x.Volume)}");
            }
        }

        private async Task<IList<string>> GetAssetUniverse()
        {
            var NyseAssetRequest = new AssetsRequest
            {
                Exchange = Exchange.Nyse,
                AssetClass = AssetClass.UsEquity,
                AssetStatus = AssetStatus.Active
            };
            var NasdaqAssetRequest = new AssetsRequest
            {
                Exchange = Exchange.Nasdaq,
                AssetClass = AssetClass.UsEquity,
                AssetStatus = AssetStatus.Active
            };

            var nyseAssets = await _alpacaService.AlpacaTradingClient.ListAssetsAsync(NyseAssetRequest);
            var nasdaqAssets = await _alpacaService.AlpacaTradingClient.ListAssetsAsync(NasdaqAssetRequest);
            var allAssets = nyseAssets.Select(x => x.Symbol)
                .Concat(nasdaqAssets.Select(x => x.Symbol))
                .Distinct()
                .ToList();

            return allAssets;
        }

        private async Task<(IIntervalCalendar, IIntervalCalendar)> GetMarketDays(DateTime startTime)
        {
            var marketBacklogDays = await GetPastMarketDays(startTime, 4);
            return (marketBacklogDays.ElementAt(marketBacklogDays.Count() - 2), marketBacklogDays.Last());
        }

        private async Task SubscribeToStreams(IEnumerable<string> assets)
        {
            foreach (var asset in assets)
            {
                StreamedData[asset] = new List<IAggregateBar>();
                SlowEMA[asset] = new ExponentialMovingAverage(CONSTANTS.SLOW_EMA_BACKLOG);
                FastEMA[asset] = new ExponentialMovingAverage(CONSTANTS.FAST_EMA_BACKLOG);
            }
            var subscription = await _alpacaService.AlpacaDataStreamingClient.SubscribeMinuteBarAsync(assets);
            subscription.Received += OnStreamedData;
        }

        private void OnStreamedData(IBar bar)
        {
            var aggregate = _mapper.Map<IAggregateBar>(bar);
            StreamedData[aggregate.Symbol].Append(aggregate);

            var aggregates = StreamedData[aggregate.Symbol];
            SlowEMA[aggregate.Symbol].Analyze(aggregates);
            FastEMA[aggregate.Symbol].Analyze(aggregates);
        }
    }
}
