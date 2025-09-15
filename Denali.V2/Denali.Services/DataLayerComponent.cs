using Alpaca.Markets;
using Alpaca.Markets.Extensions;
using Denali.Shared.Extensions;
using InteractiveBrokers.Models.Response;
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
            //await _interactiveBrokersService.InitializeHttpAuth();
            //_logger.NewLine();
            await _alpacaService.InittializeTradingClientAuth();
            _logger.NewLine();
            _logger.LogInformation("=== COMPLETED DATA LAYER INITIALIZA?TION ===");
        }

        public void InitializeDataClient() => _alpacaService.InitializeDataClient();

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

        public async Task<IDisposableAlpacaDataSubscription<IBar>> SubscribeMinuteBar(IEnumerable<string> symbols) =>
            await _alpacaService.AlpacaDataStreamingClient.SubscribeMinuteBarAsync(symbols);
    }
}
