using Alpaca.Markets;
using Denali.Models;
using Denali.Services;
using Denali.Services.Aggregators;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors.StatArb
{
    public class PairTradeStrategy
    {
        private readonly Dictionary<string, BarAggregator> _aggregatorMap;

        private readonly AlpacaService _alpacaService;
        private readonly ILogger<PairTradeStrategy> _logger;

        private BarTimeFrame _barTimeFrame;
        private int _lookback;

        public PairTradeStrategy(AlpacaService alpacaService, ILogger<PairTradeStrategy> logger)
        {
            _alpacaService = alpacaService;
            _logger = logger;
            _aggregatorMap = new();
        }

        public async Task Initialize(string tickerX, string tickerY, DateTime startDate, BarTimeFrame timeFrame, int lookback, CancellationToken cancellationToken)
        {
            _barTimeFrame = timeFrame;
            _lookback = lookback;

            _aggregatorMap[tickerX] = new BarAggregator();
            _aggregatorMap[tickerY] = new BarAggregator();
            _aggregatorMap[tickerX].SetMinuteInterval(_barTimeFrame.Value);
            _aggregatorMap[tickerY].SetMinuteInterval(_barTimeFrame.Value);

            await _alpacaService.InitializeDataStreamingClient();

            var tickerXBarSubscription = _alpacaService.AlpacaDataStreamingClient.GetMinuteBarSubscription(tickerX);
            tickerXBarSubscription.Received += OnMinuteBar;

            var tickerYBarSubscription = _alpacaService.AlpacaDataStreamingClient.GetMinuteBarSubscription(tickerY);
            tickerYBarSubscription.Received += OnMinuteBar;

            var lastUpdate = (int)_aggregatorMap[tickerX].Round(DateTime.UtcNow.Minute);
            _aggregatorMap[tickerX].SetLastUpdateMinute(lastUpdate);
            _aggregatorMap[tickerY].SetLastUpdateMinute(lastUpdate);

            _aggregatorMap[tickerX].OnBar += OnIntervalBar;
            _aggregatorMap[tickerY].OnBar += OnIntervalBar;

            await _alpacaService.AlpacaDataStreamingClient.SubscribeAsync(tickerXBarSubscription);
            await _alpacaService.AlpacaDataStreamingClient.SubscribeAsync(tickerYBarSubscription);
        }

        public void OnMinuteBar(IBar bar)
        {
            _aggregatorMap[bar.Symbol].OnMinuteBar(bar);
        }

        public void OnIntervalBar(IAggregateBar bar)
        {
            _logger.LogInformation($"{bar.Symbol} received: OHLC: ({bar.Open},{bar.High},{bar.Low},{bar.Close}), Time: {bar.TimeUtc}");
        }
    }
}
