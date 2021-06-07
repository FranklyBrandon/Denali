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
        private readonly AlpacaDataService _alpacaService;
        private readonly AlpacaTradingService _alpacaTradingService;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, GapUpCoolOff> _stockData;

        public LiveGapUpProcessor(
            GapUpWebScrapService gapUpWebScrapService
            , AlpacaDataService alpacaService
            , AlpacaTradingService alpacaTradingService
            , IMapper mapper)
        {
            this._gapUpWebScrapService = gapUpWebScrapService;
            this._alpacaService = alpacaService;
            this._alpacaTradingService = alpacaTradingService;
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

        public async Task ShutDown(CancellationToken stoppingToken)
        {
            await _alpacaService.Disconnect();
            await _alpacaTradingService.Disconnect();
        }

        public void OnBarReceived(IAggregateData barData)
        {
            var strategy = _stockData[barData.Symbol];
            strategy.OnBarReceived(barData);
        }

        private async Task SubscribeToSymbols(IEnumerable<string> symbols)
        {
            _alpacaService.InitializeDataStreamingclient();
            _alpacaTradingService.InitializeTradingClient();
            _alpacaTradingService.InitializeStreamingClient();

            var dataStreamAuth = await _alpacaService.DataStreamingClient.ConnectAndAuthenticateAsync();
            var tradeStreamAuth = await _alpacaTradingService.StreamingClient.ConnectAndAuthenticateAsync();

            foreach (var symbol in symbols)
            {
                _stockData[symbol] = new GapUpCoolOff(DateTime.UtcNow, 10, 0, symbol, _alpacaTradingService);

                var subscription = _alpacaService.DataStreamingClient.GetMinuteAggSubscription(symbol);
                subscription.Received += (bar) =>
                {
                    var mappedBar = _mapper.Map<AggregateData>(bar);
                    OnBarReceived(mappedBar);
                };
                _alpacaService.DataStreamingClient.Subscribe(subscription);
            }
        }
    }
}
