using Alpaca.Markets;
using AutoMapper;
using Denali.Models.Alpaca;
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

            

            for (int i = 1; i < marketDays.Count; i++)
            {
              


            }

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

        public async Task<Tuple<decimal, decimal>> ProcessBacklook(IIntervalCalendar previousMarketDay, IIntervalCalendar currentMarketDay)
        {
            var backlookData = await _dataLayer.GetAggregateDataMulti(new List<string> { "VTI" }, previousMarketDay.GetTradingOpenTimeUtc(), previousMarketDay.GetTradingCloseTimeUtc(), BarTimeFrame.Minute);
            var titForTatProfit = TitForTat(backlookData["VTI"].ToList());
            var crissCrossProfit = CrissCross(backlookData["VTI"].ToList());
            _logger.LogInformation($"TitForTat {titForTatProfit}, CrissCross {crissCrossProfit}");

            var forwardData = await _dataLayer.GetAggregateDataMulti(new List<string> { "VTI" }, currentMarketDay.GetTradingOpenTimeUtc(), currentMarketDay.GetTradingCloseTimeUtc(), BarTimeFrame.Minute);
            var baseLine = forwardData["VTI"].Last().Close - forwardData["VTI"].First().Open;
            if (titForTatProfit > crissCrossProfit)
            {
                var profit = TitForTat(forwardData["VTI"].ToList());
                _logger.LogInformation($"Forward profit TitForTat {profit}");
                return Tuple.Create(profit, baseLine);
            }
            else
            {
                var profit = CrissCross(forwardData["VTI"].ToList());
                _logger.LogInformation($"Forward profit CrissCross {profit}");
                return Tuple.Create(profit, baseLine);
            }
        }


        private decimal TitForTat(List<IBar> data)
        {
            decimal profit = 0;
            bool green = data[0].IsGreen();
            int kelly = 1;
            for (int i = 1; i < data.Count; i++)
            {
                var bar = data[i];
                if (green)
                {
                    profit += (bar.Close - bar.Open) * kelly;
                }
                else
                {
                    profit += (bar.Open - bar.Close) * kelly;
                }

                green = bar.IsGreen();
                if (profit > 0)
                {
                    kelly++;
                }
                else
                {
                    kelly = 1;
                }
            }
            return profit;
        }

        private decimal CrissCross(List<IBar> data)
        {
            decimal profit = 0;
            bool green = data[0].IsGreen();
            int kelly = 1;
            for (int i = 1; i < data.Count; i++)
            {
                var bar = data[i];
                if (green)
                {
                    profit += (bar.Open - bar.Close) * kelly;
                }
                else
                {
                    profit += (bar.Close - bar.Open) * kelly;
                }
                green = bar.IsGreen();
                if (profit > 0)
                {
                    kelly++;
                }
                else
                {
                    kelly = 1;
                }
            }
            return profit;
        }

        public async Task OnEntry(IEnumerable<DenaliDescentEntrySignal> signals)
        {
            _signals = signals;
        }
    }
}
