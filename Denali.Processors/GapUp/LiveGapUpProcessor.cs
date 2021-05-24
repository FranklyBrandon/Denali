using AutoMapper;
using Denali.Models.Shared;
using Denali.Services.Alpaca;
using Denali.Services.WebScrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Denali.Processors.GapUp
{
    public class LiveGapUpProcessor : IProcessor
    {
        private readonly GapUpWebScrapService _gapUpWebScrapService;
        private readonly AlpacaService _alpacaService;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, List<IAggregateData>> _stockData;

        public async Task Process(DateTime startTime, CancellationToken stoppingToken)
        {
            var stocks = await _gapUpWebScrapService.ScrapGapUpSymbols();
            stocks = stocks.OrderByDescending(x => x.VolumeInt).Take(30);
            SubscribeToSymbols(stocks.Select(x => x.Symbol ));
        }

        public Task ShutDown(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }

        public async Task OnBarReceived(IAggregateData barData)
        {
            var stockData = _stockData[barData.Symbol];
            stockData.Add(barData);
        }

        private void SubscribeToSymbols(IEnumerable<string> symbols)
        {
            _alpacaService.InitializeStreamingClient();

            foreach (var symbol in symbols)
            {
                var subscription = _alpacaService.DataStreamingClient.GetMinuteAggSubscription(symbol);
                subscription.Received += async (bar) =>
                {
                    var mappedBar = _mapper.Map<AggregateData>(bar);
                    await OnBarReceived(mappedBar);
                };
                _alpacaService.DataStreamingClient.Subscribe(subscription);
            }
        }
    }
}
