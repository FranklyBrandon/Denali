using AutoMapper;
using Denali.Algorithms.ActionAnalysis;
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
        private readonly Dictionary<string, GapUpCoolOff> _stockData;

        public LiveGapUpProcessor(
            GapUpWebScrapService gapUpWebScrapService
            , AlpacaService alpacaService
            , IMapper mapper)
        {
            this._gapUpWebScrapService = gapUpWebScrapService;
            this._alpacaService = alpacaService;
            this._mapper = mapper;
            _stockData = new Dictionary<string, GapUpCoolOff>();
        }

        public async Task Process(DateTime startTime, CancellationToken stoppingToken)
        {
            var stocks = await _gapUpWebScrapService.ScrapGapUpSymbols();
            var symbols = stocks.OrderByDescending(x => x.VolumeInt).Take(30).Select(x => x.Symbol);
            await SubscribeToSymbols(symbols);
            //https://www.tradingview.com/chart/?symbol=NASDAQ:AAPL&interval=1
        }

        public Task ShutDown(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }

        public void OnBarReceived(IAggregateData barData)
        {
            var strategy = _stockData[barData.Symbol];
            strategy.OnBarReceived(barData);
        }

        private async Task SubscribeToSymbols(IEnumerable<string> symbols)
        {
            _alpacaService.InitializeDataStreamingclient();
            var authenticationStatus = await _alpacaService.DataStreamingClient.ConnectAndAuthenticateAsync();

            foreach (var symbol in symbols)
            {
                _stockData[symbol] = new GapUpCoolOff(DateTime.UtcNow, 10, 0, symbol);

                var subscription = _alpacaService.DataStreamingClient.GetMinuteAggSubscription(symbol);
                subscription.Received += (bar) =>
                {
                    var mappedBar = _mapper.Map<AggregateData>(bar);
                    OnBarReceived(mappedBar);
                };
                _alpacaService.DataStreamingClient.Subscribe(subscription);
            }
        }

        private async Task SubmitBuyOrder()
        {
            await _alpacaService.TradingClient.PostOrderAsync()
        }
    }
}
