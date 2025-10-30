using Alpaca.Markets;
using Denali.Services;
using Denali.Shared.Extensions;
using InteractiveBrokers.Models.Response;
using InteractiveBrokers.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors.TrueFadeStrategy
{
    public class TrueFadeIBProcessor
    {
        private readonly DataLayerComponent _dataLayer;
        private readonly IInteractiveBrokersService _ibService;
        private readonly TrueFadeScreener _screener;
        private readonly TrueFadeStrategySettings _settings;
        private readonly ILogger _logger;

        public TrueFadeIBProcessor(DataLayerComponent dataLayer, IInteractiveBrokersService ibService, TrueFadeScreener screener, IOptions<TrueFadeStrategySettings> settings, ILogger<TrueFadeIBProcessor> logger)
        {
            _dataLayer = dataLayer;
            _ibService = ibService;
            _screener = screener;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task Process(DateTime dateTime)
        {
            await Initialize();
            var marketDayContext = await GetMarketDays(dateTime);
            var assetContext = await GetAssetUniverse();
            var allocatedPositions = await ScreenAssets(assetContext, marketDayContext, _settings.CapitalToTrade);
        }

        public async Task Initialize()
        {
            await _ibService.InitializeHttpAuth();
            _logger.NewLine();
            await _dataLayer.Initialize();
            _logger.NewLine();

            _logger.LogInformation("Fetching account info...");
            var account = await _ibService.GetAccounts();
            _logger.LogInformation($"Account Id: {account.selectedAccount}, IsPaper: {account.isPaper}");
            _logger.NewLine();
        }

        public async Task<MarketDayContext> GetMarketDays(DateTime dateTime)
        {
            _logger.LogInformation("Fetching market day data...");
            var workingDays = await _dataLayer.GetMarketDays(dateTime.AddDays(-(_settings.LookBackMarketDays * 2)), dateTime);
            var lookbackDays = workingDays.Where(x => x.GetTradingOpenTimeUtc().Date < dateTime.Date).TakeLast(_settings.LookBackMarketDays);
            var today = workingDays.Where(x => x.GetTradingOpenTimeUtc().Date >= dateTime.Date).FirstOrDefault();

            if (today == null)
            {
                _logger.LogError($"No trading session today {dateTime.ToShortDateString()}");
                throw new ApplicationException("No trading session today");
            }

            _logger.LogInformation($"Lookback start: {lookbackDays.First().GetTradingOpenTimeUtc().ToShortDateString()}");
            _logger.LogInformation($"Lookback end: {lookbackDays.Last().GetTradingOpenTimeUtc().ToShortDateString()}");
            _logger.LogInformation($"Today: {today.GetTradingOpenTimeUtc().ToShortDateString()}");
            _logger.NewLine();

            return new MarketDayContext(today, lookbackDays);
        }

        public async Task<AssetContext> GetAssetUniverse()
        {
            _logger.LogInformation("Fetching Alpaca assets...");
            var alpacaAssets = await _dataLayer.GetAllTradableAssets();
            _logger.LogInformation($"Alapca assets count: {alpacaAssets.Count}");
            _logger.LogInformation("Fetching IB assets...");
            var ibAssets = await _ibService.GetContractsByExchanges("NASDAQ", "NYSE");
            _logger.LogInformation($"IB assets count: {ibAssets.Count}");
            var ibTickers = ibAssets.Select(x => x.ticker).ToList();
            var mergedSymbols = alpacaAssets.Where(x => ibTickers.Contains(x.Symbol)).Select(x => x.Symbol).ToList();
            _logger.LogInformation($"Merged assets count: {mergedSymbols.Count()}");
            _logger.NewLine();

            return new AssetContext(alpacaAssets, ibAssets, mergedSymbols);
        }

        public async Task<IEnumerable<TrueFadePosition>> ScreenAssets(AssetContext assetContext, MarketDayContext marketDayContext, decimal capitalToTrade, bool logPositions = true)
        {
            _logger.LogInformation("Screening assets...");
            var screenedAssets = await _screener.ScreenTrueFadeIB(
                assetContext.MergedSymbols,
                marketDayContext.Today.GetTradingOpenTimeUtc(),
                marketDayContext.LookbackDays.ToList(),
                _settings.MinimumAverageTrueRangeMultiple,
                _settings.MinimumAverageVolume,
                _settings.MaxAssetCount);

            if (!screenedAssets.Any())
            {
                _logger.LogError("No assets to trade");
                throw new ApplicationException("No assets to trade");
            }

            var mergedAssets = screenedAssets.Join(assetContext.IBAssets, x => x.Symbol, y => y.ticker, (x, y) =>
            {
                x.IBContract = y;
                return x;
            });
            _logger.LogInformation($"Screened asset count: {screenedAssets.Count()}");
            _logger.NewLine();

            _logger.LogInformation("Fetching shortable status...");
            var marketSnapshots = await _ibService.GetShortableStatusSnapshot(mergedAssets.Select(x => x.IBContract.conid));
            var shortableAssets = marketSnapshots.Where(x => string.Equals(x.ShortableStatus, "shortable", StringComparison.InvariantCultureIgnoreCase)).Select(x => x.Conid).ToList();
            var assetsToAllocate = mergedAssets.Join(shortableAssets, x => x.IBContract.conid, y => y, (x, y) => x);
            _logger.LogInformation($"Assets ready to allocate: {assetsToAllocate.Count()}");
            _logger.NewLine();

            if (!assetsToAllocate.Any())
            {
                _logger.LogError("No assets to allocate");
                throw new ApplicationException("No assets to allocate");
            }        

            _logger.LogInformation($"Allocating capitol {capitalToTrade}");
            var allocatedAssets = TrueFadeAllocater.Allocate(assetsToAllocate, capitalToTrade, _settings.MaximumVolumePercentage);
            _logger.NewLine();

            if (logPositions)
            {
                foreach (var position in allocatedAssets)
                {
                    _logger.LogInformation($"{position.Signal.Symbol} ({position.Signal.IBContract.conid}) {position.Signal.EstimatedPrice}, Position size: {position.PositionSize}, Average volume {position.Signal.AverageVolume}, ATR: {position.Signal.AverageTrueRange}, ATR Multiple: {position.Signal.MultipleATR}");
                }
            }

            return allocatedAssets;
        }
    }

    public record MarketDayContext(IIntervalCalendar Today, IEnumerable<IIntervalCalendar> LookbackDays);
    public record AssetContext(List<IAsset> AlpacaAssets, List<Contract> IBAssets, List<string> MergedSymbols);
}
