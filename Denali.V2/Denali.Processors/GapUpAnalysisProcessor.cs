using Alpaca.Markets;
using AutoMapper;
using Denali.Services;
using Denali.Shared.Extensions;
using Denali.TechnicalAnalysis;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public class GapUpAnalysisProcessor : StrategyProcessorBase
    {
        private const decimal MINIMUM_STOCK_PRICE = 10m;
        private const int MARKET_TIME_BUFFER_MINUTES = 9;

        private readonly ILogger<GapUpAnalysisProcessor> _logger;

        public GapUpAnalysisProcessor(AlpacaService alpacaService, IMapper mapper, ILogger<GapUpAnalysisProcessor> logger) : base(alpacaService, mapper)
        {
            _logger = logger;
        }

        public async Task Process(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Initializing clients");
            _alpacaService.InitializeTradingclient();
            _alpacaService.InitializeDataClient();

            DateTime startTime = new(2025, 6, 6);
            _logger.LogInformation($"=== Processing day {startTime.ToShortDateString()} ===");

            var allAssets = await GetAssetUniverse(); // All tickers to scan
            _logger.LogInformation($"Fetching market days");
            var (previousMarketCalenderDay, currentMarketCalenderDay) = await GetMarketDays(startTime); // Trading session data
            _logger.LogInformation($"Fetching asset universe");

            var previousMarketTradingEnd = previousMarketCalenderDay.GetTradingCloseTimeUtc(); // End of previous day's session
            var currentMarketTradingBegin = currentMarketCalenderDay.GetTradingOpenTimeUtc(); // Current day pre-market trading
            var currentMarketTradingEnd = currentMarketCalenderDay.GetTradingCloseTimeUtc();

            _logger.LogInformation($"Fetching aggregate data");
            // Buffer previous market trading end to account for any missing aggregate bars
            var aggregateData = await GetAggregateDataMulti(allAssets, previousMarketTradingEnd.AddMinutes(-MARKET_TIME_BUFFER_MINUTES), currentMarketTradingBegin, BarTimeFrame.Minute); // Aggregate data for tickers

            _logger.LogInformation($"Analyzing price movements");
            // Filter out tickers by minimum price
            var symbols = aggregateData.Where(x => x.Value.Count > 0 && x.Value.Last().Close > MINIMUM_STOCK_PRICE).Select(x => x.Key).ToList();
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
                _logger.LogInformation($"Asset: {change.Key.PadRight(4)}, Change: {change.Value.Round(2)}, Price: {bars.Last().Close}, Bar Count: {bars.Count()}");
            }
        }

        private async Task<(IIntervalCalendar, IIntervalCalendar)> GetMarketDays(DateTime startTime)
        {
            var marketBacklogDays = await GetPastMarketDays(startTime, 4);
            return (marketBacklogDays.ElementAt(marketBacklogDays.Count() - 2), marketBacklogDays.Last());
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
    }
}
