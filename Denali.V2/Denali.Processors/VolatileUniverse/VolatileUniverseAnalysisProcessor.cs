using Alpaca.Markets;
using AutoMapper;
using Denali.Services;
using Denali.Services.PythonInterop;
using Denali.TechnicalAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors.VolatileUniverse
{
    public class VolatileUniverseAnalysisProcessor : StrategyProcessorBase
    {
        private readonly ILogger _logger;
        private readonly NASDAQ100Settings _nasdaq100Settings;

        public VolatileUniverseAnalysisProcessor(AlpacaService alpacaService,
            IMapper mapper,
            ILogger<PressureProcessorAnalysis> logger,
            IOptions<NASDAQ100Settings> settings
        ) : base(alpacaService, mapper)
        {
            _logger = logger;
            _nasdaq100Settings = settings.Value;
        }

        public async Task Process(DateTime startDate, CancellationToken stoppingToken)
        {
            _alpacaService.InitializeTradingclient();
            _alpacaService.InitializeDataClient();

            var marketDays = await GetOpenMarketDays(startDate.AddDays(-4), startDate);
            var aggregateBars = await GetAggregateDataMulti(
                _nasdaq100Settings.Symbols,
                marketDays.First().GetTradingOpenTimeUtc(),
                marketDays.Last().GetTradingCloseTimeUtc(),
                BarTimeFrame.Day
            );

            Dictionary<string, decimal> changePercentMap = new Dictionary<string, decimal>();
            foreach (var symbol in aggregateBars)
            {
                var count = symbol.Value.Count;
                var lastBar = symbol.Value[count - 1];
                var penUltimateBar = symbol.Value[count - 2];
                changePercentMap.Add(symbol.Key, ChangePercentage.Calculate(penUltimateBar.Close, lastBar.Open));
            }

            var orderedMap = changePercentMap.OrderByDescending(x => x.Value);
            var biggestGainer = orderedMap.First();
            var biggestLoser = orderedMap.Last();

            var gainerResult = await GetGainerStrategy(biggestGainer.Key, startDate);
            var loserResult = await GetLoserStrategy(biggestLoser.Key, startDate);

            string enterPosition = string.Empty;
            string enterSymbol = string.Empty;
            if (gainerResult.netGains > loserResult.netGains)
            {
                enterSymbol = biggestGainer.Key;
                enterPosition = gainerResult.StrategyType == StrategyType.Momentum ? "Long" : "Short";
            }
            else
            {
                enterSymbol = biggestLoser.Key;
                enterPosition = loserResult.StrategyType == StrategyType.Momentum ? "Short" : "Long";
            }

            _logger.LogInformation($"{enterPosition} stock {enterSymbol}");

            //var buys = orderedMap.Where(x => x.Value > 0).Select(x => x.Key);
            //var sells = orderedMap.Where(x => x.Value < 0).Select(x => x.Key);

            //var buyProfit = aggregateBars.Where(x => buys.Contains(x.Key)).Select(x => x.Value).Sum(x => x.Last().Close - x.Last().Open);
            //var sellProfit = aggregateBars.Where(x => sells.Contains(x.Key)).Select(x => x.Value).Sum(x => x.Last().Open - x.Last().Close);
        }

        private async Task<BackTestResult> GetGainerStrategy(string symbol, DateTime startDate)
        {
            var marketDays = await GetOpenMarketDays(startDate.AddDays(-20), startDate);
            marketDays = marketDays.Take(15);

            var aggregateBars = await GetAggregateData(
                symbol,
                marketDays.First().GetTradingOpenTimeUtc(),
                marketDays.Last().GetTradingCloseTimeUtc(),
                BarTimeFrame.Day
            );

            decimal momentumProfit = 0.0m;
            decimal reversalProfit = 0.0m;

            var count = aggregateBars.Count() -1;
            for ( var i = 1; i < count; i++ )
            {
                var previousBar = aggregateBars[i-1];
                var currentBar = aggregateBars[i];

                if (currentBar.Open > previousBar.Close)
                {
                    momentumProfit += currentBar.Close - currentBar.Open;
                    reversalProfit += currentBar.Open - currentBar.Close;
                }
            }

            var strategy = momentumProfit > reversalProfit ? StrategyType.Momentum : StrategyType.Reversal;
            return new BackTestResult(strategy, Math.Max(momentumProfit, reversalProfit));
        }

        private async Task<BackTestResult> GetLoserStrategy(string symbol, DateTime startDate)
        {
            var marketDays = await GetOpenMarketDays(startDate.AddDays(-20), startDate);
            marketDays = marketDays.Take(15);

            var aggregateBars = await GetAggregateData(
                symbol,
                marketDays.First().GetTradingOpenTimeUtc(),
                marketDays.Last().GetTradingCloseTimeUtc(),
                BarTimeFrame.Day
            );

            decimal momentumProfit = 0.0m;
            decimal reversalProfit = 0.0m;

            var count = aggregateBars.Count() - 1;
            for (var i = 1; i < count; i++)
            {
                var previousBar = aggregateBars[i - 1];
                var currentBar = aggregateBars[i];

                if (currentBar.Open < previousBar.Close)
                {
                    momentumProfit += currentBar.Open - currentBar.Close;
                    reversalProfit += currentBar.Close - currentBar.Open;
                }
            }

            var strategy = momentumProfit > reversalProfit ? StrategyType.Momentum : StrategyType.Reversal;
            return new BackTestResult(strategy, Math.Max(momentumProfit, reversalProfit));
        }
    }

    enum StrategyType
    {
        Momentum,
        Reversal
    }

    record BackTestResult(StrategyType StrategyType, decimal netGains);
}
