using Alpaca.Markets;
using AutoMapper;
using Denali.Processors.VolatileUniverse;
using Denali.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public class AllMinuteDataProcessor : StrategyProcessorBase
    {
        public AllMinuteDataProcessor(AlpacaService alpacaService, IMapper mapper) : base(alpacaService, mapper)
        {
        }

        public async Task Process(DateTime startDate, DateTime endDate, CancellationToken stoppingToken)
        {
            _alpacaService.InitializeTradingclient();
            _alpacaService.InitializeDataClient();

            var marketDays = await GetOpenMarketDays(startDate, endDate);
            var aggregateBars = await GetAggregateData(
                "VTI",
                marketDays.First().GetTradingOpenTimeUtc(),
                marketDays.Last().GetTradingCloseTimeUtc(),
                BarTimeFrame.Minute
            );

            var barMap = aggregateBars.GroupBy(x => x.TimeUtc.Date).ToDictionary(x => x.Key, y => y.ToList());

            int wins = 0;
            decimal winTotals = 0;
            int loses = 0;
            decimal loseTotals = 0;
            foreach ( var bar in barMap )
            {
                var currentDate = bar.Key;
                DateTime indicatorTime = new(currentDate.Year, currentDate.Month, currentDate.Day, 15, 0, 0); // 10:00 am UTC

                try
                {
                    var open = bar.Value.First(x => x.TimeUtc == new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 14, 30, 0)); // 9:30 am UTC
                    var close = bar.Value.First(x => x.TimeUtc == new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 21, 0, 0)); // 4:00 pm UTC
                    var indicatorBar = bar.Value.First(x => x.TimeUtc == indicatorTime);

                    var indicatorDirection = indicatorBar.Close > open.Open ? 1 : -1;
                    var realDirection = close.Close > open.Open ? 1 : -1;

                    if (indicatorDirection == realDirection)
                    {
                        wins++;
                        if (realDirection == 1)
                        {
                            winTotals += close.Close - indicatorBar.Open;
                        }
                        else
                        {
                            winTotals += indicatorBar.Open - close.Close;
                        }

                    }
                    else
                    {
                        loses++;
                        if (realDirection == 1)
                        {
                            loseTotals += close.Close - indicatorBar.Open;
                        }
                        else
                        {
                            loseTotals += indicatorBar.Open - close.Close;
                        }
                    }
                }
                catch (Exception)
                {
                    continue;
                }

            }
        }
    }
}
