using Alpaca.Markets;
using Denali.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public class TimeofDayProcessor
    {
        private readonly DataLayerComponent _dataLayer;
        private readonly ILogger _logger;
        private const int AFTER_OPEN_MINUTES = 30;
        private const int BEFORE_CLOSE_MINUTES = 10;

        public TimeofDayProcessor(DataLayerComponent dataLayer, ILogger<TimeofDayProcessor> logger)
        {
            _dataLayer = dataLayer;
            _logger = logger;
        }

        public async Task ProcessRange(DateTime start, DateTime end)
        {
            var marketDay = await _dataLayer.GetMarketDays(start, end);
            var baseLine = await GetBaseLine(marketDay);
            decimal totalProfit = 0m;
            foreach (var day in marketDay)
            {
                var response = await _dataLayer.GetAggregateDataMulti(new List<string> { "VTI" }, day.GetTradingOpenTimeUtc(), day.GetTradingCloseTimeUtc(), new BarTimeFrame(1, BarTimeFrameUnit.Minute));
                var data = response["VTI"];

                decimal profit = 0m;
                var entryTime = day.GetTradingOpenTimeUtc().AddMinutes(AFTER_OPEN_MINUTES);
                var exitTime = day.GetTradingCloseTimeUtc().AddMinutes(-BEFORE_CLOSE_MINUTES);

                var firstBar = data.First();
                var signalBar = data.Where(x => x.TimeUtc >= entryTime).FirstOrDefault();
                var entryBar = data.Where(x => x.TimeUtc > signalBar.TimeUtc).FirstOrDefault();
                var closingBar = data.Where(x => x.TimeUtc >= exitTime).FirstOrDefault();
                var openPositionBars = data.Where(x => x.TimeUtc > entryTime && x.TimeUtc < exitTime);

                bool longPosition = signalBar.Close > firstBar.Close;
                var entryPrice = entryBar.Close;
                var closePrice = closingBar.Close;

                bool exited = false;
                decimal stopLoss = 0.10m;
                decimal takeProfit = 1m;
                foreach (var bar in openPositionBars)
                {
                    if (longPosition)
                    {
                        if (bar.Close <= entryPrice - stopLoss)
                        {
                            exited = true;
                            profit -= stopLoss;
                            break;
                        }

                        if (bar.Close >= entryPrice + takeProfit)
                        {
                            exited = true;
                            profit += takeProfit;
                            break;

                        }
                    }
                    else
                    {
                        if (bar.Close >= entryPrice + stopLoss)
                        {
                            exited = true;
                            profit -= stopLoss;
                            break;
                        }

                        if (bar.Close <= entryPrice - takeProfit)
                        {
                            exited = true;
                            profit += takeProfit;
                            break;
                        }
                    }
                }

                if (!exited)
                {
                    if (longPosition)
                    {
                        profit += closePrice - entryPrice;
                    }
                    else
                    {
                        profit += entryPrice - closePrice;
                    }
                }

                _logger.LogInformation($"{day.GetTradingOpenTimeUtc().ToShortDateString()} {profit}");
                totalProfit += profit;
            }

            _logger.LogInformation($"Baseline: {baseLine}");
            _logger.LogInformation($"TOTAL PROFIT: {totalProfit}");
        }

        private async Task<decimal> GetBaseLine(IEnumerable<IIntervalCalendar> days)
        {
            var start = days.FirstOrDefault();
            var end = days.LastOrDefault();

            var startData = await _dataLayer.GetAggregateDataMulti(new List<string> { "VTI" }, start.GetTradingOpenTimeUtc().AddDays(-1), start.GetTradingCloseTimeUtc(), BarTimeFrame.Day);
            var endData = await _dataLayer.GetAggregateDataMulti(new List<string> { "VTI" }, end.GetTradingOpenTimeUtc().AddDays(-1), end.GetTradingCloseTimeUtc(), BarTimeFrame.Day);

            return endData["VTI"].First().Close - startData["VTI"].First().Open;
        }
    }
}
