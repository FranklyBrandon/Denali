using Alpaca.Markets;
using AutoMapper;
using Denali.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public class ScalpingAnalysicProcessor : StrategyProcessorBase
    {
        private readonly ILogger _logger;

        public ScalpingAnalysicProcessor(AlpacaService alpacaService,
            IMapper mapper,
            ILogger<PressureProcessorAnalysis> logger
        ) : base(alpacaService, mapper)
        {
            _logger = logger;
        }

        public async Task Process(string symbol, DateTime startDate, DateTime endDate, CancellationToken stoppingToken)
        {
            _alpacaService.InitializeTradingclient();
            _alpacaService.InitializeDataClient();

            var marketDays = await GetOpenMarketDays(startDate, endDate);
            var calenderMap = marketDays.ToDictionary(x => x.GetTradingDate(), y => y);

            var aggregateBars = await GetAggregateData(
                symbol,
                marketDays.First().GetTradingOpenTimeUtc(),
                marketDays.Last().GetTradingCloseTimeUtc(),
                BarTimeFrame.Day
            );

            const decimal TAKE_PROFIT = 0.25m;
            const decimal STOP_LOSS = 0.50m;


            var count = aggregateBars.Count() - 2;
            var initialCapital = 25000m;
            var capital = initialCapital;

            for (int i = 1; i < count; i++)
            {
                var previousBar = aggregateBars[i - 1];
                var currentBar = aggregateBars[i];

                var calenderDay = calenderMap[new DateOnly(currentBar.TimeUtc.Year, currentBar.TimeUtc.Month, currentBar.TimeUtc.Day)];

                if (currentBar.Open > previousBar.Close)
                {
                    _logger.LogInformation($"Long Trade Opened: {currentBar.TimeUtc.Date.ToShortDateString()}");
                    var tickData = await GetTickData(symbol, calenderDay);
                    var prices = tickData
                        .DistinctBy(x => x.Price)
                        .OrderBy(x => x.TimestampUtc)
                        .Select(x => x.Price)
                        .ToList();

                    var open = currentBar.Open;
                    foreach (var price in prices)
                    {
                        var stockCount = capital / open;

                        if (price >= open + TAKE_PROFIT)
                        {
                            capital += TAKE_PROFIT * stockCount;
                            _logger.LogInformation($"WIN: {capital}");
                            break;
                        }

                        if (price <= open - STOP_LOSS)
                        {
                            capital -= STOP_LOSS * stockCount;
                            _logger.LogInformation($"LOSS: {capital}");
                            break;
                        }
                    }
                }

                if (currentBar.Open < previousBar.Close)
                {
                    _logger.LogInformation($"Short Trade Opened: {currentBar.TimeUtc.Date.ToShortDateString()}");
                    var tickData = await GetTickData(symbol, calenderDay);
                    var prices = tickData
                        .DistinctBy(x => x.Price)
                        .OrderBy(x => x.TimestampUtc)
                        .Select(x => x.Price)
                        .ToList();

                    var open = currentBar.Open;
                    foreach (var price in prices)
                    {
                        var stockCount = capital / open;

                        if (price <= open - TAKE_PROFIT)
                        {
                            capital += TAKE_PROFIT * stockCount;
                            _logger.LogInformation($"WIN: {capital}");
                            break;
                        }

                        if (price >= open - STOP_LOSS)
                        {
                            capital -= STOP_LOSS * stockCount;
                            _logger.LogInformation($"LOSS: {capital}");
                            break;
                        }
                    }
                }
            }

            _logger.LogInformation($"Total increase: {((capital - initialCapital) / initialCapital) * 100}%");
            var buyHold = (initialCapital / aggregateBars.First().Open) * (aggregateBars.Last().Close - aggregateBars.First().Open);
            _logger.LogInformation($"Hold increase: {(buyHold / initialCapital) * 100}%");
        }
    }
}
