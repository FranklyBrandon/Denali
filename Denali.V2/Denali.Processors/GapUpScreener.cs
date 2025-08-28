using Alpaca.Markets;
using Denali.Services;
using Denali.TechnicalAnalysis;
using InteractiveBrokers.Models.Response;
using Microsoft.Extensions.Logging;
using Denali.Shared.Extensions;

namespace Denali.Processors
{
    public class GapUpScreener
    {
        private readonly AlpacaService _alpacaService;
        private readonly ILogger _logger;

        public GapUpScreener(AlpacaService alpacaService, ILogger logger)
        {
            _alpacaService = alpacaService;
            _logger = logger;
        }

        public async Task<List<Contract>> GetGapUpStocks(IIntervalCalendar previousMarketDay, IIntervalCalendar currentMarketDay, List<Contract> contracts)
        {
            // End of previous day's session, buffer previous market trading end to account for any missing aggregate bars
            var previousMarketTradingEnd = previousMarketDay.GetTradingCloseTimeUtc().AddMinutes(-CONSTANTS.MARKET_TIME_BUFFER_MINUTES);
            var currentMarketTradingBegin = currentMarketDay.GetTradingOpenTimeUtc();

            _logger.LogInformation($"Fetching aggregate data from Alpaca...");
            var tickers = contracts.Select(x => x.ticker);
            Dictionary<string, List<IBar>> aggregateData = new Dictionary<string, List<IBar>>();
            foreach (var batch in tickers.Chunk(5000))
            {
                var data = await _alpacaService.GetAggregateDataMulti(
                    batch,
                    previousMarketTradingEnd,
                    currentMarketTradingBegin,
                    BarTimeFrame.Minute
                );

                aggregateData = aggregateData.Concat(data).ToDictionary();
            }

            _logger.LogInformation($"Data found for {aggregateData.Count} tickers out of {contracts.Count} contracts");

            _logger.LogInformation($"Analyzing price movements...");
            // Filter out tickers by minimum price
            var symbols = aggregateData.Where(x => x.Value.Count > 0 && x.Value.Last().Close > CONSTANTS.MINIMUM_STOCK_PRICE).Select(x => x.Key).ToList();
            Dictionary<string, decimal> changePercentage = new();

            foreach (var symbol in symbols)
            {
                var data = aggregateData[symbol];
                var previousBar = data.Where(x => x.TimeUtc <= previousMarketTradingEnd).LastOrDefault();
                var currentBar = data.Where(x => x.TimeUtc >= currentMarketTradingBegin).LastOrDefault();
                if (previousBar != null && currentBar != null)
                {
                    changePercentage[symbol] = ChangePercentage.Calculate(previousBar.Close, currentBar.Close);
                }
            }

            // Filter by unrealsitic change percentage (janky way to account for reverse splits). Then order
            var orderedChanges = changePercentage.Where(x => x.Value <= 200).OrderByDescending(x => x.Value).Take(20).ToList();
            foreach (var change in orderedChanges)
            {
                var data = aggregateData[change.Key];
                _logger.LogInformation($"Asset: {change.Key.PadRight(4)}, Change: {change.Value.Round(2)}, Price: {data.Last().Close}, Bar Count: {data.Count()}, Volume: {data.Sum(x => x.Volume)}");
            }

            var orderedChangesTickers = orderedChanges.Select(x => x.Key).ToList();
            return contracts.Where(x => orderedChangesTickers.Contains(x.ticker)).ToList();
        }
    }
}
