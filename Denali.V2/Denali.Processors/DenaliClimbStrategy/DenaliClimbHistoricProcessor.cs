using Alpaca.Markets;
using Denali.Models;
using Denali.Services;
using Denali.Shared.Extensions;
using Denali.TechnicalAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace Denali.Processors.DenaliClimbStrategy
{
    public class DenaliClimbHistoricProcessor
    {
        private readonly DenaliClimbProcessor _processor;
        private readonly DataLayerComponent _dataLayer;
        private readonly DenaliClimbStrategySettings _settings;
        private readonly ILogger _logger;

        private List<DenaliClimbEntrySignal> _entrySignals;

        public DenaliClimbHistoricProcessor(
            DataLayerComponent dataLayer,
            GapUpScreener screener,
            IOptions<DenaliClimbStrategySettings> settings,
            ILogger<DenaliClimbProcessor> logger,
            ILogger<DenaliClimbHistoricProcessor> historicLogger)
        { 
            _dataLayer = dataLayer;
            _settings = settings.Value;
            _logger = historicLogger;
            _processor = new DenaliClimbProcessor(_dataLayer, screener, settings, logger);
            _processor.OnEntryAction = OnEntry; // Override real processors OnEntry
            _entrySignals = new();
        }

        public async Task ProcessRange(DateTime startDate, DateTime endDate, CancellationToken stoppingToken)
        {
            _logger.LogInformation($"HISTORIC RUN from {startDate.ToShortDateString()} to {endDate.ToShortDateString()}");
            await _processor.Initialize();

            var pastDays = await _dataLayer.GetPastMarketDays(startDate.AddDays(-1), 4);
            var forwardDays = await _dataLayer.GetMarketDays(startDate, endDate);
            List<IIntervalCalendar> marketDays = new() { pastDays.Last() };
            marketDays.AddRange(forwardDays);

            for (int i = 1; i < marketDays.Count; i++)
            {
                await ProcessDay(marketDays[i - 1], marketDays[i], _processor.AllTradableAssets);
                var entrySignals = _entrySignals.Where(x => x.SignalBar.TimeUtc.Date == marketDays[i].GetTradingOpenTimeUtc().Date);
                await ProcessEntrySignals(entrySignals, marketDays[i]);
            }
        }

        public async Task ProcessDay(IIntervalCalendar previousMarketDay, IIntervalCalendar currentMarketDay, List<IAsset> assets)
        {
            await _processor.OnScreenStart(
                currentMarketDay.GetTradingOpenTimeUtc().AddMinutes(_settings.AfterMarketOpenStartTimeMinutes), 
                previousMarketDay, 
                currentMarketDay, 
                assets);
        }

        public async Task ProcessEntrySignals(IEnumerable<DenaliClimbEntrySignal> entrySignals, IIntervalCalendar marketDay)
        {
            var quoteData = await _dataLayer.GetQuotes(
                entrySignals.Select(x => x.SignalBar.Symbol),
                marketDay.GetTradingOpenTimeUtc(),
                marketDay.GetTradingCloseTimeUtc());

            decimal totalProfit = 0;
            decimal totalInvestment = 0;
            foreach (var signal in entrySignals)
            {
                var symbol = signal.SignalBar.Symbol;
                var percentage = signal.GapUpPercentage;

                var quotes = quoteData[symbol];
                signal.EntryPrice = quotes.First().AskPrice;
                signal.TakeProfit = signal.EntryPrice + 0.10m;
                signal.StopLoss = signal.EntryPrice - 0.10m;

                var takeProfit = signal.TakeProfit;
                var entryPrice = signal.EntryPrice;
                var stopLoss = signal.StopLoss;

                totalInvestment += entryPrice;

                var takeProfitQuote = quotes.FirstOrDefault(x => x.BidPrice >= signal.TakeProfit);
                var stopLossQuote = quotes.FirstOrDefault(x => x.BidPrice <= signal.StopLoss);

                string result = "";
                decimal profit = 0;
                if (takeProfitQuote != null && stopLossQuote == null)
                {
                    result = "WIN";
                    profit = takeProfit - entryPrice;
                    totalProfit += profit;
                }
                else if (takeProfitQuote == null && stopLossQuote != null)
                {
                    result = "LOSS";
                    profit = entryPrice - stopLoss;
                    totalProfit -= profit;
                }
                else if (takeProfitQuote != null && stopLossQuote != null)
                {
                    if (takeProfitQuote.TimestampUtc == stopLossQuote.TimestampUtc)
                    {
                        result = "INDETERMINATE (same bar)";
                    }
                    else if (takeProfitQuote.TimestampUtc < stopLossQuote.TimestampUtc)
                    {
                        result = "WIN";
                        profit = takeProfit - entryPrice;
                        totalProfit += profit;
                    }
                    else
                    {
                        result = "LOSS";
                        profit = entryPrice - stopLoss;
                        totalProfit -= profit;
                    }
                }
                else if (takeProfitQuote == null && stopLossQuote == null)
                {
                    result = "INDETERMINATE (forced close)";
                    var exitQuote = quotes.FirstOrDefault(x => x.TimestampUtc >= marketDay.GetTradingCloseTimeUtc().AddMinutes(-10));
                    profit = exitQuote.BidPrice - entryPrice;
                    totalProfit += profit;
                }
                else
                {
                    throw new ArgumentException("How did we get here?");
                }

                _logger.LogInformation($"{symbol} {percentage}% {result} {profit}");
            }

            _logger.LogInformation($"Total profit of {totalProfit} with {totalInvestment} risked for a gain of {ChangePercentage.Calculate(totalInvestment, totalInvestment + totalProfit).RoundToMoney()}%");
        }

        public async Task OnEntry(DenaliClimbEntrySignal entrySignal)
        {
            _entrySignals.Add(entrySignal);
            _logger.LogInformation($"{entrySignal.SignalBar.Symbol} - {entrySignal.GapUpPercentage}%");
        }
        /*
        public async Task ProcessDate(DateTime date, CancellationToken stoppingToken)
        {
            await _processor.Process(date, stoppingToken);
            await _processor.StartTimeScheduledTask.InvokeManual();

            var dataStartTime = _processor.CurrentMarketDay.GetTradingOpenTimeUtc().AddMinutes(_settings.AfterMarketOpenStartTimeMinutes);
            var dataEndTime = _processor.CurrentMarketDay.GetTradingCloseTimeUtc();

            var streamData = await _dataLayer.GetAggregateDataMulti(
                _processor.ScreenedAssets.Select(x => x.Symbol),
                dataStartTime.AddMinutes(1), // historic run, so the first bar was already included in the pre-start bars
                dataEndTime,
                BarTimeFrame.Minute
            );

            _entrySignals = new List<DenaliClimbEntrySignal>();
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

            foreach (var entrySignal in _entrySignals)
            {
                var aggregates = _streamer.StreamedData[entrySignal.SignalBar.Symbol];
                var stopLossBar = aggregates.Where(x => x.TimeUtc > entrySignal.SignalBar.TimeUtc && x.Low <= entrySignal.StopLoss).FirstOrDefault();
                var takeProfitBar = aggregates.Where(x => x.TimeUtc > entrySignal.SignalBar.TimeUtc && x.High >= entrySignal.TakeProfit).FirstOrDefault();

                string wonOrLoss = "";
                if (takeProfitBar != null & stopLossBar != null)
                {
                    if (takeProfitBar.TimeUtc < stopLossBar.TimeUtc)
                        wonOrLoss = "WON";
                    else
                        wonOrLoss = "LOSS";
                }
                else if (takeProfitBar != null && stopLossBar == null)
                    wonOrLoss = "WON";
                else if (takeProfitBar == null && stopLossBar != null)
                    wonOrLoss = "LOSS";
                else if (takeProfitBar == null && stopLossBar == null)
                {
                    if (aggregates.Last().Close > entrySignal.SignalBar.Close)
                    {
                        wonOrLoss = "WON";
                    }
                    else
                    {
                        wonOrLoss = "LOSS";
                    }
                }

                _logger.LogInformation($"{entrySignal.SignalBar.Symbol}: First pullback at {entrySignal.FirstPullbackTime.ToString("HH:mm")}. Opening range high {entrySignal.OpeningRangeHigh}. Broke high at {entrySignal.OpeningRangeBreakoutTime.ToString("HH:mm")}. Confirmation pullback at {entrySignal.ConfirmationPullbackTime.ToString("HH:mm")}. Entry signal at {entrySignal.SignalBar.TimeUtc.ToString("HH:mm")} {wonOrLoss}");

            }
        }
        */
    }
}
