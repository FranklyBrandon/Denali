using Alpaca.Markets;
using AutoMapper;
using Denali.Processors.DenaliClimbStrategy;
using Denali.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public class PreMarketHours
    {
        private readonly DataLayerComponent _dataLayer;
        private readonly GapUpScreener _gapUpScreener;
        private readonly ILogger _logger;

        public PreMarketHours(DataLayerComponent dataLayer, GapUpScreener gapUpScreener, ILogger<PreMarketHours> logger)
        {
            _dataLayer = dataLayer;
            _gapUpScreener = gapUpScreener;
            _logger = logger;
        }

        public async Task Process(DateTime startDate, DateTime endDate, CancellationToken stoppingToken)
        {
            await _dataLayer.Initialize();
            var allTradableAssets = await _dataLayer.GetAllTradableAssets();

            decimal totalProfit = 0m;
            var marketDays = await _dataLayer.GetMarketDays(startDate, endDate);
            for (int i = 0; i < marketDays.Count() - 1; i++)
            {
                var previousMarketDay = marketDays.ElementAt(i);
                var currentMarketDay = marketDays.ElementAt(i + 1);

                // Previous after hours trading to midnight
                var startTime = previousMarketDay.GetTradingCloseTimeUtc();
                var endTime = currentMarketDay.GetTradingOpenTimeUtc().Date;

                var gapUps = await _gapUpScreener.GetGapUpBetween(startTime, endTime, allTradableAssets, 10m, 0m, new BarTimeFrame(30, BarTimeFrameUnit.Minute));
                var aggregateData = await _dataLayer.GetAggregateDataMulti(gapUps.ChangePercentage.Keys, startTime, currentMarketDay.GetSessionCloseTimeUtc(), new BarTimeFrame(30, BarTimeFrameUnit.Minute));

                decimal profit = 0m;
                var targets = gapUps.ChangePercentage.Join(allTradableAssets, x => x.Key, y => y.Symbol, (x, y) => new { x, y });
                targets = targets.Where(x => x.y.Shortable).Take(20);

                foreach (var gapup in targets)
                {
                    if (aggregateData.TryGetValue(gapup.x.Key, out var data))
                    {
                        var entry = data.First(x => x.TimeUtc >= currentMarketDay.GetSessionOpenTimeUtc());
                        var exit = data.First(x => x.TimeUtc >= currentMarketDay.GetTradingOpenTimeUtc());
                        profit += exit.Close - entry.Close;
                    }
                }

                totalProfit += profit;
                _logger.LogInformation($"Profit: {profit}");
            }

            _logger.LogInformation($"Total Profit: {totalProfit}");

        }
    }
}
