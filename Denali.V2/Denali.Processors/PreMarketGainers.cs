using Alpaca.Markets;
using Denali.Processors.DenaliClimbStrategy;
using Denali.Services;
using Denali.TechnicalAnalysis;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public class PreMarketGainers
    {
        private readonly DataLayerComponent _dataLayer;
        private readonly ILogger _logger;

        public PreMarketGainers(DataLayerComponent dataLayer, ILogger<PreMarketGainers> logger)
        {
            _dataLayer = dataLayer;
            _logger = logger;
        }

        public async Task Process(DateTime startDate, DateTime endDate, CancellationToken stoppingToken)
        {
            await _dataLayer.Initialize();
            var allTradableAssets = await _dataLayer.GetAllTradableAssets();
            var marketDays = await _dataLayer.GetMarketDays(startDate, endDate);

            const int STOCK_COUNT = 100;
            decimal macroTotal = 0;
            for (int i = 1; i < marketDays.Count(); i++)
            {
                var previousPostMarketDay = marketDays.ElementAt(i -1);
                var currentPreMarketDay = marketDays.ElementAt(i);

                var aggregateData = await _dataLayer.GetAggregateDataMulti(allTradableAssets.Select(x => x.Symbol), previousPostMarketDay.GetTradingCloseTimeUtc(), currentPreMarketDay.GetTradingOpenTimeUtc(), BarTimeFrame.Minute);
                List<Results> results = new List<Results>();
                foreach (var asset in aggregateData)
                {
                    var postMarketData = asset.Value.Where(x => x.TimeUtc <= previousPostMarketDay.GetTradingCloseTimeUtc());
                    if (!postMarketData.Any())
                        continue;

                    var change = ChangePercentage.Calculate(postMarketData.First().Open, postMarketData.Last().Close);
                    var preMarketData = asset.Value.Where(x => x.TimeUtc >= currentPreMarketDay.GetSessionOpenTimeUtc());

                    if (!preMarketData.Any())
                        continue;

                    var preMarketProfit = preMarketData.Last().Close - preMarketData.First().Open;
                    results.Add(new Results(asset.Key, change, preMarketProfit, preMarketData.First().Open));
                    results = results.OrderByDescending(x => x.PostMarketChange).ToList();
                }

                var total = results.Take(STOCK_COUNT).Sum(x => x.PreMarketProfit);
                var totalInvestment = results.Take(STOCK_COUNT).Sum(x => x.EntryPrice);
                _logger.LogInformation($"{currentPreMarketDay.GetTradingDate().ToShortDateString()} Total: {total}, Investment: {totalInvestment}");
                macroTotal += total;
            }
            _logger.LogInformation($"TOTAL: {macroTotal}");
        }
    }

    public record Results(string Asset, decimal PostMarketChange, decimal PreMarketProfit, decimal EntryPrice);
}
