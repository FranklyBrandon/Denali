using Alpaca.Markets;
using Denali.Services;
using Denali.Shared.Time;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Denali.Processors.TrueFadeStrategy
{
    public class TrueFadeStrategy
    {
        private readonly DataLayerComponent _dataLayer;
        private readonly BrokerageLayerComponent _brokerageLayer;
        private readonly TrueFadeStrategySettings _settings;
        private readonly ILogger _logger;

        private readonly TrueFadeScreener _screener;
        private readonly Dictionary<string, TrueFadePosition> _positions;
        private ScheduledTask _scheduledTask;
        private readonly FileService _fileService;

        public TrueFadeStrategy(DataLayerComponent dataLayer, BrokerageLayerComponent brokerageLayer, FileService fileService, TrueFadeScreener screener, IOptions<TrueFadeStrategySettings> settings, ILogger<TrueFadeStrategy> logger)
        {
            _dataLayer = dataLayer;
            _brokerageLayer = brokerageLayer;
            _fileService = fileService;
            _settings = settings.Value;
            _screener = screener;
            _logger = logger;
            _positions = new Dictionary<string, TrueFadePosition>();
            
        }
        public async Task Process(DateTime dateTime)
        {
            await _dataLayer.Initialize();
            var assets = await _dataLayer.GetAllTradableAssets();

            var workingDays = await _dataLayer.GetMarketDays(dateTime.AddDays(-(_settings.LookBackMarketDays * 2)), dateTime);
            var lookbackDays = workingDays.Where(x => x.GetTradingOpenTimeUtc().Date < dateTime.Date);
            var today = workingDays.Where(x => x.GetTradingOpenTimeUtc().Date >= dateTime.Date).FirstOrDefault();

            if (today == null)
            {
                _logger.LogError($"No trading session today {dateTime.ToShortDateString()}");
                return;
            }

            _logger.LogInformation("Screening assets...");
            var positionsToEnter = await ScreenAssets(assets, today, lookbackDays.TakeLast(_settings.LookBackMarketDays).ToList(), _settings.CapitalToTrade);
            await EnterPositions(positionsToEnter);

            var closeTime = today.GetTradingCloseTimeUtc().AddMinutes(-10);
            _logger.LogInformation($"Scheduling close for {closeTime.ToShortTimeString()}");
            _scheduledTask = new ScheduledTask(
                closeTime,
                () => ClosePositions(closeTime)
            );
        }

        public async Task<IEnumerable<TrueFadePosition>> ScreenAssets(List<IAsset> assets, IIntervalCalendar currentDay, List<IIntervalCalendar> backlogDays, decimal capitolToTrade)
        {
            var screenedAssets = await _screener.ScreenTrueFade(assets, currentDay.GetTradingOpenTimeUtc(), backlogDays, _settings.MinimumAverageTrueRangeMultiple, _settings.MaxAssetCount);

            if (!screenedAssets.Any())
                return new List<TrueFadePosition>();

            return TrueFadeAllocater.Allocate(screenedAssets, capitolToTrade, _settings.MaximumVolumePercentage);
        }

        public async Task EnterPositions(IEnumerable<TrueFadePosition> positions)
        {
            _brokerageLayer.StreamTradeUpdates(HandleTradeUpdate);

            List<Task> entryOrders = new List<Task>();
            foreach (var position in positions)
            {
                var entryRequest = new NewOrderRequest(
                    symbol: position.Signal.Symbol,
                    quantity: 0,
                    side: OrderSide.Sell,
                    type: OrderType.Market,
                    duration: TimeInForce.Gtc
                );

                _positions.Add(position.Signal.Symbol, position);
                //_logger.LogInformation($"Entering {position.Signal.Symbol} {position.Signal.EstimatedPrice}, Position size: {position.Signal.PositionSize}, Average volume {position.Signal.AverageVolume}, ATR: {position.Signal.AverageTrueRange}, ATR Multiple: {position.Signal.MultipleATR}");
                entryOrders.Add(_brokerageLayer.SubmitOrder(entryRequest));
            }

            await Task.WhenAll(entryOrders);
        }

        public async Task ClosePositions(DateTime today)
        {
            _logger.LogInformation("Liquidating all open orders");
            await _brokerageLayer.CloseAllPositions();
            await _fileService.WriteJSONResourceToFile($"TrueFade-{today.Year}-{today.Month}-{today.Day}", _positions);
        }

        private async void HandleTradeUpdate(ITradeUpdate update)
        {
            if (update.Event == TradeEvent.Fill)
            {
                var filledOrder = update.Order;
                //_positions[filledOrder.Symbol].FilledOrders.Add(filledOrder);

                // Entry Sell order
                if (filledOrder.OrderSide == OrderSide.Sell)
                {
                    if (!filledOrder.AverageFillPrice.HasValue)
                    {
                        _logger.LogError($"No fill price for asset {filledOrder.Symbol}");
                        return;
                    }

                    if (!filledOrder.Quantity.HasValue)
                    {
                        _logger.LogError($"No quantity for asset {filledOrder.Symbol}");
                        return;
                    }

                    var entryPrice = filledOrder.AverageFillPrice.Value;
                    var entryQunatity = filledOrder.Quantity.Value;
                    var stopPrice = _positions[filledOrder.Symbol].Signal.AverageTrueRange + entryPrice;

                    _logger.LogInformation($"SELL filled {filledOrder.Symbol}, Average Price: {filledOrder.AverageFillPrice.Value}, Quantity: {filledOrder.Quantity.Value}, StopLoss: {stopPrice}");

                    var stopLossOrder = new NewOrderRequest(
                        symbol: filledOrder.Symbol,
                        quantity: OrderQuantity.Fractional(entryQunatity),
                        side: OrderSide.Buy,
                        type: OrderType.Stop,
                        duration: TimeInForce.Gtc
                    )
                    {
                        StopPrice = stopPrice
                    };

                    await _brokerageLayer.SubmitOrder(stopLossOrder);
                }
                else
                {
                    _logger.LogInformation($"BUY filled {filledOrder.Symbol}, Average Price: {filledOrder.AverageFillPrice.Value}, Quantity: {filledOrder.Quantity.Value}");
                }
            }
        }
    }
}
