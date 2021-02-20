using System;
using Microsoft.Extensions.Configuration;
using System.Threading;
using Microsoft.Extensions.Logging;
using Denali.Services.Polygon;
using Alpaca.Markets;
using System.Threading.Tasks;
using Denali.Strategies;
using System.Collections.Generic;
using Denali.Models.Shared;
using AutoMapper;
using System.Linq;
using Denali.Services.Alpaca;

namespace Denali.Processors
{
    public class LiveTradingProcessor : IProcessor
    {
        private readonly IConfiguration _configuration;
        private readonly IAggregateStrategy _strategy;
        private readonly PolygonService _polygonService;
        private readonly AlpacaService _alpacaService;
        private readonly IMapper _mapper;
        private readonly ILogger<LiveTradingProcessor> _logger;

        private readonly AggregationPeriod _timeFrame = new AggregationPeriod(1, AggregationPeriodUnit.Minute);
        private readonly DateTime _today = DateTime.Today;

        private Guid _buyOrderId;
        private bool _buyOrderOpen = false;
        private Guid _sellOrderId;
        private bool _sellOrderOpen = false;
        private int _barsSinceBuySubmit = 0;

        public List<IAggregateData> Data { get; set; }

        public LiveTradingProcessor(IConfiguration configuration
            , PolygonService polygonService
            , AlpacaService alpacaService
            , IAggregateStrategy strategy
            , IMapper mapper
            , ILogger<LiveTradingProcessor> logger)
        {
            this._configuration = configuration;
            this._polygonService = polygonService;
            this._alpacaService = alpacaService;
            this._strategy = strategy;
            this._mapper = mapper;
            this._logger = logger;

            Data = new List<IAggregateData>();
        }

        public async Task Process(CancellationToken stoppingToken)
        {
            var ticker = _configuration["ticker"];

            _polygonService.InitializeStreamingClient();
            _polygonService.InitializeDataClient();
            _alpacaService.InitializeTradingClient();
            _alpacaService.InitializeStreamingClient();

            IHistoricalItems<IAgg> currentResponse;
            IHistoricalItems<IAgg> backlogResponse;

            try
            {
                currentResponse = await _polygonService.DataClient.ListAggregatesAsync(
                    new AggregatesRequest(ticker, _timeFrame)
                        .SetInclusiveTimeInterval(_today, _today.AddDays(1)));

                backlogResponse = await _polygonService.DataClient.ListAggregatesAsync(
                    new AggregatesRequest(ticker, _timeFrame)
                        .SetInclusiveTimeInterval(_today.AddDays(-1), _today));
            }
            catch (Exception ex)
            {

                throw;
            }

            // Only use last 20 bars for initialization
            var lastBars = backlogResponse.Items.Skip(Math.Max(0, backlogResponse.Items.Count - 20));
            var currentBars = _mapper.Map<List<AggregateData>>(currentResponse.Items);
            var backlogBars = _mapper.Map<List<AggregateData>>(lastBars);

            _strategy.Initialize(backlogBars);
            currentBars.ForEach(x => Data.Add(x));

            // Wire up events from the streaming client
            WireEvents();

            // Connect and Authenticate
            _logger.LogInformation("Connecting and Authenticating with Alpaca.");
            var alpacaAuthStatus = await _alpacaService.StreamingClient.ConnectAndAuthenticateAsync();
            _logger.LogInformation($"Connection Status: {alpacaAuthStatus}");


            _logger.LogInformation("Connecting and Authenticating with Polygon.");
            var polygonAuthStatus = await _polygonService.StreamingClient.ConnectAndAuthenticateAsync();
            _logger.LogInformation($"Connection Status: {polygonAuthStatus}.");

            // Subscribe to data stream
            _polygonService.StreamingClient.SubscribeMinuteAgg(ticker);
            _logger.LogInformation("Subscribed to minute aggregate data.");

            _logger.LogInformation("===Begin Strategy===");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000);
            }
        }

        public async Task ShutDown(CancellationToken stoppingToken)
        {
            _logger.LogInformation("===Ending Strategy===");
            try
            {
                var orders = await _alpacaService.TradingClient.ListOrdersAsync(new ListOrdersRequest());
                foreach (var order in orders)
                {
                    await _alpacaService.TradingClient.DeleteOrderAsync(order.OrderId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to cancel all orders orders: {ex.Message}");
                throw;
            }

            await _polygonService.Disconnect();
            await _alpacaService.Disconnect();
        }

        public void WireEvents()
        {
            _polygonService.StreamingClient.MinuteAggReceived += OnMinuteAggregate;
            _alpacaService.StreamingClient.OnTradeUpdate += OnTradeUpdate;
        }

        private void OnTradeUpdate(ITradeUpdate obj)
        {
            _logger.LogInformation($"Trade Update of type {obj.Event} for Order {obj.Order.OrderId}");
            if (obj.Event == TradeEvent.Fill)
            {
                if (obj.Order.OrderId == _buyOrderId)
                {
                    _logger.LogInformation("Buy Order filled");
                    _buyOrderOpen = false;

                } else
                {
                    _logger.LogInformation("Sell Order filled");
                    _sellOrderOpen = false;
                }
            }
            else if (obj.Event == TradeEvent.New)
            {
                if (obj.Order.OrderSide == OrderSide.Sell)
                {
                    if (obj.Order.OrderType == OrderType.Limit)
                    {
                        _logger.LogInformation("Sell Order Initiated");
                        _sellOrderId = obj.Order.OrderId;
                        _sellOrderOpen = true;
                    }
                }
            }
            else if (obj.Event == TradeEvent.Replaced)
            {
                if (obj.Order.OrderSide == OrderSide.Sell)
                {
                    _logger.LogInformation("Stop Loss Initiated");
                    _sellOrderOpen = true;
                }
            }
        }

        private async void OnMinuteAggregate(IStreamAgg obj)
        {
            _logger.LogInformation($"Aggregate received at {obj.EndTimeUtc}");
            _logger.LogInformation($"O: {obj.Open}, H: {obj.High}, L: {obj.Low}, C: {obj.Close}, " +
                $"V: {obj.Volume}, ST: {obj.StartTimeUtc}, ET: {obj.EndTimeUtc}");

            var bar = _mapper.Map<AggregateData>(obj);
            Data.Add(bar);

            // If a buy order is already open, expire it, or wait for it to fill before submitting another order
            if (_buyOrderOpen)
            {
                _barsSinceBuySubmit++;

                if (_barsSinceBuySubmit >= 1)
                {
                    _logger.LogInformation("Canceling existing buy Order");
                    _barsSinceBuySubmit = 0;
                    await _alpacaService.TradingClient.DeleteOrderAsync(_buyOrderId);
                    _buyOrderOpen = false;
                    _strategy.ProcessTick(Data);
                    return;
                }
            }
            else
            {
                if (_strategy.ProcessTick(Data))
                {
                    var stopLoss = Math.Min(obj.Close - ((obj.Open - obj.Close) / 2), obj.Close - 0.04M);
                    if (!_sellOrderOpen)
                    {
                        SubmitLongOrder(obj.Close, stopLoss, obj.Symbol, 1);
                    }
                }
            }     
        }

        private async void SubmitLongOrder(decimal price, decimal stoploss, string ticker, int quantity = 1)
        {
            _logger.LogInformation("Submitting order");
            var orderBase = OrderSide.Buy.Limit(ticker, quantity, price);
            var orderRequest = orderBase.Bracket(price + 0.02M, stoploss, null);
            try
            {
                var order = await _alpacaService.TradingClient.PostOrderAsync(orderRequest);
                _buyOrderId = order.OrderId;
                _buyOrderOpen = true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Order Failed to post: {ex.Message}");
            }
        }
    }
}
