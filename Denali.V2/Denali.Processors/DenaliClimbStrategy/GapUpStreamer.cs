using Alpaca.Markets;
using Denali.Models;
using Denali.Services;
using Denali.Services.Extensions;
using Denali.Shared.Extensions;
using Denali.TechnicalAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Denali.Processors.DenaliClimbStrategy
{
    public class GapUpStreamer
    {
        public Dictionary<string, List<IBar>> StreamedData;
        public Dictionary<string, ExponentialMovingAverage> SlowEMA;
        public Dictionary<string, ExponentialMovingAverage> FastEMA;
        public Dictionary<string, decimal> High;
        public Dictionary<string, DenaliClimbEntrySignal> EntryProps;

        public delegate Task OnEntry(DenaliClimbEntrySignal entrySignal);
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
            await InitializeMetrics(assets.Select(x => x.Symbol).ToList(), startTime, marketOpenTime);

            _logger.LogInformation("Subscribing to streams");
            var subscription = await _dataLayer.SubscribeMinuteBar(assets.Select(x => x.Symbol));
            subscription.Received += OnStreamedData;
            _logger.LogInformation("Successfully subscribed to streams");
        }

        public async Task InitializeMetrics(List<string> assets, DateTime startTime, DateTime marketOpenTime)
        {
            StreamedData = new();
            SlowEMA = new();
            FastEMA = new();
            High = new();
            EntryProps = new();

            _logger.LogInformation("Fetching historic data...");
            StreamedData = await _dataLayer.GetAggregateDataMulti(assets, marketOpenTime, startTime, BarTimeFrame.Minute);

            foreach (var asset in assets)
            {
                SlowEMA[asset] = new ExponentialMovingAverage(_settings.SlowEMABacklog);
                FastEMA[asset] = new ExponentialMovingAverage(_settings.FastEMABacklog);

                SlowEMA[asset].AnalyzeAll(StreamedData[asset]);
                FastEMA[asset].AnalyzeAll(StreamedData[asset]);
                High[asset] = StreamedData[asset].Max(x => x.High);
                EntryProps[asset] = new();
            }

            _logger.LogInformation("Finished loading data");
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

            var entryProps = EntryProps[bar.Symbol];

            // Entry Signal
            if (!entryProps.Signal && entryProps.FirstPullback && entryProps.OpeningRangeBreakout && entryProps.ConfirmationPullback)
            {
                if (
                    fastEmas.GetHistoricValue(0).Value > slowEmas.GetHistoricValue(0).Value && // Most recent fast EMA should be above slow ema
                    fastEmas.GetHistoricValue(1).Value >= slowEmas.GetHistoricValue(1).Value && // Penultimate can be equal to or greater
                    aggregates.GetHistoricValue(0).IsGreen() && aggregates.GetHistoricValue(1).IsGreen()
                    )
                {
                    EntryProps[bar.Symbol].Signal = true;
                    EntryProps[bar.Symbol].SignalBar = bar;
                    await OnEntryAction(EntryProps[bar.Symbol]);
                    return;
                }
            }

            // Confirmation pullback
            if (!entryProps.ConfirmationPullback && entryProps.FirstPullback && entryProps.OpeningRangeBreakout)
            {
                if (fastEmas.Last().Value < slowEmas.Last().Value)
                {
                    entryProps.ConfirmationPullback = true;
                    entryProps.ConfirmationPullbackTime = bar.TimeUtc;
                    return;
                }
            }

            // Opening range break
            if (!entryProps.OpeningRangeBreakout && entryProps.FirstPullback)
            {
                if (bar.Close >= entryProps.OpeningRangeHigh)
                {
                    entryProps.OpeningRangeBreakout = true;
                    entryProps.OpeningRangeBreakoutTime = bar.TimeUtc;
                    return;
                }
            }

            // First pullback
            if (!entryProps.FirstPullback)
            {
                if (fastEmas.Last().Value < slowEmas.Last().Value)
                {
                    entryProps.FirstPullback = true;
                    entryProps.FirstPullbackTime = bar.TimeUtc;
                    entryProps.OpeningRangeHigh = aggregates.Max(x => x.High);
                    return;
                }
            }
        }
    }
}
