using Alpaca.Markets;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Denali.Services.Alpaca
{
    public class AlpacaTradingService
    {
        private readonly AlpacaSettings _settings;
        public AlpacaTradingClient TradingClient { get; private set; }
        public AlpacaStreamingClient StreamingClient { get; private set; }
        public ConcurrentDictionary<string, IOrder> OrderMap { get; set; }


        public AlpacaTradingService(AlpacaSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public void InitializeTradingClient()
        {
            if (TradingClient != null)
            {
                TradingClient.Dispose();
                TradingClient = null;
            }

            var config = new AlpacaTradingClientConfiguration
            {
                ApiEndpoint = new Uri(_settings.MarketUrl),
                SecurityId = new SecretKey(_settings.APIKey, _settings.APISecret)
            };

            TradingClient = new AlpacaTradingClient(config);
            OrderMap = new ConcurrentDictionary<string, IOrder>();
        }

        public void InitializeStreamingClient()
        {
            if (StreamingClient != null)
            {
                StreamingClient.Dispose();
                StreamingClient = null;
            }

            var config = new AlpacaStreamingClientConfiguration
            {
                ApiEndpoint = new Uri(_settings.TradingStreamingURL),
                SecurityId = new SecretKey(_settings.APIKey, _settings.APISecret)
            };

            StreamingClient = new AlpacaStreamingClient(config);
            StreamingClient.OnTradeUpdate += StreamingClient_OnTradeUpdate;
        }

        private void StreamingClient_OnTradeUpdate(ITradeUpdate obj)
        {
            OrderMap[obj.Order.Symbol] = obj.Order;
        }

        public bool HasOpenPosition(string symbol)
        {
            IOrder order;
            if (OrderMap.TryGetValue(symbol, out order))
            {
                if (order.OrderStatus == OrderStatus.Canceled
                    || order.OrderStatus == OrderStatus.Fill
                    || order.OrderStatus == OrderStatus.Filled)
                    return false;

                return true;
            }

            return false;
        }
        public async Task<IOrder> EnterPosition(string symbol, int quantity, OrderType type, TimeInForce time)
        {
            var request = new NewOrderRequest(symbol, OrderQuantity.FromInt64(quantity), OrderSide.Buy, type, time);
            return await TradingClient.PostOrderAsync(request);
        }

        public async Task<IOrder> ClosePosition(string symbol, OrderType type, TimeInForce time)
        {
            IOrder order;
            if (OrderMap.TryGetValue(symbol, out order))
            {
                var side = order.OrderSide == OrderSide.Buy ? OrderSide.Sell : OrderSide.Buy;
                var request = new NewOrderRequest(order.Symbol, OrderQuantity.FromInt64(order.IntegerQuantity), side, type, time);
                return await TradingClient.PostOrderAsync(request);
            }

            return null;

        }

        public async Task Disconnect()
        {
            if (StreamingClient != null)
            {
                await StreamingClient.DisconnectAsync();
                StreamingClient.Dispose();
                StreamingClient = null;
            }

            if (TradingClient != null)
            {
                TradingClient.Dispose();
                TradingClient = null;
            }
        }
    }
}
