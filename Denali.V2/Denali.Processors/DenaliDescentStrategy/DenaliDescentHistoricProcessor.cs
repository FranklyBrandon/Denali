using Alpaca.Markets;
using Denali.Processors.DenaliClimbStrategy;
using Denali.Services;
using Denali.Services.Extensions;
using Denali.Shared.Extensions;
using Denali.TechnicalAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Denali.Processors.DenaliDescentStrategy
{
    public class DenaliDescentHistoricProcessor
    {
        private readonly DenaliDescentProcessor _processor;
        private readonly DataLayerComponent _dataLayer;
        private readonly DenaliDescentStrategySettings _settings;
        private readonly ILogger _logger;

        private IEnumerable<DenaliDescentEntrySignal> _signals;

        public DenaliDescentHistoricProcessor(
            DataLayerComponent dataLayer,
            GapUpScreener gapUpScreener,
            IOptions<DenaliDescentStrategySettings> settings,
            ILogger<DenaliDescentProcessor> logger,
            ILogger<DenaliDescentHistoricProcessor> historicLogger)
        {
            _dataLayer = dataLayer;
            _settings = settings.Value;
            _logger = logger;

            _processor = new DenaliDescentProcessor(_dataLayer, gapUpScreener, settings, logger);
            _processor.OnEntryAction = OnEntry;
        }

        public async Task ProcessRange(DateTime startDate, DateTime endDate, CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Denali Descent HISTORIC RUN from {startDate.ToShortDateString()} to {endDate.ToShortDateString()}");
            await _processor.Initialize();

            var pastDays = await _dataLayer.GetPastMarketDays(startDate.AddDays(-1), 4);
            var forwardDays = await _dataLayer.GetMarketDays(startDate, endDate);
            List<IIntervalCalendar> marketDays = new() { pastDays.Last() };
            marketDays.AddRange(forwardDays);

            decimal profit = 0;
            for (int i = 1; i < marketDays.Count; i++)
            {
                await _processor.OnScreenStart(marketDays[i].GetTradingOpenTimeUtc(), marketDays[i - 1], marketDays[i], _processor.AllTradableAssets);
                //await ProcessSignals(marketDays[i]);
                //profit += await ProcessGreenBars(marketDays[i]);
                //profit += await ProcessRedBars(marketDays[i]);
                //profit += await ProcessSignals(marketDays[i]);
                profit += await ProcessOpen(marketDays[i - 1], marketDays[i]);
            }
            _logger.LogInformation($"TOTAL PROFIT: {profit}");
        }

        public async Task<decimal> ProcessSignals(IIntervalCalendar marketDay)
        {
            var symbols = _signals.Select(x => x.Symbol);

            var entryData = await _dataLayer.GetAggregateDataMulti(symbols, marketDay.GetTradingOpenTimeUtc(), marketDay.GetTradingOpenTimeUtc().AddMinutes(1), BarTimeFrame.Minute);
            var tradeData = await _dataLayer.GetTrades(symbols, marketDay.GetTradingOpenTimeUtc(), marketDay.GetTradingCloseTimeUtc());

            decimal totalProfit = 0;
            decimal totalInvestment = 0;
            int winCount = 0;
            int lossCount = 0;
            foreach (var signal in _signals)
            {
                var symbol = signal.Symbol;
                var percentage = signal.GapUpPercentage;

                var trades = tradeData[symbol];
                if (!entryData.TryGetValue(symbol, out var aggregates))
                    continue;

                var entryPrice = aggregates.First().Open;
                var takeProfit = entryPrice + 0.50m;
                var stopLoss = entryPrice - 0.50m;

                totalInvestment += entryPrice;

                var takeProfitQuote = trades.FirstOrDefault(x => x.Price >= takeProfit);
                var stopLossQuote = trades.FirstOrDefault(x => x.Price <= stopLoss);

                string result = "";
                decimal profit = 0;
                if (takeProfitQuote != null && stopLossQuote == null)
                {
                    result = "WIN";
                    winCount++;
                    profit = Math.Abs(entryPrice - takeProfit);
                    totalProfit += profit;
                }
                else if (takeProfitQuote == null && stopLossQuote != null)
                {
                    result = "LOSS";
                    lossCount++;
                    profit = Math.Abs(entryPrice - stopLoss);
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
                        winCount++;
                        profit = Math.Abs(entryPrice - takeProfit);
                        totalProfit += profit;
                    }
                    else
                    {
                        result = "LOSS";
                        lossCount++;
                        profit = Math.Abs(entryPrice - stopLoss);
                        totalProfit -= profit;
                    }
                }
                else if (takeProfitQuote == null && stopLossQuote == null)
                {
                    result = "INDETERMINATE (forced close)";
                    var exitPrice = trades.Last().Price;
                    profit = exitPrice - entryPrice;
                    if(profit > 0)
                    {
                        result = "WIN";
                        winCount++;
                        totalProfit += profit;
                    }
                    else
                    {
                        result = "LOSS";
                        lossCount++;
                        totalProfit -= profit;
                    }

                }
                else
                {
                    throw new ArgumentException("How did we get here?");
                }

                _logger.LogInformation($"{symbol} {percentage}% {result} {profit}");
            }

            _logger.LogInformation($"Total profit of {totalProfit} with {totalInvestment} risked for a gain of {ChangePercentage.Calculate(totalInvestment, totalInvestment + totalProfit).RoundToMoney()}%");
            _logger.LogInformation($"Wins: {winCount}, Losses: {lossCount}");
            return totalProfit;
         }

        public async Task<decimal> ProcessGreenBars(IIntervalCalendar marketDay)
        {
            var symbols = _signals.Select(x => x.Symbol);
            var data = await _dataLayer.GetAggregateDataMulti(symbols, marketDay.GetTradingOpenTimeUtc().AddDays(-1), marketDay.GetTradingCloseTimeUtc(), BarTimeFrame.Day);

            decimal totalProfit = 0;
            decimal totalInvestment = 0;
            int winCount = 0;
            int lossCount = 0;
            foreach(var symbol in data)
            {
                var bar = symbol.Value.First();
                if(bar.IsGreen())
                {
                    winCount++;
                    totalProfit += bar.Close - bar.Open;
                    totalInvestment += bar.Open;
                }
                else
                {
                    lossCount++;
                    totalProfit -= bar.Open - bar.Close;
                    totalInvestment += bar.Open;
                }
            }

            _logger.LogInformation($"Total profit of {totalProfit} with {totalInvestment} risked for a gain of {ChangePercentage.Calculate(totalInvestment, totalInvestment + totalProfit).RoundToMoney()}%");
            _logger.LogInformation($"Wins: {winCount}, Losses: {lossCount}");
            return totalProfit;
        }

        public async Task<decimal> ProcessRedBars(IIntervalCalendar marketDay)
        {
            var symbols = _signals.Take(100).Select(x => x.Symbol);
            var data = await _dataLayer.GetAggregateDataMulti(symbols, marketDay.GetTradingOpenTimeUtc().AddDays(-1), marketDay.GetTradingCloseTimeUtc(), BarTimeFrame.Day);

            decimal totalProfit = 0;
            decimal totalInvestment = 0;
            int winCount = 0;
            int lossCount = 0;
            foreach (var symbol in data)
            {
                var bar = symbol.Value.First();
                if (!bar.IsGreen())
                {
                    totalInvestment += bar.Open;
                    if (totalInvestment > 25000)
                        continue;

                    winCount++;
                    totalProfit += bar.Open - bar.Close;

                }
                else
                {
                    totalInvestment += bar.Open;
                    if (totalInvestment > 25000)
                        continue;

                    lossCount++;
                    totalProfit -= bar.Close - bar.Open;
                }
            }

            _logger.LogInformation($"Total profit of {totalProfit} with {totalInvestment} risked for a gain of {ChangePercentage.Calculate(totalInvestment, totalInvestment + totalProfit).RoundToMoney()}%");
            _logger.LogInformation($"Wins: {winCount}, Losses: {lossCount}");
            return totalProfit;
        }

        public async Task<decimal> ProcessOpen(IIntervalCalendar previousMarketDay, IIntervalCalendar currentMarketDay)
        {
            var signals = _signals.Take(100);
            var data = await _dataLayer.GetAggregateDataMulti(signals.Select(x => x.Symbol), previousMarketDay.GetTradingCloseTimeUtc(), currentMarketDay.GetTradingOpenTimeUtc().AddMinutes(30), new BarTimeFrame(15, BarTimeFrameUnit.Minute));

            decimal totalProfit = 0;
            decimal totalInvestment = 0;
            foreach (var signal in signals)
            {
                if (totalInvestment >= 25000)
                    continue;

                if (!data.TryGetValue(signal.Symbol, out var bars))
                    continue;

                var bar = bars.Where(x => x.TimeUtc == currentMarketDay.GetTradingOpenTimeUtc().AddMinutes(15)).FirstOrDefault();
                var previousBars = bars.Where(x => x.TimeUtc < currentMarketDay.GetTradingOpenTimeUtc().AddMinutes(15));

                if (bar != null && previousBars.Count() > 1)
                {
                    var entry = bars.Where(x => x.TimeUtc > bar.TimeUtc).FirstOrDefault();
                    if (bar.IsGreen() && bar.Close > previousBars.Max(x => x.High) && entry != null)
                    {
                        _logger.LogInformation($"{signal.Symbol} {signal.GapUpPercentage}%");
                    }
                }             
            }

            _logger.LogInformation($"Total profit of {totalProfit}");
            return totalProfit;
        }

        public async Task OnEntry(IEnumerable<DenaliDescentEntrySignal> signals)
        {
            _signals = signals;
        }
    }
}
