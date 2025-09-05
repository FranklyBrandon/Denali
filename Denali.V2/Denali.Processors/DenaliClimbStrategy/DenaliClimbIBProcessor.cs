using Alpaca.Markets;
using AutoMapper;
using Denali.Services;
using Denali.Shared.Extensions;
using Denali.Shared.Time;
using InteractiveBrokers.Models.Response;
using InteractiveBrokers.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Denali.Processors.DenaliClimbStrategy
{
    public class DenaliClimbIBProcessor
    {
        public ScheduledTask StartTimeScheduledTask;
        public IDateTimeService DateTimeService;
        public List<IAsset> TradableAssets;

        private readonly DenaliClimbStrategySettings _settings;
        private readonly ILogger<DenaliClimbIBProcessor> _logger;
        private readonly IMapper _mapper;
        private readonly DataLayerComponent _dataLayer;

        public DenaliClimbIBProcessor(DataLayerComponent dataLayer, IDateTimeService dateTimeService, IOptions<DenaliClimbStrategySettings> settings, IMapper mapper, ILogger<DenaliClimbIBProcessor> logger)
        {
            _dataLayer = dataLayer;
            DateTimeService = dateTimeService;
            _settings = settings.Value;
            _mapper = mapper;
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
            TradableAssets = await _dataLayer.GetAllTradableAssets();
            _logger.LogInformation($"Total contracts count: {TradableAssets.Count()}");
            _logger.NewLine();
        }

        public async Task Process(DateTime startDate, CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Processing day {startDate.ToShortDateString()}");
            _logger.NewLine();

            _logger.LogInformation("Fetching previous market days...");
            var marketBacklogDays = await _dataLayer.GetPastMarketDays(startDate, 4);
            var previousMarketDay = marketBacklogDays.ElementAt(marketBacklogDays.Count() - 2);
            var currentMarketDay = marketBacklogDays.Last();

            _logger.LogInformation($"Previous market day: [Date: {previousMarketDay.GetSessionOpenTimeUtc().ToShortDateString()}, Close time (UTC): {previousMarketDay.GetTradingCloseTimeUtc().ToString("HH:mm")}]");
            _logger.LogInformation($"Current market day : [Date: {currentMarketDay.GetSessionOpenTimeUtc().ToShortDateString()}, Open time (UTC): {currentMarketDay.GetTradingOpenTimeUtc().ToString("HH:mm")}]");

            // Schedule time for market open + buffer minutes
            var startTime = currentMarketDay.GetTradingOpenTimeUtc().AddMinutes(_settings.AfterMarketOpenBufferMinutes);
            _logger.LogInformation($"Scheduling start time for (UTC) {startTime.ToString("HH:mm")}");
            _logger.NewLine();
            StartTimeScheduledTask = new ScheduledTask(
                startTime,
                () => OnStartTime(startTime, previousMarketDay, currentMarketDay, TradableAssets)
            );
        }

        public async Task OnStartTime(DateTime startTime, IIntervalCalendar previousMarketDay, IIntervalCalendar currentMarketDay, List<IAsset> assets)
        {
            _logger.LogInformation($"Processing go time!");
            var screener = new GapUpScreener(_dataLayer, _settings, _logger);
            var gapUpStocks = await screener.GetGapUpStocks(previousMarketDay, currentMarketDay, assets);
            var streamer = new GapUpStreamer(_dataLayer, _settings, DateTimeService, _logger);
            await streamer.SubscribeToTickerStream(gapUpStocks, startTime, currentMarketDay.GetTradingOpenTimeUtc());
        }
    }
}