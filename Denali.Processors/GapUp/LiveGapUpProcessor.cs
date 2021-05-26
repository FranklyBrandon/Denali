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

        public LiveGapUpProcessor(
            GapUpWebScrapService gapUpWebScrapService
            , AlpacaService alpacaService
            , IMapper mapper)
        {
            this._gapUpWebScrapService = gapUpWebScrapService;
            this._alpacaService = alpacaService;
            this._mapper = mapper;
        }

        public async Task Process(DateTime startTime, CancellationToken stoppingToken)
        {
            //var stocks = await _gapUpWebScrapService.ScrapGapUpSymbols();
            //stocks = stocks.OrderByDescending(x => x.VolumeInt).Take(1);
            await SubscribeToSymbols(new List<string> { "AAPL"});
        }

        public Task ShutDown(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }

        public void OnBarReceived(IAggregateData barData)
        {
            var stockData = _stockData[barData.Symbol];
            stockData.Add(barData);
        }

        private async Task SubscribeToSymbols(IEnumerable<string> symbols)
        {
            _alpacaService.InitializeDataStreamingclient();
            var la = await _alpacaService.DataStreamingClient.ConnectAndAuthenticateAsync();

            foreach (var symbol in symbols)
            {
                _stockData[symbol] = new List<IAggregateData>();

                var subscription = _alpacaService.DataStreamingClient.GetMinuteAggSubscription(symbol);
                subscription.Received += (bar) =>
                {
                    Console.WriteLine($"BAR RECEIVED! {DateTime.Now.ToString()}");
                    var mappedBar = _mapper.Map<AggregateData>(bar);
                    OnBarReceived(mappedBar);
                };
                _alpacaService.DataStreamingClient.Subscribe(subscription);
            }
        }
    }
}
