using Alpaca.Markets;
using Alpaca.Markets.Extensions;
using Denali.Models.Alpaca;
using Microsoft.Extensions.Logging;

namespace Denali.Services
{
    public class DataLayerComponent
    {
        private readonly AlpacaService _alpacaService;
        private readonly ILogger _logger;

        public DataLayerComponent(AlpacaService alpacaService, ILogger<DataLayerComponent> logger)
        {
            _alpacaService = alpacaService;
            _logger = logger;
        }

        public async Task Initialize()
        {
            _logger.LogInformation("=== INITIALIZING DATA LAYER COMPONENTS ===");
            await _alpacaService.InittializeTradingClientAuth();
            _logger.LogInformation("=== COMPLETED DATA LAYER INITIALIZATION ===");
        }

        public async Task<List<IAsset>> GetAllTradableAssets()
        {
            var NyseAssetRequest = new AssetsRequest
            {
                Exchange = Exchange.Nyse,
                AssetClass = AssetClass.UsEquity,
                AssetStatus = AssetStatus.Active,
            };
            var NasdaqAssetRequest = new AssetsRequest
            {
                Exchange = Exchange.Nasdaq,
                AssetClass = AssetClass.UsEquity,
                AssetStatus = AssetStatus.Active
            };

            var nyseAssets = await _alpacaService.AlpacaTradingClient.ListAssetsAsync(NyseAssetRequest);
            var nasdaqAssets = await _alpacaService.AlpacaTradingClient.ListAssetsAsync(NasdaqAssetRequest);
            return nyseAssets.Select(x => x)
                .Concat(nasdaqAssets.Select(x => x))
                .Where(x => x.IsTradable)
                .Distinct()
                .ToList();
        }

        public async Task<IEnumerable<IIntervalCalendar>> GetMarketDays(DateTime from, DateTime into)
        {
            var calenders = await _alpacaService.AlpacaTradingClient.ListIntervalCalendarAsync(
                new CalendarRequest().WithInterval(
                    new Interval<DateTime>(from, into)
                )
            );
            return calenders.OrderBy(x => x.GetTradingDate());
        }

        public async Task<Dictionary<string, List<IQuote>>> GetQuotes(IEnumerable<string> symbols, DateTime from, DateTime into)
        {
            string? pageToken = default;
            Dictionary<string, List<IQuote>> quotes = new Dictionary<string, List<IQuote>>();

            do
            {
                var request = new HistoricalQuotesRequest(
                    symbols, 
                    from, 
                    into
                ).WithPageSize(10000);

                if (!string.IsNullOrWhiteSpace(pageToken))
                    request.WithPageToken(pageToken);

                var response = await _alpacaService.AlpacaDataClient.GetHistoricalQuotesAsync(request).ConfigureAwait(false);
                pageToken = response.NextPageToken;

                foreach (var symbolData in response.Items)
                {
                    if (quotes.ContainsKey(symbolData.Key))
                    {
                        var newData = quotes[symbolData.Key];
                        newData.AddRange(symbolData.Value);
                        quotes[symbolData.Key] = newData;
                    }
                    else
                        quotes[symbolData.Key] = symbolData.Value.ToList();
                }

            } while (!string.IsNullOrWhiteSpace(pageToken));

            return quotes;
        }

        public async Task<IEnumerable<IIntervalCalendar>> GetPastMarketDays(DateTime day, int pastDays = 0)
        {
            var calenders = await _alpacaService.AlpacaTradingClient.ListIntervalCalendarAsync(
                new CalendarRequest().WithInterval(
                    new Interval<DateTime>(day.AddDays(-pastDays), day)
                )
            );
            return calenders.OrderBy(x => x.GetTradingDate());
        }

        public async Task<Dictionary<string, List<IBar>>> GetAggregateDataMulti(IEnumerable<string> symbols, DateTime startTime, DateTime endTime, BarTimeFrame timeFrame)
        {
            string? pageToken = default;
            Dictionary<string, List<IBar>> bars = new Dictionary<string, List<IBar>>();

            do
            {
                var request = new HistoricalBarsRequest(
                        symbols,
                        startTime,
                        endTime,
                        timeFrame
                ).WithPageSize(10000);

                if (!string.IsNullOrWhiteSpace(pageToken))
                    request.WithPageToken(pageToken);

                var response = await _alpacaService.AlpacaDataClient.GetHistoricalBarsAsync(request).ConfigureAwait(false);
                pageToken = response.NextPageToken;
                foreach (var symbolData in response.Items)
                {
                    if (bars.ContainsKey(symbolData.Key))
                    {
                        var newData = bars[symbolData.Key];
                        newData.AddRange(symbolData.Value);
                        bars[symbolData.Key] = newData;
                    }
                    else
                        bars[symbolData.Key] = symbolData.Value.ToList();
                }

            } while (!string.IsNullOrWhiteSpace(pageToken));

            return bars;
        }

        public async Task<Dictionary<string, List<ITrade>>> GetTrades(IEnumerable<string> symbols, DateTime startTime, DateTime endTime)
        {
            string? pageToken = default;
            Dictionary<string, List<ITrade>> trades = new Dictionary<string, List<ITrade>>();

            do
            {
                var request = new HistoricalTradesRequest(symbols, startTime, endTime).WithPageSize(10000);

                if (!string.IsNullOrWhiteSpace(pageToken))
                    request.WithPageToken(pageToken);

                var response = await _alpacaService.AlpacaDataClient.GetHistoricalTradesAsync(request).ConfigureAwait(false);
                pageToken = response.NextPageToken;
                foreach (var item in response.Items)
                {
                    if (trades.ContainsKey(item.Key))
                    {
                        var newData = trades[item.Key];
                        newData.AddRange(item.Value);
                        trades[item.Key] = newData;
                    }
                    else
                        trades[item.Key] = item.Value.ToList();
                }
            } while (!string.IsNullOrWhiteSpace(pageToken));
            return trades;
        }

        public async Task<IDisposableAlpacaDataSubscription<IBar>> SubscribeMinuteBar(IEnumerable<string> symbols) =>
            await _alpacaService.AlpacaDataStreamingClient.SubscribeMinuteBarAsync(symbols);
    }
}
