using Alpaca.Markets;
using AutoMapper;
using Denali.Models;
using Denali.Processors.Exceptions;
using Denali.Services;
using Denali.Services.Aggregators;
using Denali.Shared.Extensions;
using Denali.TechnicalAnalysis;
using Denali.TechnicalAnalysis.ElephantBars;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Denali.Processors.ElephantStrategy
{
    public class ElephantRideStrategy
    {
        private readonly AlpacaService _alpacaService;
        private readonly ElephantBarSettings _elephantBarSettings;
        private readonly ILogger<ElephantRideStrategy> _logger;
        private readonly BarAggregator _barAggregator;
        private readonly TradeAggregator _tradeAggregator;
        private readonly IMapper _mapper;

        private const int BACKLOG_DAYS = 2;
        private const int BACKLOG_MARKET_DAYS = 5;

        private List<IAggregateBar> AggregateBars;
        private SimpleMovingAverage _sma3;
        private SimpleMovingAverage _sma8;
        private SimpleMovingAverage _sma21;
        private ElephantBars _elephantBars;

        private Guid _limitOrderId;
        private bool _limitOrderOpen;
        private bool _limitOrderFilled;

        public ElephantRideStrategy(
            AlpacaService alpacaService, 
            IOptions<ElephantBarSettings> elephantBarSettings, 
            BarAggregator barAggregator,
            TradeAggregator tradeAggregator,
            IMapper mapper,
            ILogger<ElephantRideStrategy> logger)
        {
            _alpacaService = alpacaService ?? throw new ArgumentNullException(nameof(alpacaService));
            _elephantBarSettings = elephantBarSettings?.Value ?? throw new ArgumentNullException(nameof(elephantBarSettings));
            _barAggregator = barAggregator ?? throw new ArgumentNullException(nameof(barAggregator));
            _tradeAggregator = tradeAggregator ?? throw new ArgumentNullException(nameof(tradeAggregator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _sma3 = new SimpleMovingAverage(3);
            _sma8 = new SimpleMovingAverage(8);
            _sma21 = new SimpleMovingAverage(21);

            _elephantBars = new ElephantBars(_elephantBarSettings);
        }

        /// <summary>
        /// Requires Trading Client Initialized. Requires Data Client Initialized
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task Setup(DateTime date)
        {
            await _alpacaService.InitializeDataStreamingClient();
            await _alpacaService.InitializeStreamingClient();
            _alpacaService.InitializeTradingclient();
            _alpacaService.InitializeDataClient();

            var marketDays = await _alpacaService.GeOpenMarketDays(BACKLOG_MARKET_DAYS, date);

            // Get backlog market days plus the current date
            var daysThatNeedData = marketDays.Skip(1).Take(BACKLOG_DAYS).Reverse();
            var currentDate = marketDays.First();
            if (currentDate.GetSessionOpenTimeUtc().Day != date.Day)
            {
                _logger.LogInformation($"No trading window detected for day {date.Day}");
                throw new NoTradingWindowException();
            }

            // Populate backlog data
            var backlogBars = new List<IBar>();
            foreach (var day in daysThatNeedData)
            {
                var response = await _alpacaService.AlpacaDataClient.ListHistoricalBarsAsync(
                    new HistoricalBarsRequest("AAPL", day.GetTradingOpenTimeUtc(), day.GetTradingCloseTimeUtc(), new BarTimeFrame(5, BarTimeFrameUnit.Minute)));
                
                if (!string.IsNullOrWhiteSpace(response.NextPageToken))
                    throw new Exception("More than one page");

                backlogBars.AddRange(response.Items);
            }

            IEnumerable<IAggregateBar> mappedBars = _mapper.Map<List<AggregateBar>>(backlogBars);
            AggregateBars = mappedBars.ToList();

            // Initialize TA
            _sma3.Analyze(AggregateBars);
            _sma8.Analyze(AggregateBars);
            _sma21.Analyze(AggregateBars);
            _elephantBars.Initialize(AggregateBars);

            // Subscribe to realtime trades and candlestick data
            var tradeSubscription = _alpacaService.AlpacaDataStreamingClient.GetTradeSubscription("AAPL");
            tradeSubscription.Received += OnTradePrice;

            var barSubscription = _alpacaService.AlpacaDataStreamingClient.GetMinuteBarSubscription("AAPL");
            barSubscription.Received += OnMinuteBar;

            _alpacaService.AlpacaStreamingclient.OnTradeUpdate += OnTradeUpdate;

            await _alpacaService.AlpacaDataStreamingClient.SubscribeAsync(tradeSubscription);
            await _alpacaService.AlpacaDataStreamingClient.SubscribeAsync(barSubscription);

            // Initialize the aggregators to calculate the specified intervals
            _barAggregator.SetMinuteInterval(5);
            _tradeAggregator.SetMinuteInterval(5);

            var lastUpdate = (int)_barAggregator.Round(DateTime.UtcNow.Minute);
            _barAggregator.SetLastUpdateMinute(lastUpdate);
            _tradeAggregator.SetLastUpdateMinute(lastUpdate);

            _barAggregator.OnBar += OnIntervalBar;
            _tradeAggregator.OnBarOpen += OnBarOpen;
        }

        public void OnTradePrice(ITrade trade)
        {
            _sma3.ProvisionalChange(trade, new List<IAggregateBar>(AggregateBars));
            _sma8.ProvisionalChange(trade, new List<IAggregateBar>(AggregateBars));
            _sma21.ProvisionalChange(trade, new List<IAggregateBar>(AggregateBars));
            _tradeAggregator.OnTrade(trade);
        }

        public void OnMinuteBar(IBar bar)
        {
            //_logger.LogInformation($"Minute Bar received: Open: {bar.Open}, High: {bar.High}, Low: {bar.Low}, Close: {bar.Close}, Time: {bar.TimeUtc}, LastUpdateMinute: {_barAggregator._lastUpdateMinute}");
            _barAggregator.OnMinuteBar(bar);
        }

        public void OnIntervalBar(IAggregateBar bar)
        {

            AggregateBars.Add(bar);
            _sma3.Analyze(AggregateBars);
            _sma8.Analyze(AggregateBars);
            _sma21.Analyze(AggregateBars);
            _elephantBars.Analyze(AggregateBars);
            _logger.LogInformation($"Interval Bar received: OHLC: ({bar.Open},{bar.High},{bar.Low},{bar.Close}), SMAS: ({_sma3.MovingAverages.Last()}, {_sma8.MovingAverages.Last()}, {_sma21.MovingAverages.Last()}) Time: {bar.TimeUtc}");
        }

        public void OnBarOpen(IAggregateBar bar)
        {
            _logger.LogInformation($"Bar Open Received: Open: {bar.Open}, SMAs: ({_sma3.ProvisionalValue}, {_sma8.ProvisionalValue}, {_sma21.ProvisionalValue}), Elephant Trigger: {_elephantBars.Trigger} Time: {bar.TimeUtc}");
            ProcessBarOpen(bar.Open);
        }

        private void OnTradeUpdate(ITradeUpdate tradeUpdate)
        {
                _logger.LogInformation($"Order Update: {tradeUpdate.Order.OrderId}: {tradeUpdate.Event}");
                if (tradeUpdate.Event == TradeEvent.Fill) 
                {
                    _limitOrderOpen = false;
                    _limitOrderFilled = true;
                }
                else if (tradeUpdate.Event == TradeEvent.Canceled)
                {
                    _limitOrderOpen = false;
                    _limitOrderFilled = false;
                }
        }

        private async Task ProcessBarOpen(decimal openPrice)
        {
            if (_limitOrderOpen)
            {
                await _alpacaService.AlpacaTradingClient.CancelOrderAsync(_limitOrderId);
            }
            else if (_limitOrderFilled)
            {
                await _alpacaService.AlpacaTradingClient.DeletePositionAsync(new DeletePositionRequest("AAPL"));
            }

            if ((_sma3.ProvisionalValue > _sma8.ProvisionalValue && _sma8.ProvisionalValue > _sma21.ProvisionalValue) 
                && (_sma3.MovingAverages.GetHistoricValue(1) < _sma3.ProvisionalValue && _sma8.MovingAverages.GetHistoricValue(1) < _sma8.ProvisionalValue))
            {
                var triggerPrice = openPrice + _elephantBars.Trigger;
                var limitOrder = StopLimitOrder.Buy("AAPL", OrderQuantity.FromInt64(1), triggerPrice, triggerPrice);
                var order = await _alpacaService.AlpacaTradingClient.PostOrderAsync(limitOrder);
                _limitOrderId = order.OrderId;
                _limitOrderOpen = true;
                _logger.LogInformation($"Long Limit at: {triggerPrice}: {order.OrderId}");
            }
            else if ((_sma3.ProvisionalValue < _sma8.ProvisionalValue && _sma8.ProvisionalValue < _sma21.ProvisionalValue)
                && (_sma3.MovingAverages.GetHistoricValue(1) > _sma3.ProvisionalValue && _sma8.MovingAverages.GetHistoricValue(1) > _sma8.ProvisionalValue))
            {
                var triggerPrice = openPrice - _elephantBars.Trigger;
                var limitOrder = StopLimitOrder.Sell("AAPL", OrderQuantity.FromInt64(1), triggerPrice, triggerPrice);
                var order = await _alpacaService.AlpacaTradingClient.PostOrderAsync(limitOrder);
                _limitOrderId = order.OrderId;
                _limitOrderOpen = true;
                _logger.LogInformation($"Short Limit at: {triggerPrice}: {order.OrderId}");
            }
        }
    }
}
