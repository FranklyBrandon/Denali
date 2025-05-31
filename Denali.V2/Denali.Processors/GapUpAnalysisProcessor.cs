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
        private const decimal MINIMUM_STOCK_PRICE = 2m;
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

            DateTime startTime = new(2025, 5, 30);
            _logger.LogInformation($"=== Processing day {startTime.ToShortDateString()} ===");

            _logger.LogInformation($"Fetching market days");
            var (previousMarketCalenderDay, currentMarketCalenderDay) = await GetMarketDays(startTime); // Trading session data
            _logger.LogInformation($"Fetching asset universe");
            var allAssets = await GetAssetUniverse(); // All tickers to scan
                                                      // Aggregate data for tickers

            var previousMarketSessionEnd = previousMarketCalenderDay.GetTradingCloseTimeUtc(); // End of previous day's session
            var currentMarketSessionBegin = currentMarketCalenderDay.GetTradingOpenTimeUtc(); // Current day pre-market trading

            _logger.LogInformation($"Fetching aggregate data");
            // Buffer previous market trading end to account for any missing aggregate bars
            var aggregateData = await GetAggregateDataMulti(allAssets, previousMarketSessionEnd.AddMinutes(-MARKET_TIME_BUFFER_MINUTES), currentMarketSessionBegin, BarTimeFrame.Minute);

            _logger.LogInformation($"Analyzing price movements");
            // Filter out tickers by minimum price
            var symbols = aggregateData.Where(x => x.Value.Count > 0 && x.Value.Last().Close > MINIMUM_STOCK_PRICE).Select(x => x.Key).ToList();
            Dictionary<string, decimal> changePercentage = new();

            foreach (var symbol in symbols)
            {
                var previousBar = aggregateData[symbol].Where(x => x.TimeUtc <= previousMarketSessionEnd).LastOrDefault();
                var currentBar = aggregateData[symbol].Where(x => x.TimeUtc < currentMarketSessionBegin).LastOrDefault();
                if (previousBar != null && currentBar != null) 
                {
                    changePercentage[symbol] = ChangePercentage.Calculate(previousBar.Close, currentBar.Close);
                }
            }

            // Filter by unrealsitic change percentage (janky way to account for reverse splits). Then order
            var orderedChanges = changePercentage.Where(x => x.Value <= 200).OrderByDescending(x => x.Value).ToList();
            var topAsset = orderedChanges.First();
            _logger.LogInformation($"Top asset is {topAsset.Key} with {topAsset.Value.Round(2)}% change");
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
