using Alpaca.Markets;
using Denali.Services;
using Denali.Services.Extensions;
using Denali.Shared.Extensions;
using Denali.TechnicalAnalysis;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors.TrueFadeStrategy
{
   

    public class TrueFadeStrategy
    {
        private readonly DataLayerComponent _dataLayer;
        private readonly ILogger _logger;

        private const int LOOKBACK = 14;

        public TrueFadeStrategy(DataLayerComponent dataLayer, ILogger<TrueFadeStrategy> logger)
        {
            _dataLayer = dataLayer;
            _logger = logger;
        }

        public async Task ProcessRange(DateTime startDate, DateTime endDate, CancellationToken stoppingToken)
        {
            _logger.LogInformation($"True Fade HISTORIC RUN from {startDate.ToShortDateString()} to {endDate.ToShortDateString()}");
            _logger.NewLine();
            await _dataLayer.Initialize();

            var assets = await _dataLayer.GetAllTradableAssets();
            //assets = assets.Where(x => x.Symbol == "LPLA").ToList();

            var pastDays = await _dataLayer.GetPastMarketDays(startDate.AddDays(-1), 4);
            var forwardDays = await _dataLayer.GetMarketDays(startDate, endDate);
            List<IIntervalCalendar> marketDays = new() { pastDays.Last() };
            marketDays.AddRange(forwardDays);

            decimal totalProfit = 0;
            for (int i = 1; i < marketDays.Count; i++)
            {

                var currentDay = marketDays[i];
                var previousDay = marketDays[i - 1];
                var backlogStart = previousDay.GetTradingOpenTimeUtc().AddDays(-15);

                _logger.NewLine();
                _logger.LogInformation($"Analyzing day {currentDay.GetTradingOpenTimeUtc().ToShortDateString()}");

                var aggregateData = await _dataLayer.GetAggregateDataMulti(assets.Select(x => x.Symbol), backlogStart, currentDay.GetTradingOpenTimeUtc(), BarTimeFrame.Day);

                List<TrueFadeRecord> ranges = new List<TrueFadeRecord>();
                foreach (var data in aggregateData)
                {
                    var bars = data.Value;

                    if (bars.Count < 10) 
                        continue;

                    var lastBar = bars.GetHistoricValue(1);
                    var penUltimateBar = bars.GetHistoricValue(2);

                    if (!lastBar.IsGreen() || lastBar.Open <= penUltimateBar.High || lastBar.Close >= 500m)
                        continue;

                    var averageTrueRange = AverageTrueRange.CalculateAverageTrueRange(10, bars.Take(10));
                    var trueRange = AverageTrueRange.CalculateTrueRange(penUltimateBar, lastBar);

                    if (averageTrueRange == 0 || trueRange == 0)
                        continue;

                    var multiple = trueRange / averageTrueRange;
                    if (multiple > 3)
                    {
                        ranges.Add(new TrueFadeRecord(data.Key, lastBar.Close, multiple.RoundToMoney(), averageTrueRange.RoundToMoney(), 0));
                    }
                }

                var rangeAssets = ranges.Join(assets, x => x.Symbol, y => y.Symbol, (x, y) => new { x.Symbol, x.Price, x.MultipleATR, y.Shortable, x.AverageTrueRange });
                var ordered = rangeAssets.Where(x => x.Shortable).OrderByDescending(x => x.MultipleATR);

                if (ordered.Count() <= 0)
                    continue;

                // Enter positions
                decimal investment = 0;
                List<TrueFadeRecord> positions = new List<TrueFadeRecord>();
                foreach (var range in ordered)
                {
                    investment += range.Price;
                    positions.Add(new TrueFadeRecord(range.Symbol, range.Price, range.MultipleATR, range.AverageTrueRange, 0));
                    if (investment >= 25000)
                        break;
                }

                if (positions.Count <= 0)
                    continue;

                var minuteData = await _dataLayer.GetAggregateDataMulti(positions.Select(x => x.Symbol), currentDay.GetTradingOpenTimeUtc(), currentDay.GetTradingCloseTimeUtc(), BarTimeFrame.Minute);
                decimal dailyProfit = 0;
                foreach (var position in positions)
                {
                    decimal profit = 0;
                    var entryBar = aggregateData[position.Symbol].Last();
                    if (!minuteData.TryGetValue(position.Symbol, out var minutes))
                    {
                        profit += entryBar.Open - entryBar.Close;
                        dailyProfit += profit;
                        _logger.LogInformation($"{position.Symbol} Price: {position.Price} ATR Multiplier: {position.MultipleATR} ATR: {position.AverageTrueRange} Profit: {profit}");
                        break;
                    }

                    bool exited = false;
                    decimal stopLoss = entryBar.Open + position.AverageTrueRange;
                    decimal takeProfit = 0;

                    if (position.AverageTrueRange >= 3)
                    {
                        takeProfit = entryBar.Open - 1;
                    }

                    foreach (var minute in minutes)
                    {
                        if (minute.High >= stopLoss)
                        {
                            profit += entryBar.Open - stopLoss;
                            exited = true;
                            break;
                        }

                        if (minute.Low <= takeProfit)
                        {
                            profit += 1;
                            exited = true;
                            break;
                        }

                        if (minute.Low <= entryBar.Open - position.AverageTrueRange / 2)
                        {
                            stopLoss = entryBar.Open;
                        }
                        if (minute.Low <= entryBar.Open - position.AverageTrueRange)
                        {
                            stopLoss = entryBar.Open - position.AverageTrueRange / 2;
                        }
                        if (minute.Low <= entryBar.Open - position.AverageTrueRange - position.AverageTrueRange / 2)
                        {
                            stopLoss = entryBar.Open - position.AverageTrueRange;
                        }
                    }

                    if(!exited)
                    {
                        profit += entryBar.Open - entryBar.Close;
                    }
                    //if (entryBar.High - entryBar.Open >= position.averageTrueRange)
                    //{
                    //    profit -= position.averageTrueRange;
                    //}
                    //else
                    //{
                    //    profit += entryBar.Open - entryBar.Close;
                    //}

                   
                    dailyProfit += profit;
                    _logger.LogInformation($"{position.Symbol} Price: {position.Price} ATR Multiplier: {position.MultipleATR} ATR: {position.AverageTrueRange} Profit: {profit}");
                }

                _logger.LogInformation($"Total Profit: {dailyProfit} Total investment: {investment}");
                totalProfit += dailyProfit;
            }

            _logger.NewLine();
            _logger.LogInformation($"All time profit {totalProfit}.");
        }
    }
}
