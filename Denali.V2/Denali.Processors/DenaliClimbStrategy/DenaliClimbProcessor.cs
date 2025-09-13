using Alpaca.Markets;
using Denali.Services;
using Denali.Shared.Extensions;
using Denali.Shared.Time;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Denali.Processors.DenaliClimbStrategy
{
    public class DenaliClimbProcessor
    {
        public ScheduledTask StartTimeScheduledTask;
        public List<IAsset> AllTradableAssets;
        public List<IAsset> ScreenedAssets;
        public IIntervalCalendar CurrentMarketDay; 

        private readonly DataLayerComponent _dataLayer;
        private readonly GapUpScreener _screener;
        private readonly GapUpStreamer _streamer;
        private readonly TradeManager _tradeManager;
        private readonly DenaliClimbStrategySettings _settings;
        private readonly ILogger<DenaliClimbProcessor> _logger;

        public DenaliClimbProcessor(
            DataLayerComponent dataLayer, 
            GapUpScreener screener, 
            GapUpStreamer streamer, 
            TradeManager tradeManager,
            IOptions<DenaliClimbStrategySettings> settings, 
            ILogger<DenaliClimbProcessor> logger)
        {
            _dataLayer = dataLayer;
            _screener = screener;
            _streamer = streamer;
            _streamer.OnEntryAction = OnEntry;
            _tradeManager = tradeManager;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task Initialize()
        {
            _logger.LogInformation("Initializing Denali Climb Strategy");
            _logger.LogInformation(@$"Running with settings:
                   AfterMarketOpenBufferMinutes: {_settings.AfterMarketOpenBufferMinutes}
                   PreMarketBufferMinutes: {_settings.PreMarketBufferMinutes}
                   MinimumStockPrice: {_settings.MinimumStockPrice}
                   SlowEMABacklog: {_settings.SlowEMABacklog}
                   FastEMABacklog: {_settings.FastEMABacklog}"
            );

            await _dataLayer.Initialize();

            _logger.LogInformation("Fetching Asset Universe...");
            AllTradableAssets = await _dataLayer.GetAllTradableAssets();
            _logger.LogInformation($"Total contracts count: {AllTradableAssets.Count()}");
            _logger.NewLine();
        }

        public async Task Process(DateTime date, CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Processing day {date.ToShortDateString()}");
            _logger.NewLine();

            _logger.LogInformation("Fetching previous market days...");
            var marketBacklogDays = await _dataLayer.GetPastMarketDays(date, 4);
            var previousMarketDay = marketBacklogDays.ElementAt(marketBacklogDays.Count() - 2);
            CurrentMarketDay = marketBacklogDays.Last();

            _logger.LogInformation($"Previous market day: [Date: {previousMarketDay.GetSessionOpenTimeUtc().ToShortDateString()}, Close time (UTC): {previousMarketDay.GetTradingCloseTimeUtc().ToString("HH:mm")}]");
            _logger.LogInformation($"Current market day : [Date: {CurrentMarketDay.GetSessionOpenTimeUtc().ToShortDateString()}, Open time (UTC): {CurrentMarketDay.GetTradingOpenTimeUtc().ToString("HH:mm")}]");

            // Schedule time for market open + buffer minutes
            var startTime = CurrentMarketDay.GetTradingOpenTimeUtc().AddMinutes(_settings.AfterMarketOpenBufferMinutes);
            _logger.LogInformation($"Scheduling start time for (UTC) {startTime.ToString("HH:mm")}");
            StartTimeScheduledTask = new ScheduledTask(
                startTime,
                () => OnStartTime(startTime, previousMarketDay, CurrentMarketDay, AllTradableAssets)
            );
            _logger.LogInformation($"Time scheduled: {StartTimeScheduledTask.IsScheduled}");
            _logger.NewLine();
        }

        private async Task OnStartTime(DateTime startTime, IIntervalCalendar previousMarketDay, IIntervalCalendar currentMarketDay, List<IAsset> assets)
        {
            _logger.LogInformation($"Processing go time!");
            ScreenedAssets = await _screener.GetGapUpStocks(previousMarketDay, currentMarketDay, assets);
            await _streamer.SubscribeToTickerStream(ScreenedAssets, startTime, currentMarketDay.GetTradingOpenTimeUtc());
        }

        public async Task OnEntry(EntrySignal entrySignal)
        {
            _logger.LogInformation($"Entry signal for {entrySignal.Bar.Symbol} at {entrySignal.Bar.TimeUtc}");
            await _tradeManager.ProcessEntry(entrySignal.StopLoss, entrySignal.TakeProfit, entrySignal.Bar.Symbol);
        }
    }
}