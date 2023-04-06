using Alpaca.Markets;
using AutoMapper;
using Denali.Models;
using Denali.Processors.Exceptions;
using Denali.Services;
using Denali.Shared.Time;
using Denali.TechnicalAnalysis;
using Microsoft.Extensions.Logging;

namespace Denali.Processors.GapMomentum
{
    public class GapMomentumAnalysis : StrategyProcessorBase
    {
        private readonly ILogger _logger;

        public GapMomentumAnalysis(AlpacaService alpacaService,
            IMapper mapper, 
            ILogger<GapMomentumAnalysis> logger
        ) : base(alpacaService, mapper)
        {
            _logger = logger;
        }
        public async Task Process(string ticker, DateTime startDate, DateTime endDate, CancellationToken stoppingToken)
        {
            _alpacaService.InitializeTradingclient();
            _alpacaService.InitializeDataClient();

            var today = TimeUtils.GetNewYorkTime(DateTime.UtcNow);
            var marketDays = await GetOpenMarketDays(startDate, endDate);
            if (marketDays?.FirstOrDefault()?.GetTradingDate().Day != startDate.Day)
            {
                _logger.LogInformation($"No trading window detected for day {today.Day}");
                throw new NoTradingWindowException();
            }

            var aggregateBars = await _alpacaService.AlpacaDataClient.ListHistoricalBarsAsync(
                new HistoricalBarsRequest(
                    ticker,
                    marketDays.First().GetTradingOpenTimeUtc(),
                    marketDays.Last().GetTradingCloseTimeUtc(),
                    BarTimeFrame.Day
                )
            ).ConfigureAwait(false);

            const bool FULL_GAP = false;
            var gap = new Gap(FULL_GAP);

            marketDays = marketDays.Skip(1).Take(marketDays.Count() - 1);
            for (int i = 1; i < aggregateBars.Items.Count(); i++)
            {
                var marketDay = marketDays.ElementAt(i);
                var currentBar = _mapper.Map<AggregateBar>(aggregateBars.Items[i]);
                var previousBar = _mapper.Map<AggregateBar>(aggregateBars.Items[i - 1]);

                if (gap.IsGapUp(currentBar, previousBar))
                {
                    _logger.LogInformation(
                        $"====> Gap Up detected on {TimeUtils.GetNewYorkTime(currentBar.TimeUtc).ToString("MM-dd-yyyy")}");
                    _logger.LogInformation($"Long Entry at: {currentBar.Open}");
                    await ProcessGap(gapUp: true, ticker, currentBar.Open, currentBar, marketDay);
                }

                if (gap.IsGapDown(currentBar, previousBar))
                {
                    _logger.LogInformation(
                        $"=====> Gap Down detected on {TimeUtils.GetNewYorkTime(currentBar.TimeUtc).ToString("MM-dd-yyyy")}");
                    _logger.LogInformation($"Short Entry at: {currentBar.Open}");
                    await ProcessGap(gapUp: false, ticker, currentBar.Open, currentBar, marketDay);
                }
            }
        }

        private async Task ProcessGap(bool gapUp, string ticker, decimal entryPrice, AggregateBar currentBar, IIntervalCalendar marketDay)
        {
            var highWaterMark = 0.30m;
            var stopProfit = highWaterMark / 2;
            var stopLoss = 0.60m;

            var aggregateMinutes = await _alpacaService.AlpacaDataClient.ListHistoricalBarsAsync(
               new HistoricalBarsRequest(
                   ticker,
                   marketDay.GetTradingOpenTimeUtc(),
                   marketDay.GetTradingCloseTimeUtc(),
                   BarTimeFrame.Minute
               )
           ).ConfigureAwait(false); 

            bool highWaterMet = false;
            foreach (var bar in aggregateMinutes.Items)
            {
                if (gapUp)
                {
                    // Stop Loss scenario
                    if (bar.Low <= bar.Open - stopLoss)
                    {
                        _logger.LogWarning($"Stop loss at: {TimeUtils.GetNewYorkTime(bar.TimeUtc).TimeOfDay}");
                    }
                    
                    // Stop profit scenario
                    if (!highWaterMet)
                    {
                        if (bar.High >= entryPrice + highWaterMark)
                        {
                            highWaterMet = true;
                            _logger.LogInformation($"High Watermark met at: {TimeUtils.GetNewYorkTime(bar.TimeUtc).TimeOfDay}");
                            if (bar.Low <= entryPrice + stopProfit)
                                _logger.LogWarning("High water and stop out in same minute!");
                        }
                    }
                    else
                    {
                        if (bar.Low <= entryPrice + stopProfit)
                        {
                            _logger.LogInformation($"Stop Profit at: {TimeUtils.GetNewYorkTime(bar.TimeUtc).TimeOfDay}");
                            break;
                        }
                    }
                }
                else
                {
                    if (bar.High >= bar.Open + stopLoss)
                    {
                        _logger.LogWarning($"Stop loss at: {TimeUtils.GetNewYorkTime(bar.TimeUtc).TimeOfDay}");
                    }

                    if (!highWaterMet)
                    {
                        if (bar.Low <= entryPrice - highWaterMark)
                        {
                            highWaterMet = true;
                            _logger.LogInformation($"High Watermark met at: {TimeUtils.GetNewYorkTime(bar.TimeUtc).TimeOfDay}");
                            if (bar.High >= entryPrice - stopProfit)
                                _logger.LogWarning("High water and stop out in same minute!");
                        }
                    }
                    else
                    {
                        if (bar.High >= entryPrice - stopProfit)
                        {
                            _logger.LogInformation($"Stop Profit at: {TimeUtils.GetNewYorkTime(bar.TimeUtc).TimeOfDay}");
                            break;
                        }
                    }
                }
            }
        }
    }
}
