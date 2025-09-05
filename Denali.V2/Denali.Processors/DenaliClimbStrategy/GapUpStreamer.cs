using Alpaca.Markets;
using Denali.Services;
using Denali.Shared.Extensions;
using Denali.TechnicalAnalysis;
using Microsoft.Extensions.Logging;

namespace Denali.Processors.DenaliClimbStrategy
{
    public class GapUpStreamer
    {
        public Dictionary<string, List<IBar>> StreamedData;
        public Dictionary<string, ExponentialMovingAverage> SlowEMA;
        public Dictionary<string, ExponentialMovingAverage> FastEMA;
        public Dictionary<string, decimal> High;

        private readonly DataLayerComponent _dataLayer;
        private readonly IDateTimeService _dateTimeService;
        private readonly DenaliClimbStrategySettings _settings;
        private readonly ILogger _logger;

        public GapUpStreamer(DataLayerComponent dataLayer, DenaliClimbStrategySettings settings, IDateTimeService dateTimeService, ILogger logger)
        {
            _dataLayer = dataLayer;
            _dateTimeService = dateTimeService;
            _settings = settings;
            _logger = logger;
        }

        public async Task SubscribeToTickerStream(List<IAsset> assets, DateTime startTime, DateTime marketOpenTime)
        {
            StreamedData = new();
            SlowEMA = new();
            FastEMA = new();
            High = new();

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

        private void OnStreamedData(IBar bar)
        {
            StreamedData[bar.Symbol].Append(bar);

            var aggregates = StreamedData[bar.Symbol];
            SlowEMA[bar.Symbol].Analyze(aggregates);
            FastEMA[bar.Symbol].Analyze(aggregates);

            var fastEmas = FastEMA[bar.Symbol].MovingAverages;
            var slowEmas = SlowEMA[bar.Symbol].MovingAverages;
            var high = High[bar.Symbol];

            var stackedEmas = fastEmas.Count > 2 && slowEmas.Count > 2 &&
                fastEmas.GetHistoricValue(0) > slowEmas.GetHistoricValue(0) &&
                fastEmas.GetHistoricValue(1) > slowEmas.GetHistoricValue(1);

            var priceAction = aggregates.GetHistoricValue(0).Close > aggregates.GetHistoricValue(1).Close;

            var breakout = bar.Close > high;

            if (stackedEmas && priceAction && breakout)
            {
                _logger.LogInformation($"Entry signal for {bar.Symbol} at {bar.TimeUtc}");
            }
        }
    }
}
