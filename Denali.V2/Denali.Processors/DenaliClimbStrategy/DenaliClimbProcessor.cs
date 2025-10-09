using Alpaca.Markets;
using Denali.Models;
using Denali.Services;
using Denali.Services.Extensions;
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

        public delegate Task OnEntry(DenaliClimbEntrySignal entrySignal);
        public OnEntry OnEntryAction { get; set; }

        private readonly DataLayerComponent _dataLayer;
        private readonly GapUpScreener _screener;
        private readonly DenaliClimbStrategySettings _settings;
        private readonly ILogger<DenaliClimbProcessor> _logger;

        public DenaliClimbProcessor(
            DataLayerComponent dataLayer, 
            GapUpScreener screener, 
            IOptions<DenaliClimbStrategySettings> settings, 
            ILogger<DenaliClimbProcessor> logger)
        {
            _dataLayer = dataLayer;
            _screener = screener;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task Initialize()
        {
            _logger.LogInformation("Initializing Denali Climb Strategy");
            _logger.LogInformation(@$"Running with settings:
                   AfterMarketOpenStartTimeMinutes: {_settings.AfterMarketOpenStartTimeMinutes}
                   MinimumStockPrice: {_settings.MinimumStockPrice}
                   MinimumGapUpPercentage: {_settings.MinimumGapUpPercentage}"
            );

            _logger.NewLine();
            await _dataLayer.Initialize();
            _logger.NewLine();

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
            var currentMarketDay = marketBacklogDays.Last();

            _logger.LogInformation($"Previous market day: [Date: {previousMarketDay.GetSessionOpenTimeUtc().ToShortDateString()}, Close time (UTC): {previousMarketDay.GetTradingCloseTimeUtc().ToString("HH:mm")}]");
            _logger.LogInformation($"Current market day : [Date: {currentMarketDay.GetSessionOpenTimeUtc().ToShortDateString()}, Open time (UTC): {currentMarketDay.GetTradingOpenTimeUtc().ToString("HH:mm")}]");

            // Schedule time for market open + buffer minutes
            var startTime = currentMarketDay.GetTradingOpenTimeUtc().AddMinutes(_settings.AfterMarketOpenStartTimeMinutes);
            _logger.LogInformation($"Scheduling start time for (UTC) {startTime.ToString("HH:mm")}");
            StartTimeScheduledTask = new ScheduledTask(
                startTime,
                () => OnScreenStart(startTime, previousMarketDay, currentMarketDay, AllTradableAssets)
            );
            _logger.LogInformation($"Time scheduled: {StartTimeScheduledTask.IsScheduled}");
            _logger.NewLine();
        }

        public async Task OnScreenStart(DateTime startTime, IIntervalCalendar previousMarketDay, IIntervalCalendar currentMarketDay, List<IAsset> assets)
        {
            _logger.LogInformation($"Processing go time {startTime.ToString("yyyy-MM-dd HH:mm")} (UTC)");
            var screenedAssets = await _screener.GetGapUpStocks(previousMarketDay, currentMarketDay, assets, _settings.MinimumStockPrice, 3m);
            var openingData = await _dataLayer.GetAggregateDataMulti(screenedAssets.Select(x => x.Key), currentMarketDay.GetTradingOpenTimeUtc(), currentMarketDay.GetTradingOpenTimeUtc().AddMinutes(30), new BarTimeFrame(15, BarTimeFrameUnit.Minute));
            openingData = openingData.OrderByDescending(x => screenedAssets[x.Key]).ToDictionary(x => x.Key, x => x.Value);
            foreach (var ticker in openingData)
            {
                var gapUpData = screenedAssets[ticker.Key];
                await OnEntryAction(new DenaliClimbEntrySignal
                {
                    SignalBar = ticker.Value[0],
                    GapUpPercentage = gapUpData
                });

            }
        }
    }
}