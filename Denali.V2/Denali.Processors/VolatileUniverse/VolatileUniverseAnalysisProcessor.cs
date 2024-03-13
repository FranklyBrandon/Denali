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

            //var buys = orderedMap.Where(x => x.Value > 0).Select(x => x.Key);
            //var sells = orderedMap.Where(x => x.Value < 0).Select(x => x.Key);

            //var buyProfit = aggregateBars.Where(x => buys.Contains(x.Key)).Select(x => x.Value).Sum(x => x.Last().Close - x.Last().Open);
            //var sellProfit = aggregateBars.Where(x => sells.Contains(x.Key)).Select(x => x.Value).Sum(x => x.Last().Open - x.Last().Close);
        }

        public async Task BackTestSymbols(string biggestGainer, string biggestLoser, DateTime startDate)
        {
            var marketDays = await GetOpenMarketDays(startDate.AddDays(-20), startDate.AddDays(-1));
            marketDays = marketDays.Take(15);
        }
    }
}
