using Alpaca.Markets;
using AutoMapper;
using Denali.Models;
using Denali.Services;
using Microsoft.Extensions.Logging;
using Denali.Shared.Extensions;
using Denali.Shared.Time;
 
namespace Denali.Processors.MartingaleBasis
{
    public class MartingaleBasisAnalysisProcessor : StrategyProcessorBase
    {
        private readonly ILogger _logger;



        public MartingaleBasisAnalysisProcessor(AlpacaService alpacaService,
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
            var aggregateBars = await GetAggregateData(
                symbol, 
                marketDays.First().GetTradingOpenTimeUtc(), 
                marketDays.Last().GetTradingCloseTimeUtc(),
                BarTimeFrame.Day
            ); 

            const decimal initialCapitol = 25000.00m;
            decimal runningCapitol = initialCapitol;
            int sizing = 2;
            var percentTarget = 7m;
            List<Position> openPositions = new List<Position>();

            foreach (var aggregateBar in aggregateBars)
            {
                _logger.LogInformation($"====== {TimeUtils.GetNewYorkTime(aggregateBar.TimeUtc).ToShortDateString()} =====");

                if (runningCapitol >= (aggregateBar.Open * sizing))
                {
                    runningCapitol -= aggregateBar.Open * sizing;
                    openPositions.Add(new Position(symbol, MarketSide.Buy, aggregateBar.TimeUtc, aggregateBar.Open, sizing));
                    _logger.LogInformation($"Enter Trade ({sizing}x)");
                }
                else
                {
                    var lastSize = (int)(runningCapitol / (aggregateBar.Open * sizing)).Round(0);
                    if (lastSize > 0)
                    {
                        runningCapitol -= aggregateBar.Open * sizing;
                        openPositions.Add(new Position(symbol, MarketSide.Buy, aggregateBar.TimeUtc, aggregateBar.Open, lastSize));
                        _logger.LogInformation($"Enter Last Size Trade ({lastSize}x)");
                    }
                    else
                    {
                        _logger.LogWarning("No more capitol! Holding Bag!");
                    }
                }

                var currentProfit = openPositions.Sum(x => x.Net(aggregateBar.Close));
                var percentGain = (currentProfit / initialCapitol) * 100;
                if (percentGain >= percentTarget)
                {
                    runningCapitol += StockValue(openPositions, aggregateBar.Close);
                    openPositions.Clear();
                    _logger.LogInformation($"Closed all positions");
                }
                else
                {
                    _logger.LogInformation("Held exisitng position");
                }

                _logger.LogInformation($"Liquid Cash: {runningCapitol}");
                _logger.LogInformation($"Portfolio Value: {runningCapitol + StockValue(openPositions, aggregateBar.Close)}");
            }

            _logger.LogInformation($"Martingale Basis gains: { (runningCapitol + StockValue(openPositions, aggregateBars.Last().Close)) - initialCapitol }");
            _logger.LogInformation($"Buy and hold gains: {GetBuyHoldGains(initialCapitol, aggregateBars)}");
        }

        private decimal GetBuyHoldGains(decimal initialCapitol, List<IBar> aggregates)
        {
            var first = aggregates.First();
            var last = aggregates.Last();
            var positionSize = (initialCapitol / first.Open).Round(0);
            return (last.Close - first.Open) * positionSize;
        }

        private decimal StockValue(List<Position> positions, decimal currentPrice) =>
            currentPrice * positions.Sum(x => x.Size);
    }
}
