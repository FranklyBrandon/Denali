using AutoMapper;
using Denali.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alpaca.Markets;
using AutoMapper;
using Denali.Models;
using Denali.Processors.Exceptions;
using Denali.Services;
using Denali.Shared.Time;
using Microsoft.Extensions.Logging;

namespace Denali.Processors
{
    public class PressureProcessorAnalysis : StrategyProcessorBase
    {
        private readonly ILogger _logger;

        public PressureProcessorAnalysis(AlpacaService alpacaService,
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

            int total = 0;
            int wins = 0;
            decimal limit = 0.10m;
            foreach (var marketDay in marketDays)
            {
                var tickData = await GetTickData(symbol, marketDay.GetTradingOpenTimeUtc(), marketDay.GetTradingCloseTimeUtc());
                var firstTick = tickData.First();
                ITrade secondTick = default;
                for (int i = 1; i < tickData.Count()-1; i++)
                {
                    var tick = tickData[i];
                    if (tick.Price != firstTick.Price)
                    {
                        secondTick = tick;
                        break;
                    }
                }
                var lastTick = tickData.Last();

                total++;

                if (secondTick.Price > firstTick.Price)
                {
                    var limitHit = tickData.Where(x => x.Price >= secondTick.Price + limit).FirstOrDefault();
                    var stopHit = tickData.Where(x => x.Price <= secondTick.Price - limit).FirstOrDefault();
                    if (limitHit != null)
                    {
                        if (stopHit == null)
                        {
                            wins++;
                            continue;
                        }

                        if (limitHit.TimestampUtc < stopHit.TimestampUtc)
                        {
                            wins++;
                            continue;
                        }
                    }
                }
                    

                if (secondTick.Price < firstTick.Price)
                {
                    var limitHit = tickData.Where(x => x.Price <= secondTick.Price - limit).FirstOrDefault();
                    var stopHit = tickData.Where(x => x.Price >= secondTick.Price + limit).FirstOrDefault();
                    if (limitHit != null)
                    {
                        if (stopHit == null)
                        {
                            wins++;
                            continue;
                        }

                        if (limitHit.TimestampUtc < stopHit.TimestampUtc)
                        {
                            wins++;
                            continue;
                        }
                    }
                }
                    
            }

            Console.WriteLine($"Total: {total}");
            Console.WriteLine($"Wins: {wins}");
            Console.WriteLine($"Percent: {(double)wins / (double)total}");
        }
    }
}
