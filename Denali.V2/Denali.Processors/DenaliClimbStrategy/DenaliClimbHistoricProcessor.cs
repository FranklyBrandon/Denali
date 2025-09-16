using Alpaca.Markets;
using Denali.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Denali.Processors.DenaliClimbStrategy
{
    public class DenaliClimbHistoricProcessor
    {
        private readonly DenaliClimbProcessor _processor;
        private readonly DataLayerComponent _dataLayer;
        private readonly GapUpStreamer _streamer;
        private readonly DenaliClimbStrategySettings _settings;
        private readonly ILogger _logger;

        private List<EntrySignal> _entrySignals;

        public DenaliClimbHistoricProcessor(
            DataLayerComponent dataLayer,
            GapUpScreener screener,
            GapUpStreamer streamer,
            TradeManager tradeManager,
            IOptions<DenaliClimbStrategySettings> settings,
            ILogger<DenaliClimbProcessor> logger,
            ILogger<DenaliClimbHistoricProcessor> historicLogger)
        { 
            _dataLayer = dataLayer;
            _streamer = streamer;
            _settings = settings.Value;
            _logger = historicLogger;
            _processor = new DenaliClimbProcessor(_dataLayer, screener, _streamer, tradeManager, settings, logger);
            _streamer.OnEntryAction = OnEntry; // Override real processors OnEntry
        }

        public async Task ProcessRange(DateTime startDate, DateTime endDate, CancellationToken stoppingToken)
        {
            _logger.LogInformation($"HISTORIC RUN from {startDate.ToShortDateString()} to {endDate.ToShortDateString()}");
            await _processor.Initialize();

            var daySpan = (endDate - startDate).Days;
            for (int i = 0; i <= daySpan; i++)
            {
                await ProcessDate(startDate.AddDays(i), stoppingToken);
            }

        }
        public async Task ProcessDate(DateTime date, CancellationToken stoppingToken)
        {
            await _processor.Process(date, stoppingToken);
            await _processor.StartTimeScheduledTask.InvokeManual();

            var dataStartTime = _processor.CurrentMarketDay.GetTradingOpenTimeUtc().AddMinutes(_settings.AfterMarketOpenBufferMinutes);
            var dataEndTime = _processor.CurrentMarketDay.GetTradingCloseTimeUtc();

            var streamData = await _dataLayer.GetAggregateDataMulti(
                _processor.ScreenedAssets.Select(x => x.Symbol),
                dataStartTime.AddMinutes(1), // historic run, so the first bar was already included in the pre-start bars
                dataEndTime,
                BarTimeFrame.Minute
            );

            _entrySignals = new List<EntrySignal>();
            var totalMinutes = (dataEndTime - dataStartTime).TotalMinutes;
            for (int i = 0; i < totalMinutes; i++)
            {
                var time = dataStartTime.AddMinutes(i);
                var dataFrames = streamData.SelectMany(x => x.Value.Where(x => x.TimeUtc == time));

                foreach (var frame in dataFrames)
                {
                    _streamer.OnStreamedData(frame);
                }
            }

            /*
            var orderedEntries = _entrySignals.OrderBy(x => x.Bar.TimeUtc);
            foreach (var entry in orderedEntries)
            {
                var bars = _streamer.StreamedData[entry.Bar.Symbol];
                var profitBar = bars.FirstOrDefault(x => x.TimeUtc > entry.Bar.TimeUtc.AddMinutes(1) && x.High >= entry.TakeProfit);
                var stopBar = bars.FirstOrDefault(x => x.TimeUtc > entry.Bar.TimeUtc.AddMinutes(1) && x.Low <= entry.StopLoss);
                var timeStr = entry.Bar.TimeUtc.ToString("HH:mm");
                var symbol = entry.Bar.Symbol;
                var details = $"[Stop: {entry.StopLoss}, Profit: {entry.TakeProfit}]";

                if (stopBar == null && profitBar == null)
                {
                    _logger.LogInformation($"Entry Signal {symbol} at {timeStr} {details}: INDETERMINATE : No exit");
                    continue;
                }

                if (stopBar == null)
                {
                    _logger.LogInformation($"Entry Signal {symbol} at {timeStr} {details}: TAKEPROFIT at {profitBar.TimeUtc:HH:mm}");
                    continue;
                }

                if (profitBar == null)
                {
                    _logger.LogInformation($"Entry Signal {symbol} at {timeStr} {details}: STOPLOSS at {stopBar.TimeUtc:HH:mm}");
                    continue;
                }

                // Both stopBar and profitBar are not null
                if (stopBar.TimeUtc == profitBar.TimeUtc)
                {
                    _logger.LogInformation($"Entry Signal {symbol} at {timeStr} {details}: INDETERMINATE : Same bar exit");
                }
                else if (stopBar.TimeUtc < profitBar.TimeUtc)
                {
                    _logger.LogInformation($"Entry Signal {symbol} at {timeStr} {details}: STOPLOSS at {stopBar.TimeUtc:HH:mm}");
                }
                else
                {
                    _logger.LogInformation($"Entry Signal {symbol} at {timeStr} {details}: TAKEPROFIT at {profitBar.TimeUtc:HH:mm}");
                }
            }
            */
        }

        public async Task OnEntry(EntrySignal entrySignal)
        {
            _entrySignals.Add(entrySignal);
            //_logger.LogInformation($"Entry Signal for {entrySignal.Bar.Symbol}");
        }
    }
}
