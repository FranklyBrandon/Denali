using Alpaca.Markets;
using Denali.Services;
using Denali.Services.Extensions;
using Denali.Shared.Extensions;
using Denali.TechnicalAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Denali.Processors.DenaliClimbStrategy
{
    public record EntrySignal
    {
        public decimal StopLoss { get; set; }
        public decimal TakeProfit { get; set; }
        public bool BrokeHigh { get; set; } = false;
        public DateTime BrokeHighTime { get; set; }
        public bool Pullback { get; set; } = false;
        public DateTime PullbackDateTime { get; set; }
        public bool Signal { get; set; } = false;
        public IBar SignalBar { get; set; }
    }
    public class GapUpStreamer
    {
        public Dictionary<string, List<IBar>> StreamedData;
        public Dictionary<string, ExponentialMovingAverage> SlowEMA;
        public Dictionary<string, ExponentialMovingAverage> FastEMA;
        public Dictionary<string, decimal> High;
        public Dictionary<string, EntrySignal> EntryProps;

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
            EntryProps = new();

            _logger.LogInformation("Fetching historic data...");
            StreamedData = await _dataLayer.GetAggregateDataMulti(assets.Select(x => x.Symbol), marketOpenTime, startTime, BarTimeFrame.Minute);

            foreach (var asset in assets)
            {
                SlowEMA[asset.Symbol] = new ExponentialMovingAverage(_settings.SlowEMABacklog);
                FastEMA[asset.Symbol] = new ExponentialMovingAverage(_settings.FastEMABacklog);

                SlowEMA[asset.Symbol].Analyze(StreamedData[asset.Symbol]);
                FastEMA[asset.Symbol].Analyze(StreamedData[asset.Symbol]);
                High[asset.Symbol] = StreamedData[asset.Symbol].Max(x => x.High);
                EntryProps[asset.Symbol] = new();
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

            if (fastEmas.Count < 2 || slowEmas.Count < 2)
                return;

            if (!EntryProps[bar.Symbol].BrokeHigh && bar.Close >= High[bar.Symbol])
            {
                EntryProps[bar.Symbol].BrokeHigh = true;
                EntryProps[bar.Symbol].BrokeHighTime = bar.TimeUtc;
                return;
            }

            if (!EntryProps[bar.Symbol].Pullback && EntryProps[bar.Symbol].BrokeHigh && 
                fastEmas.Last().Value < slowEmas.Last().Value)
            {
                EntryProps[bar.Symbol].Pullback = true;
                EntryProps[bar.Symbol].PullbackDateTime = bar.TimeUtc;
                return;
            }

            if (!EntryProps[bar.Symbol].Signal &&
                EntryProps[bar.Symbol].BrokeHigh &&
                EntryProps[bar.Symbol].Pullback &&
                fastEmas.GetHistoricValue(0).Value > slowEmas.GetHistoricValue(0).Value && // Most recent fast EMA should be above slow ema
                fastEmas.GetHistoricValue(1).Value >= slowEmas.GetHistoricValue(1).Value && // Penultimate can be equal to or greater
                aggregates.GetHistoricValue(0).IsGreen() && aggregates.GetHistoricValue(1).IsGreen()
                )
            {
                EntryProps[bar.Symbol].Signal = true;
                EntryProps[bar.Symbol].SignalBar = bar;
                await OnEntryAction(EntryProps[bar.Symbol]);
            }
        }
    }
}
