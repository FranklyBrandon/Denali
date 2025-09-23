using Alpaca.Markets;
using Denali.Models;
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

        public async Task ProcessEntry(DenaliClimbEntrySignal entrySignal)
        {
            await _lock.WaitAsync();
            try
            {
                _logger.LogInformation($"Submitting order for {entrySignal.SignalBar.Symbol} with latest close {entrySignal.SignalBar.Close}. Stop loss at {entrySignal.StopLoss} and take profit at {entrySignal.TakeProfit}");
                if (!_traded)
                {
                    _traded = true;
                    var order = new NewOrderRequest(entrySignal.SignalBar.Symbol, OrderQuantity.Notional(1000), OrderSide.Buy, OrderType.Market, TimeInForce.Day)
                    {
                        OrderClass = OrderClass.Bracket,
                        TakeProfitLimitPrice = entrySignal.TakeProfit,
                        StopLossStopPrice = entrySignal.StopLoss             
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
