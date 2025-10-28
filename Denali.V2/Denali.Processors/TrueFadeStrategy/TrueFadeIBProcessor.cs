using Alpaca.Markets;
using Denali.Services;
using Denali.Shared.Extensions;
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
            await _ibService.InitializeHttpAuth();
            _logger.NewLine();
            await _dataLayer.Initialize();
            _logger.NewLine();

            _logger.LogInformation("Fetching account info...");
            var account = await _ibService.GetAccounts();
            _logger.LogInformation($"Account Id: {account.selectedAccount}, IsPaper: {account.isPaper}");
            _logger.NewLine();

            _logger.LogInformation("Fetching market day data...");
            var workingDays = await _dataLayer.GetMarketDays(dateTime.AddDays(-(_settings.LookBackMarketDays * 2)), dateTime);
            var lookbackDays = workingDays.Where(x => x.GetTradingOpenTimeUtc().Date < dateTime.Date).TakeLast(_settings.LookBackMarketDays);
            var today = workingDays.Where(x => x.GetTradingOpenTimeUtc().Date >= dateTime.Date).FirstOrDefault();

            if (today == null)
            {
                _logger.LogError($"No trading session today {dateTime.ToShortDateString()}");
                return;
            }

            _logger.LogInformation($"Lookback start: {lookbackDays.First().GetTradingOpenTimeUtc().ToShortDateString()}");
            _logger.LogInformation($"Lookback end: {lookbackDays.Last().GetTradingOpenTimeUtc().ToShortDateString()}");
            _logger.LogInformation($"Today: {today.GetTradingOpenTimeUtc().ToShortDateString()}");
            _logger.NewLine();

            _logger.LogInformation("Fetching Alpaca assets...");
            var dataAssets = await _dataLayer.GetAllTradableAssets();
            _logger.LogInformation($"Alapca assets count: {dataAssets.Count}");
            _logger.LogInformation("Fetching IB assets...");
            var ibAssets = await _ibService.GetContractsByExchanges("NASDAQ", "NYSE");
            _logger.LogInformation($"IB assets count: {ibAssets.Count}");
            var ibTickers = ibAssets.Select(x => x.ticker).ToList();
            var dataTickers = dataAssets.Where(x => ibTickers.Contains(x.Symbol)).Select(x => x.Symbol);
            _logger.LogInformation($"Merged assets count: {dataTickers.Count()}");
            _logger.NewLine();

            _logger.LogInformation("Screening assets...");
            var screenedAssets = await _screener.ScreenTrueFadeIB(
                dataTickers, 
                today.GetTradingOpenTimeUtc(), 
                lookbackDays.ToList(), 
                _settings.MinimumAverageTrueRangeMultiple,
                _settings.MinimumAverageVolume,
                _settings.MaxAssetCount);

            if (!screenedAssets.Any())
            {
                _logger.LogInformation("No assets to trade");
                return;
            }

            var mergedAssets = screenedAssets.Join(ibAssets, x => x.Symbol, y => y.ticker, (x, y) =>
            {
                x.IBContract = y;
                return x;
            });
            _logger.LogInformation($"Screened asset count: {screenedAssets.Count()}");
            _logger.NewLine();

            _logger.LogInformation("Fetching shortable status...");
            var marketSnapshots = await _ibService.GetMarketSnapshots(mergedAssets.Select(x => x.IBContract.conid));
            var shortableAssets = marketSnapshots.Where(x => string.Equals(x.ShortableStatus, "shortable", StringComparison.InvariantCultureIgnoreCase)).Select(x => x.Conid).ToList();
            var assetsToAllocate = mergedAssets.Join(shortableAssets, x => x.IBContract.conid, y => y, (x, y) => x);
            _logger.LogInformation($"Assets ready to allocate: {assetsToAllocate.Count()}");
            _logger.NewLine();

            if (!assetsToAllocate.Any())
                return;

            _logger.LogInformation($"Allocating capitol {_settings.CapitalToTrade}");
            var allocatedAssets =  TrueFadeAllocater.Allocate(assetsToAllocate, _settings.CapitalToTrade, _settings.MaximumVolumePercentage);
            _logger.NewLine();

            foreach (var position in allocatedAssets)
            {
                _logger.LogInformation($"{position.Signal.Symbol} ({position.Signal.IBContract.conid}) {position.Signal.EstimatedPrice}, Position size: {position.PositionSize}, Average volume {position.Signal.AverageVolume}, ATR: {position.Signal.AverageTrueRange}, ATR Multiple: {position.Signal.MultipleATR}");
            }
        }

        public async Task EnterPositions()
        {

        }
    }
}
