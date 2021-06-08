using AutoMapper;
using Denali.Algorithms.ActionAnalysis;
using Denali.Models.Shared;
using Denali.Services.Alpaca;
using Denali.Services.WebScrap;
using Denali.Shared.Utility;
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
        private readonly Dictionary<string, GapUpCoolOff> _strategies;
        private readonly TimeUtils _timeUtils;

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
            this._timeUtils = new TimeUtils();
            _strategies = new Dictionary<string, GapUpCoolOff>();
        }

        public async Task Process(DateTime startTime, CancellationToken stoppingToken)
        {
            var fromDate = _timeUtils.GetNYSEOpenDateTime(DateTime.UtcNow.Date);
            var toDate = _timeUtils.GetNYSECloseDateTime(DateTime.UtcNow.Date);
            var stocks = await _gapUpWebScrapService.ScrapGapUpSymbols();
            var symbols = stocks.OrderByDescending(x => x.VolumeInt).Take(30).Select(x => x.Symbol);
            await SubscribeToSymbols(symbols, fromDate, toDate);
            //https://www.tradingview.com/chart/?symbol=NASDAQ:AAPL&interval=1
        }

        public async Task ShutDown(CancellationToken stoppingToken)
        {
            await _alpacaService.Disconnect();
            await _alpacaTradingService.Disconnect();
        }

        public void OnBarReceived(IAggregateData barData)
        {
            var strategy = _strategies[barData.Symbol];
            strategy.OnBarReceived(barData);
        }

        private async Task SubscribeToSymbols(IEnumerable<string> symbols, DateTime fromdate, DateTime toDate)
        {
            _alpacaService.InitializeDataClient();
            _alpacaService.InitializeDataStreamingclient();
            _alpacaTradingService.InitializeTradingClient();
            _alpacaTradingService.InitializeStreamingClient();

            var dataStreamAuth = await _alpacaService.DataStreamingClient.ConnectAndAuthenticateAsync();
            var tradeStreamAuth = await _alpacaTradingService.StreamingClient.ConnectAndAuthenticateAsync();
            var existingData = await _alpacaService.GetHistoricBarData(fromdate, toDate, Alpaca.Markets.TimeFrame.Minute, symbols);

            foreach (var symbol in symbols)
            {
                _strategies[symbol] = new GapUpCoolOff(DateTime.UtcNow.Date, 10, 0, symbol, _alpacaTradingService);

                List<IAggregateData> currentBars;
                if (existingData.TryGetValue(symbol, out currentBars))
                    _strategies[symbol].SetInitialData(currentBars);

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
