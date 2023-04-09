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
        private readonly GapMomentumSettings _settings;
        private readonly Gap _gap;

        public GapMomentumAnalysis(AlpacaService alpacaService,
            IMapper mapper, 
            ILogger<GapMomentumAnalysis> logger
        ) : base(alpacaService, mapper)
        {
            _logger = logger;
            _settings = new GapMomentumSettings();
            _gap = new Gap(_settings.FullGap);
        }
        public async Task Process(string ticker, DateTime startDate, DateTime endDate, CancellationToken stoppingToken)
        {
            _alpacaService.InitializeTradingclient();
            _alpacaService.InitializeDataClient();

            var marketDays = await GetOpenMarketDays(startDate, endDate);
            if (marketDays?.FirstOrDefault()?.GetTradingDate().Day != startDate.Day)
            {
                _logger.LogInformation($"No trading window detected for day {startDate.Day}");
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

            marketDays = marketDays.Skip(1).Take(marketDays.Count() - 1);
            for (int i = 1; i < aggregateBars.Items.Count(); i++)
            {
                var marketDay = marketDays.ElementAt(i);
                var currentBar = _mapper.Map<AggregateBar>(aggregateBars.Items[i]);
                var previousBar = _mapper.Map<AggregateBar>(aggregateBars.Items[i - 1]);

                if (_gap.IsGapUp(currentBar, previousBar))
                {
                    var gapTrade = new GapMomentumTrade(ticker, true, previousBar, currentBar, marketDay);
                    await ProcessGap(gapTrade);
                } 

                if (_gap.IsGapDown(currentBar, previousBar))
                {
                    var gapTrade = new GapMomentumTrade(ticker, false, previousBar, currentBar, marketDay);
                    await ProcessGap(gapTrade);
                }
            }
        }

        private async Task ProcessGap(GapMomentumTrade gapTrade)
        {
            var agregateResponse = await _alpacaService.AlpacaDataClient.ListHistoricalBarsAsync(
               new HistoricalBarsRequest(
                   gapTrade.Symbol,
                   gapTrade.MarketDay.GetTradingOpenTimeUtc(),
                   gapTrade.MarketDay.GetTradingCloseTimeUtc(),
                   BarTimeFrame.Minute
               )
           ).ConfigureAwait(false);

            IEnumerable<AggregateBar> aggregateMinuteBars = _mapper.Map<List<AggregateBar>>(agregateResponse);
            gapTrade.AggregateMinuteBars = aggregateMinuteBars;

            gapTrade.Shares = (int)Math.Floor(_settings.InitialCapitol / gapTrade.TradeBar.Open);

            _logger.LogInformation(
                $"====> Gap {(gapTrade.Up ? "UP" : "DOWN")} detected on {TimeUtils.GetNewYorkTime(gapTrade.TradeBar.TimeUtc).ToString("MM-dd-yyyy")}");
            _logger.LogInformation($"{(gapTrade.Up ? "LONG" : "SHORT")} Entry at: {gapTrade.TradeBar.Open}");

            if (gapTrade.Up)
                ProcessGapUp(gapTrade);
            else
                ProcessGapDown(gapTrade);
        }

        private void ProcessGapUp(GapMomentumTrade gapTrade)
        {
            var highWaterMet = false;
            var totalCount = gapTrade.AggregateMinuteBars.Count();
            for (int i = 0; i < totalCount; i ++)
            {
                var bar = gapTrade.AggregateMinuteBars.ElementAt(i);
                var stopLoss = false;
                var highWaterTakeProfit = false;

                if (bar.Low <= bar.Open - _settings.StopLossMark)
                    stopLoss = true;

                if (bar.High >= gapTrade.TradeBar.Open + _settings.HighWaterMark)
                    highWaterMet = true;
                
                if (highWaterMet && bar.Low <= gapTrade.TradeBar.Open + _settings.HighWaterTakeProfit)
                    highWaterTakeProfit = true;

                if (stopLoss)
                {
                    if (highWaterMet)
                        _logger.LogWarning("High water and stop out in same minute!");

                    gapTrade.profit -= gapTrade.Shares * _settings.StopLossMark;
                    _logger.LogInformation($"Profit: {gapTrade.profit}");
                    break;
                }

                if (highWaterTakeProfit)
                {
                    _logger.LogInformation($"Stop Profit at: {TimeUtils.GetNewYorkTime(bar.TimeUtc).TimeOfDay}");
                    gapTrade.profit += gapTrade.Shares * _settings.HighWaterTakeProfit;
                    _logger.LogInformation($"Profit: {gapTrade.profit}");
                    break;
                }

                if ((i + 1) == totalCount)
                {
                    _logger.LogInformation($"Take Profit at: {TimeUtils.GetNewYorkTime(bar.TimeUtc).TimeOfDay}");
                    gapTrade.profit += gapTrade.Shares * bar.Close - gapTrade.TradeBar.Open;
                    _logger.LogInformation($"Profit: {gapTrade.profit}");
                    break;
                }
            }
        }

        private void ProcessGapDown(GapMomentumTrade gapTrade)
        {

        }
    }
}
