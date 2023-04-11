using Alpaca.Markets;
using AutoMapper;
using Denali.Models;
using Denali.Processors.Exceptions;
using Denali.Services;
using Denali.Shared.Time;
using Microsoft.Extensions.Logging;

namespace Denali.Processors.GapMomentum
{
    public class GapMomentumAnalysis : StrategyProcessorBase
    {
        private readonly ILogger _logger;
        private readonly GapMomentumSettings _settings;

        public GapMomentumAnalysis(AlpacaService alpacaService,
            IMapper mapper, 
            ILogger<GapMomentumAnalysis> logger
        ) : base(alpacaService, mapper)
        {
            _logger = logger;
            _settings = new GapMomentumSettings();
        }
        public async Task Process(string symbol, DateTime startDate, DateTime endDate, CancellationToken stoppingToken)
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
                    symbol,
                    marketDays.First().GetTradingOpenTimeUtc(),
                    marketDays.Last().GetTradingCloseTimeUtc(),
                    BarTimeFrame.Day
                )
            ).ConfigureAwait(false);

            marketDays = marketDays.Skip(1).Take(marketDays.Count() - 1);
            for (int i = 1; i < aggregateBars.Items.Count(); i++)
            {
                var marketDay = marketDays.ElementAt(i);
                var previousBar = _mapper.Map<AggregateBar>(aggregateBars.Items[i - 1]);
                var currentBar = _mapper.Map<AggregateBar>(aggregateBars.Items[i]);

                var strategy = new GapMomentumStrategy(_settings);
                strategy.SetPreviousDailyBar(previousBar);
                strategy.OnPremarketTick(currentBar.Open);
                var trade = strategy.EvaluateTrade();

                if (trade.action == MarketAction.Trade)
                {
                    strategy.Trade.EntryPrice = currentBar.Open;
                    Console.WriteLine($"Processing Gap on {currentBar.TimeUtc.Date}");
                    await ProcessGap(symbol, marketDay, strategy);
                }                   
            }
        }

        private async Task ProcessGap(string symbol, IIntervalCalendar marketDay, GapMomentumStrategy strategy)
        {
            var ticks = await GetTickDataForDay(symbol, marketDay);
            var limitOrderOpen = false;

            foreach (var tick in ticks)
            {
                Console.WriteLine($"Price: {tick.Price}");
                var result = strategy.OnTick(tick.Price);

                // Limit order fulfilled
                if (limitOrderOpen && strategy.UnderPriceLimit(tick.Price, strategy.Trade.EntryPrice, _settings.HighWaterTakeProfit, strategy.Trade.direction))
                {
                    Console.WriteLine($"Stop Limit fulfilled at {TimeUtils.GetNewYorkTime(tick.TimestampUtc).TimeOfDay}");
                    return;
                }

                if (result.action == MarketAction.Trade)
                {
                    if (result.orderType == Models.OrderType.StopLimit)
                    {
                        limitOrderOpen = true;
                        Console.WriteLine($"Stop Limit order opened at {TimeUtils.GetNewYorkTime(tick.TimestampUtc).TimeOfDay}");
                    }

                    if (result.orderType == Models.OrderType.Market)
                    {
                        Console.WriteLine($"Stopped Out at {TimeUtils.GetNewYorkTime(tick.TimestampUtc).TimeOfDay}");
                        return;
                    }
                }
            }
        }

        private async Task<List<ITrade>> GetTickDataForDay(string symbol, IIntervalCalendar marketDay)
        {
            string? pageToken = default;
            List<ITrade> trades = new List<ITrade>();

            do
            {
                var request = new HistoricalTradesRequest(
                        symbol,
                        marketDay.GetTradingOpenTimeUtc(),
                        marketDay.GetTradingCloseTimeUtc()
                    ).WithPageSize(10000);

                if (!string.IsNullOrWhiteSpace(pageToken))
                    request.WithPageToken(pageToken);

                var response = await _alpacaService.AlpacaDataClient.GetHistoricalTradesAsync(request).ConfigureAwait(false);
                pageToken = response.NextPageToken;
                trades.AddRange(response.Items[symbol]);

            } while (!string.IsNullOrWhiteSpace(pageToken));

            return trades;
        }
    }
}
