using Alpaca.Markets;
using Denali.Services;
using Denali.Shared.Extensions;
using Denali.TechnicalAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public class MomentumScreen
    {
        private readonly DataLayerComponent _dataLayer;
        private readonly ILogger _logger;

        public MomentumScreen(DataLayerComponent dataLayer, ILogger<MomentumScreen> logger)
        {
            _dataLayer = dataLayer;
            _logger = logger;
        }

        public async Task ProcessRange(DateTime start, DateTime end)
        {
            var marketDay = await _dataLayer.GetMarketDays(start, end);
            var assets = await _dataLayer.GetAllTradableAssets();
            assets = assets.Where(x => x.Shortable).ToList();
            decimal totalProfit = 0m;

            foreach (var day in marketDay)
            {
                decimal dayProfit = 0m;
                _logger.LogInformation($"{day.GetTradingOpenTimeUtc().ToShortDateString()}");
                var data = await _dataLayer.GetAggregateDataMulti(assets.Select(x => x.Symbol), day.GetTradingOpenTimeUtc(), day.GetTradingCloseTimeUtc(), new BarTimeFrame(15, BarTimeFrameUnit.Minute));
                foreach (var ticker in data)
                {
                    var tickerData = ticker.Value;
                    for (var i = 0; i < tickerData.Count; i++)
                    {
                        var currentItem = tickerData[i];
                        if (ChangePercentage.Calculate(currentItem.Open, currentItem.Close) > 5 && currentItem.Open > 5 && currentItem.Open <= 300)
                        {
                            if (i + 1 < tickerData.Count)
                            {
                                var nextItem = tickerData[i + 1];
                                var profit = (nextItem.Open - nextItem.Close) * (1000 / nextItem.Open) - (0.70m);
                                profit = profit.RoundToMoney();
                                dayProfit += profit;
                                _logger.LogInformation($"{currentItem.Symbol}@{nextItem.Open} {currentItem.TimeUtc} {profit}");
                                break;
                            }
                        }
                    }
                }

                totalProfit += dayProfit;
                _logger.LogInformation($"Daily Profit: {dayProfit}");
            }

            _logger.LogInformation($"Total Profit: {totalProfit}");
        }
    }
}
