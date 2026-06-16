using Alpaca.Markets;
using Denali.Processors.GapUpFadeStrategy;
using Denali.Services;
using Denali.Shared.Extensions;
using Denali.TechnicalAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Denali.Processors.DenaliClimbStrategy
{
    public class GapUpScreener
    {
        private readonly DataLayerComponent _dataLayer;
        private readonly DenaliClimbStrategySettings _settings;
        private readonly ILogger _logger;

        public GapUpScreener(DataLayerComponent dataLayer, IOptions<DenaliClimbStrategySettings> settings, ILogger<GapUpScreener> logger)
        {
            _dataLayer = dataLayer;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<Dictionary<string, decimal>> GetGapUpStocks(
            IIntervalCalendar previousMarketDay, 
            IIntervalCalendar currentMarketDay, 
            List<IAsset> assets,
            decimal minimumstockPrice,
            decimal minimumPercentage,
            bool descending = true)
        {
            // End of previous day's session, buffer previous market trading end to account for any missing aggregate bars
            var previousMarketTradingEnd = previousMarketDay.GetTradingCloseTimeUtc();
            var currentMarketTradingBegin = currentMarketDay.GetTradingOpenTimeUtc();

            var tickers = assets.Select(x => x.Symbol);
            Dictionary<string, List<IBar>> aggregateData = new Dictionary<string, List<IBar>>();
            foreach (var batch in tickers.Chunk(5000))
            {
                var data = await _dataLayer.GetAggregateDataMulti(
                    batch,
                    previousMarketTradingEnd,
                    currentMarketTradingBegin,
                    new BarTimeFrame(15, BarTimeFrameUnit.Minute)
                );

                aggregateData = aggregateData.Concat(data).ToDictionary();
            }
            // Filter out tickers by minimum price
            var symbols = aggregateData.Where(x => x.Value.Count > 0 && x.Value.Last().Close > minimumstockPrice).Select(x => x.Key).ToList();
            Dictionary<string, decimal> changePercentage = new();

            foreach (var symbol in symbols)
            {
                var data = aggregateData[symbol];
                var previousBar = data.Where(x => x.TimeUtc <= previousMarketTradingEnd).LastOrDefault();
                var currentBar = data.Where(x => x.TimeUtc >= currentMarketTradingBegin).LastOrDefault();
                if (previousBar != null && currentBar != null)
                {
                    changePercentage[symbol] = ChangePercentage.Calculate(previousBar.Close, currentBar.Close).RoundToMoney();
                }
            }

            // Filter by unrealsitic change percentage (janky way to account for reverse splits). Then order
            var filtered = changePercentage.Where(x => x.Value >= minimumPercentage && x.Value <= 200);

            return descending
                ? filtered.OrderByDescending(x => x.Value).ToDictionary()
                : filtered.OrderBy(x => x.Value).ToDictionary();
        }

        public async Task<GapUpResults> GetGapUpBetween(
            DateTime startTime,
            DateTime endTime,
            List<IAsset> assets,
            decimal minimumstockPrice,
            decimal minimumGapUpPercentage,
            BarTimeFrame barTimeFrame,
            bool descending = true)
        {
            var tickers = assets.Select(x => x.Symbol);
            Dictionary<string, List<IBar>> aggregateData = new Dictionary<string, List<IBar>>();
            foreach (var batch in tickers.Chunk(5000))
            {
                var data = await _dataLayer.GetAggregateDataMulti(
                    batch,
                    startTime,
                    endTime,
                    barTimeFrame
                ).ConfigureAwait(false);

                aggregateData = aggregateData.Concat(data).ToDictionary();
            }
            // Filter out tickers by minimum price
            var symbols = aggregateData.Where(x => x.Value.Count > 0 && x.Value.Last().Close > minimumstockPrice).Select(x => x.Key).ToList();
            Dictionary<string, decimal> changePercentage = new();

            foreach (var symbol in symbols)
            {
                var data = aggregateData[symbol];
                var previousBar = data.FirstOrDefault(x => x.TimeUtc >= startTime);
                var currentBar = data.LastOrDefault(x => x.TimeUtc <= endTime && x.TimeUtc != previousBar?.TimeUtc);
                if (previousBar != null && currentBar != null)
                {
                    changePercentage[symbol] = ChangePercentage.Calculate(previousBar.Close, currentBar.Close).RoundToMoney();
                }
            }

            // Filter by unrealsitic change percentage (janky way to account for reverse splits). Then order
            var filtered = changePercentage.Where(x => x.Value >= minimumGapUpPercentage && x.Value <= 200);

            var orderedGapUps = descending
                ? filtered.OrderByDescending(x => x.Value).ToDictionary()
                : filtered.OrderBy(x => x.Value).ToDictionary();

            return new GapUpResults(aggregateData, orderedGapUps);
        }


        public async Task<IEnumerable<GapUpFadeSignal>> GetGapUp(
            DateTime startTime,
            DateTime endTime,
            List<string> tickers,
            decimal minimumstockPrice,
            decimal minimumGapUpPercentage,
            decimal maximumPrice = 500,
            bool descending = true)
        {
            Dictionary<string, List<IBar>> aggregateData = new Dictionary<string, List<IBar>>();
            foreach (var batch in tickers.Chunk(5000))
            {
                var data = await _dataLayer.GetAggregateDataMulti(
                    batch,
                    startTime,
                    endTime,
                    BarTimeFrame.Minute
                ).ConfigureAwait(false);

                aggregateData = aggregateData.Concat(data).ToDictionary();
            }
            // Filter out tickers by minimum price
            var symbols = aggregateData.Where(x => x.Value.Count > 0 && x.Value.Last().Close > minimumstockPrice).Select(x => x.Key).ToList();
            List<GapUpFadeSignal> signals = new();

            foreach (var symbol in symbols)
            {
                var data = aggregateData[symbol];
                var previousBar = data.FirstOrDefault(x => x.TimeUtc == startTime);
                var currentBar = data.LastOrDefault(x => x.TimeUtc.Date == endTime.Date);
                if (previousBar != null && currentBar != null)
                {
                    var changePercentage = ChangePercentage.Calculate(previousBar.Close, currentBar.Close).RoundToMoney();
                    var signal = new GapUpFadeSignal { Symbol = symbol, GapUpPercentage = changePercentage, LastPrice = currentBar.Close };
                    signals.Add(signal);
                }
            }

            var filtered = signals.Where(x => x.GapUpPercentage >= minimumGapUpPercentage && x.LastPrice <= maximumPrice);

            return descending
                ? filtered.OrderByDescending(x => x.GapUpPercentage).ToList()
                : filtered.OrderBy(x => x.GapUpPercentage).ToList();
        }
    }
}

public record GapUpResults(Dictionary<string, List<IBar>> AggregateData, Dictionary<string, decimal> ChangePercentage);
