using Alpaca.Markets;
using Denali.Models;
using Denali.Processors.DenaliClimbStrategy;
using Denali.Services;
using Denali.Shared.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Denali.Processors.DenaliDescentStrategy
{
    public class DenaliDescentProcessor
    {
        public List<IAsset> AllTradableAssets;
        public delegate Task OnEntry(IEnumerable<DenaliDescentEntrySignal> entrySignal);
        public OnEntry OnEntryAction { get; set; }

        private readonly DataLayerComponent _dataLayer;
        private readonly GapUpScreener _screener;
        private readonly DenaliDescentStrategySettings _settings;
        private readonly ILogger<DenaliDescentProcessor> _logger;

        public DenaliDescentProcessor(DataLayerComponent dataLayer, GapUpScreener screener, IOptions<DenaliDescentStrategySettings> settings, ILogger<DenaliDescentProcessor> logger)
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

        public async Task OnScreenStart(DateTime startTime, IIntervalCalendar previousMarketDay, IIntervalCalendar currentMarketDay, List<IAsset> assets)
        {
            _logger.LogInformation($"Processing go time {startTime.ToString("yyyy-MM-dd HH:mm")} (UTC)");

           
           var screenedAssets = (await _screener.GetGapUpBetween(
                previousMarketDay.GetTradingCloseTimeUtc(), 
                currentMarketDay.GetTradingOpenTimeUtc().AddMinutes(-3), 
                assets, 
                _settings.MinimumStockPrice,
                _settings.MinimumGapUpPercentage,
                BarTimeFrame.Minute,
                descending: true)).ChangePercentage;
            
            
            /*
            var screenedAssets = await _screener.GetGapUpStocks(
                previousMarketDay,
                currentMarketDay,
                assets,
                _settings.MinimumStockPrice,
                _settings.MinimumGapUpPercentage,
                descending: true);
            */

            await OnEntryAction(screenedAssets.Select(x => new DenaliDescentEntrySignal(x.Key, x.Value)));
        }

        public async Task ExecuteSignals(IEnumerable<DenaliDescentEntrySignal> signals)
        {

        }
    }
}
