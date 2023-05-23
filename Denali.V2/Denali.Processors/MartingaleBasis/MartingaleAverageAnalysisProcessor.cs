using Alpaca.Markets;
using AutoMapper;
using Denali.Models;
using Denali.Services;
using Denali.Shared.Extensions;
using Denali.Shared.Time;
using Denali.TechnicalAnalysis;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors.MartingaleBasis
{
    public class MartingaleAverageAnalysisProcessor : StrategyProcessorBase
    {
        private readonly ILogger _logger;
        private ExponentialMovingAverage _ema;
         
        public MartingaleAverageAnalysisProcessor(AlpacaService alpacaService,
            IMapper mapper,
            ILogger<PressureProcessorAnalysis> logger
        ) : base(alpacaService, mapper)
        {
            _logger = logger;
            _ema = new ExponentialMovingAverage(21);
        }

        public async Task Process(string symbol, DateTime startDate, DateTime endDate, CancellationToken stoppingToken)
        {
            _alpacaService.InitializeTradingclient();
            _alpacaService.InitializeDataClient();
             
            var marketDays = await GetOpenMarketDays(startDate, endDate);
            var aggregateBarData = await GetAggregateData(
                symbol,
                marketDays.First().GetTradingOpenTimeUtc(),
                marketDays.Last().GetTradingCloseTimeUtc(),
                new BarTimeFrame(15, BarTimeFrameUnit.Minute)
            );

            var aggregateBars = _mapper.Map<List<AggregateBar>>(aggregateBarData);

            const decimal initialCapitol = 25000.00m;
            decimal runningCapitol = initialCapitol;
            int stepSizing = 8;
            const int UNDER_EMA_MAX = 3;
            const int OVER_EMA_MAX = 3;
            List<Position> openPositions = new List<Position>();

            int count = aggregateBars.Count -1 ;

            int underEMACount = 0;
            int overEMACount = 0; 
             
            for (int i = 1; i < count; i++)
            {
                var bars = aggregateBars.Take(i).ToList();
                var latestBar = bars.Last();
                _ema.Analyze(bars);

                _logger.LogInformation($"====== {TimeUtils.GetNewYorkTime(latestBar.TimeUtc).ToShortDateString()} =====");

                // Initial Investment
                if (i == 1)
                {
                    var allInSize = (int)((runningCapitol / latestBar.Open).Round(0));
                    runningCapitol -= latestBar.Open * allInSize;
                    openPositions.Add(new Position(symbol, MarketSide.Buy, latestBar.TimeUtc, latestBar.Open, allInSize));
                    continue;
                }

                if (!_ema.MovingAverages.Any())
                    continue;

                if (runningCapitol >= (latestBar.Open * stepSizing))
                {
                    runningCapitol -= latestBar.Open * stepSizing;
                    openPositions.Add(new Position(symbol, MarketSide.Buy, latestBar.TimeUtc, latestBar.Open, stepSizing));
                    _logger.LogInformation($"Enter Trade ({stepSizing}x)");
                }

                if (latestBar.Close <= _ema.MovingAverages.Last())
                {
                    underEMACount++;

                    if (underEMACount >= UNDER_EMA_MAX && overEMACount >= OVER_EMA_MAX && GetCurrentProfit(openPositions, latestBar.Close) > 0)
                    {
                        runningCapitol += StockValue(openPositions, latestBar.Close);
                        openPositions.Clear();
                        overEMACount = 0;
                        _logger.LogInformation($"Closed all positions");
                        continue;
                    }
                }
                else
                {
                    overEMACount++;
                    underEMACount = 0;
                }
            }

            _logger.LogInformation($"Martingale Average gains: { (runningCapitol + StockValue(openPositions, aggregateBars.Last().Close)) - initialCapitol }");
            _logger.LogInformation($"Buy and hold gains: {GetBuyHoldGains(initialCapitol, aggregateBars)}");
        }

        private decimal GetCurrentProfit(List<Position> openPositions, decimal stockPrice) =>
            openPositions.Sum(x => x.Net(stockPrice));

        private decimal StockValue(List<Position> positions, decimal currentPrice) =>
            currentPrice * positions.Sum(x => x.Size);

        private decimal GetBuyHoldGains(decimal initialCapitol, List<AggregateBar> aggregates)
        {
            var first = aggregates.First();
            var last = aggregates.Last();
            var positionSize = (initialCapitol / first.Open).Round(0);
            return (last.Close - first.Open) * positionSize;
        }
    }
}
