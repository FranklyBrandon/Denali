using Alpaca.Markets;
using AutoMapper;
using Denali.Services;
using Denali.Shared.Extensions;
using Denali.Shared.Time;
using InteractiveBrokers.Models.Response;
using InteractiveBrokers.Services;
using Microsoft.Extensions.Logging;

namespace Denali.Processors
{
    public class DenaliClimbIBProcessor : StrategyProcessorBase
    {
        public ScheduledTask StartTimeScheduledTask;

        private readonly IInteractiveBrokersService _interactiveBrokersService;
        private readonly ILogger<DenaliClimbIBProcessor> _logger;

        public DenaliClimbIBProcessor(AlpacaService alpacaService, IInteractiveBrokersService interactiveBrokersService, IMapper mapper, ILogger<DenaliClimbIBProcessor> logger) : base(alpacaService, mapper)
        {
            _interactiveBrokersService = interactiveBrokersService;
            _logger = logger;
        }

        public async Task Process(DateTime startDate, CancellationToken stoppingToken)
        {
            await _interactiveBrokersService.InitializeHttpAuth();
            _logger.NewLine();
            await _alpacaService.InittializeTradingClientAuth();
            _logger.NewLine();

            _logger.LogInformation($"Processing day {startDate.ToShortDateString()}");
            _logger.NewLine();

            _logger.LogInformation("Fetching Asset Universe...");
            var contracts = await GetMergedContracts();
            _logger.LogInformation($"Total contracts count: {contracts.Count()}");
            _logger.NewLine();

            _logger.LogInformation("Fetching previous market days...");
            var marketBacklogDays = await GetPastMarketDays(startDate, 4);
            var previousMarketDay = marketBacklogDays.ElementAt(marketBacklogDays.Count() - 2);
            var currentMarketDay = marketBacklogDays.Last();

            _logger.LogInformation($"Previous market day: [Date: {previousMarketDay.GetSessionOpenTimeUtc().ToShortDateString()}, Close time (UTC): {previousMarketDay.GetTradingCloseTimeUtc().ToString("HH:mm")}]");
            _logger.LogInformation($"Current market day : [Date: {currentMarketDay.GetSessionOpenTimeUtc().ToShortDateString()}, Open time (UTC): {currentMarketDay.GetTradingOpenTimeUtc().ToString("HH:mm")}]");

            // Schedule time for market open + buffer minutes
            var startTime = currentMarketDay.GetSessionOpenTimeUtc().AddMinutes(CONSTANTS.AFTER_OPEN_BUFFER_MINUTES);
            StartTimeScheduledTask = new ScheduledTask(
                startTime,
                () => OnStartTime(startTime, previousMarketDay, currentMarketDay, contracts)
            );
        }

        public async Task OnStartTime(DateTime startTime, IIntervalCalendar previousMarketDay, IIntervalCalendar currentMarketDay, List<Contract> contracts)
        {
            var screener = new GapUpScreener(_alpacaService, _logger);
            var gapUpStocks = await screener.GetGapUpStocks(previousMarketDay, currentMarketDay, contracts);
        }

        public void OnSubscribe(List<Contract> contracts)
        {

        }

        private async Task<List<Contract>> GetMergedContracts()
        {
            var mergedContracts = new List<Contract>();
            var ibContracts = await _interactiveBrokersService.GetContractsByExchanges("NYSE", "NASDAQ");

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
            var allAlpacaContracts = nyseAssets.Select(x => x.Symbol)
                .Concat(nasdaqAssets.Select(x => x.Symbol))
                .Distinct()
                .ToList();

            return ibContracts.Join(allAlpacaContracts, ib => ib.ticker, a => a, (ib, a) => ib).ToList();
        }
    }
}
