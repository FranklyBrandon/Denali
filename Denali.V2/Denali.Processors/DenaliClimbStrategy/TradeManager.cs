using Alpaca.Markets;
using Denali.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
namespace Denali.Processors.DenaliClimbStrategy
{
    public class TradeManager
    {
        private readonly BrokerageLayerComponent _brokerageLayer;
        private static bool _traded = false;
        private static readonly SemaphoreSlim _lock = new SemaphoreSlim(1,1);
        private readonly ILogger _logger;

        public TradeManager(BrokerageLayerComponent brokerageLayerComponent, ILogger<TradeManager> logger)
        {
            _brokerageLayer = brokerageLayerComponent;
            _logger = logger;
        }

        public async Task ProcessEntry(decimal localLow, decimal latestClose, string symbol)
        {
            await _lock.WaitAsync();
            try
            {
                var takeProfit = latestClose + (latestClose * 0.5m);
                _logger.LogInformation($"Submitting order for {symbol} with latest close {latestClose}. Stop loss at {localLow} and take profit at {takeProfit}");
                if (!_traded)
                {
                    _traded = true;
                    var order = new NewOrderRequest(symbol, OrderQuantity.Notional(1000), OrderSide.Buy, OrderType.Market, TimeInForce.Day)
                    {
                        OrderClass = OrderClass.Bracket,
                        TakeProfitLimitPrice = latestClose + (latestClose * 0.5m),
                        StopLossStopPrice = localLow
                    };

                    await _brokerageLayer.SubmitOrder(order);
                }
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}
