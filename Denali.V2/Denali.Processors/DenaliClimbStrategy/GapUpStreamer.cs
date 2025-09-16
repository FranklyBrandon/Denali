using Alpaca.Markets;
using Denali.Services;
using Denali.Shared.Extensions;
using Denali.TechnicalAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Denali.Processors.DenaliClimbStrategy
{
    public record EntrySignal(decimal StopLoss, decimal TakeProfit, IBar Bar);

    public class GapUpStreamer
    {
        public Dictionary<string, List<IBar>> StreamedData;
        public Dictionary<string, ExponentialMovingAverage> SlowEMA;
        public Dictionary<string, ExponentialMovingAverage> FastEMA;
        public Dictionary<string, decimal> High;
        public HashSet<string> PastHigh;

        public delegate Task OnEntry(EntrySignal entrySignal);
        public OnEntry OnEntryAction { get; set; }

        private readonly HashSet<string> _entrySignals;
        private readonly DataLayerComponent _dataLayer;
        private readonly DenaliClimbStrategySettings _settings;
        private readonly ILogger _logger;

        public GapUpStreamer(DataLayerComponent dataLayer, IOptions<DenaliClimbStrategySettings> settings, ILogger<GapUpStreamer> logger)
        {
            _dataLayer = dataLayer;
            _settings = settings.Value;
            _logger = logger;

            _entrySignals = new();
        }

        public async Task SubscribeToTickerStream(List<IAsset> assets, DateTime startTime, DateTime marketOpenTime)
        {
            StreamedData = new();
            SlowEMA = new();
            FastEMA = new();
            High = new();
            PastHigh = new();

            _logger.LogInformation("Fetching historic data...");
            StreamedData = await _dataLayer.GetAggregateDataMulti(assets.Select(x => x.Symbol), marketOpenTime, startTime, BarTimeFrame.Minute);

            foreach (var asset in assets)
            {
                SlowEMA[asset.Symbol] = new ExponentialMovingAverage(_settings.SlowEMABacklog);
                FastEMA[asset.Symbol] = new ExponentialMovingAverage(_settings.FastEMABacklog);

                SlowEMA[asset.Symbol].Analyze(StreamedData[asset.Symbol]);
                FastEMA[asset.Symbol].Analyze(StreamedData[asset.Symbol]);
                High[asset.Symbol] = StreamedData[asset.Symbol].Max(x => x.High);
            }
            
            _logger.LogInformation("Finished loading data");

            _logger.LogInformation("Subscribing to streams");
            var subscription = await _dataLayer.SubscribeMinuteBar(assets.Select(x => x.Symbol));
            subscription.Received += OnStreamedData;
            _logger.LogInformation("Successfully subscribed to streams");
        }

        public async void OnStreamedData(IBar bar)
        {
            StreamedData[bar.Symbol] = StreamedData[bar.Symbol].Append(bar).ToList();

            var aggregates = StreamedData[bar.Symbol];
            SlowEMA[bar.Symbol].Analyze(aggregates);
            FastEMA[bar.Symbol].Analyze(aggregates);

            var fastEmas = FastEMA[bar.Symbol].MovingAverages;
            var slowEmas = SlowEMA[bar.Symbol].MovingAverages;
            var high = High[bar.Symbol];

            if (bar.Close >= high && PastHigh.Add(bar.Symbol))
            {
                _logger.LogInformation($"{bar.Symbol} broke high at {bar.TimeUtc.ToString("HH:mm")}");
            }
        }
    }
}
