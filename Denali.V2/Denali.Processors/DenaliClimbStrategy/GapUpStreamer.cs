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
                //SlowEMA[asset] = new ExponentialMovingAverage(_settings.SlowEMABacklog);
                //FastEMA[asset] = new ExponentialMovingAverage(_settings.FastEMABacklog);

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

           
        }

        private static decimal GetStopLoss(IList<EMA> fastEmas, IList<EMA> slowEmas, List<IBar> aggregates)
        {
            EMA downTrendEnd = null;

            for (int i = 0; i < slowEmas.Count; i++)
            {
                var fastEma = fastEmas.GetHistoricValue(i);
                var slowEma = slowEmas.GetHistoricValue(i);

                if (downTrendEnd is null && fastEma.Value < slowEma.Value)
                {
                    downTrendEnd = slowEma;
                }

                if (downTrendEnd is not null && fastEma.Value > slowEma.Value)
                {
                    return aggregates.Where(x => x.TimeUtc >= slowEma.TimeUtc && x.TimeUtc <= downTrendEnd.TimeUtc).Min(x => x.Low);
                }
            }

            throw new ArithmeticException("No stoploss calclated");
        }
    }
}
