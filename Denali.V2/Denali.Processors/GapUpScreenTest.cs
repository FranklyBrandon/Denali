using Alpaca.Markets;
using Denali.Processors.DenaliClimbStrategy;
using Denali.Services;
using Denali.Services.Extensions;
using Denali.Shared.Extensions;
using Denali.TechnicalAnalysis;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public class GapUpScreenTest
    {
        private readonly DataLayerComponent _dataLayer;
        private readonly GapUpScreener _gapUpScreener;
        private readonly ILogger _logger;
        public List<IAsset> AllTradableAssets;

        public GapUpScreenTest(DataLayerComponent dataLayer, GapUpScreener gapUpScreener, ILogger<GapUpScreenTest> logger)
        {
            _dataLayer = dataLayer;
            _gapUpScreener = gapUpScreener;
            _logger = logger;
        }

        public async Task Initialize()
        {


            await _dataLayer.Initialize();

            AllTradableAssets = await _dataLayer.GetAllTradableAssets();
        }

        public async Task Process(DateTime date, CancellationToken stoppingToken)
        {
            var marketBacklogDays = await _dataLayer.GetPastMarketDays(date, 4);
            var previousMarketDay = marketBacklogDays.ElementAt(marketBacklogDays.Count() - 2);
            var currentMarketDay = marketBacklogDays.Last();

            var gapUps = await _gapUpScreener.GetGapUpStocks(previousMarketDay, currentMarketDay, AllTradableAssets, 10, 3);

            var data = await _dataLayer.GetAggregateDataMulti(gapUps.Select(x => x.Key), currentMarketDay.GetTradingOpenTimeUtc(), currentMarketDay.GetTradingOpenTimeUtc().AddMinutes(30), new BarTimeFrame(15, BarTimeFrameUnit.Minute));
            var granularData = await _dataLayer.GetAggregateDataMulti(gapUps.Select(x => x.Key), currentMarketDay.GetTradingOpenTimeUtc(), currentMarketDay.GetTradingCloseTimeUtc(), BarTimeFrame.Minute);

            decimal totalProfit = 0;
            decimal totalInvestment = 0;

            foreach (var ticker in data)
            {
                if (ticker.Value.Count() > 2)
                {
                    if (ticker.Value[0].IsGreen() &&
                        ticker.Value[1].IsGreen() &&
                        ticker.Value[1].Close > ticker.Value[0].High)
                    {

                        
                        var openPrice = ticker.Value.Last().Open;
                        var takeProfit = openPrice + (ticker.Value.Last().Open * .01m);
                        var stopLoss = ticker.Value[1].Low;

                        var takeProfitBar = granularData[ticker.Key].FirstOrDefault(x => x.TimeUtc > currentMarketDay.GetTradingOpenTimeUtc().AddMinutes(30) && x.High >= takeProfit);
                        var stopLossBar = granularData[ticker.Key].FirstOrDefault(x => x.TimeUtc > currentMarketDay.GetTradingOpenTimeUtc().AddMinutes(30) && x.Low <= stopLoss);
                        totalInvestment += openPrice;

                        if (takeProfitBar != null && stopLossBar == null)
                        {
                            _logger.LogInformation($"{ticker.Key} WIN of {takeProfit - openPrice}");
                            totalProfit += (takeProfit - openPrice);
                        }
                        else if (takeProfitBar == null && stopLossBar != null)
                        {
                            _logger.LogInformation($"{ticker.Key} LOSS of {openPrice - stopLoss}");
                            totalProfit -= (openPrice - stopLoss);
                        }
                        else if (takeProfitBar != null && stopLossBar != null)
                        {
                            if (takeProfitBar.TimeUtc == stopLossBar.TimeUtc)
                            {
                                _logger.LogInformation($"{ticker.Key} INDETERMINATE (same bar)");
                            }
                            else if (takeProfitBar.TimeUtc < stopLossBar.TimeUtc)
                            {
                                _logger.LogInformation($"{ticker.Key} WIN of {takeProfit - openPrice}");
                                totalProfit += (takeProfit - openPrice);
                            }
                            else
                            {
                                _logger.LogInformation($"{ticker.Key} LOSS of -{openPrice - stopLoss}");
                                totalProfit -= (openPrice - stopLoss);
                            }
                        }
                        else if (takeProfitBar == null && stopLossBar == null)
                        {
                            _logger.LogInformation($"{ticker.Key} INDETERMINATE (no exit)");
                        }
                    }
                }
            }

            _logger.LogInformation($"Total profit of {totalProfit} with {totalInvestment} risked for a gain of {ChangePercentage.Calculate(totalInvestment, totalInvestment + totalProfit).RoundToMoney()}%");
        }
    }
}
