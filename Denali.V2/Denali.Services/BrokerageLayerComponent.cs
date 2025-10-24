using Alpaca.Markets;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Drawing;

namespace Denali.Services
{
    public class BrokerageLayerComponent
    {
        private readonly AlpacaService _alpacaService;
        private readonly ILogger _logger;

        public BrokerageLayerComponent(AlpacaService alpacaService, ILogger<BrokerageLayerComponent> logger)
        {
            _alpacaService = alpacaService;
            _logger = logger;
        }

        public async Task<IOrder> SubmitOrder(NewOrderRequest order) =>
            await _alpacaService.AlpacaTradingClient.PostOrderAsync(order);

        public void StreamTradeUpdates(Action<ITradeUpdate> action) =>
            _alpacaService.AlpacaStreamingclient.OnTradeUpdate += action;

        public async Task<IReadOnlyList<IPositionActionStatus>> CloseAllPositions() => 
            await _alpacaService.AlpacaTradingClient.DeleteAllPositionsAsync();
    }
}
