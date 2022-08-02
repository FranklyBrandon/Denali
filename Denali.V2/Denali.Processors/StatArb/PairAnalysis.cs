using Alpaca.Markets;
using AutoMapper;
using Denali.Models;
using Denali.Services;
using Denali.Shared.Extensions;
using Denali.TechnicalAnalysis;
using Denali.TechnicalAnalysis.StatisticalArbitrage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Denali.Processors.StatArb
{
    public class PairAnalysis : StrategyProcessorBase
    {
        private readonly FileService _fileService;
        private readonly SimpleMovingAverageDouble _spreadAverage;
        private readonly StandardDeviation _std;
        private readonly PairSpreadCalculation _pairSpread;

        public PairAnalysis(AlpacaService alpacaService, FileService fileService, IMapper mapper) : base(alpacaService, mapper)
        {
            _fileService = fileService;
            _spreadAverage = new SimpleMovingAverageDouble(100);
            _std = new StandardDeviation();
            _pairSpread = new PairSpreadCalculation(100);
        }

        public async Task Process(string tickerA, string tickerB, DateTime startDate, DateTime endDate, BarTimeFrame barTimeFrame, int numberOfBacklogDays, CancellationToken stoppingToken)
        {
            _alpacaService.InitializeTradingclient();
            _alpacaService.InitializeDataClient();

            var marketDays = await GetOpenMarketDays(startDate, endDate);
            if (marketDays != null && marketDays.Any())
            {
                var tickerAbars = await _alpacaService.AlpacaDataClient.ListHistoricalBarsAsync(
                    new HistoricalBarsRequest(tickerA, marketDays.First().GetTradingOpenTimeUtc(), marketDays.Last().GetTradingCloseTimeUtc(), barTimeFrame));

                var tickerBbars = await _alpacaService.AlpacaDataClient.ListHistoricalBarsAsync(
                    new HistoricalBarsRequest(tickerB, marketDays.First().GetTradingOpenTimeUtc(), marketDays.Last().GetTradingCloseTimeUtc(), barTimeFrame));

                if (tickerAbars.Items.Count != tickerBbars.Items.Count)
                    throw new ArgumentOutOfRangeException("Historic pair timeframe data frames do not match");

                var tickerAData = _mapper.Map<List<AggregateBar>>(tickerAbars.Items);
                var tickerBData = _mapper.Map<List<AggregateBar>>(tickerBbars.Items);

                _pairSpread.Analyze(tickerAData, tickerBData);
                var zscores = _pairSpread.PairSpreads.Select(x =>
                    new PairSpread(x.varienceMean, x.standardDeviation, x.zScore.RoundToFourPlaces(), x.timeUTC)
                ).ToList();

                var json = JsonSerializer.Serialize(zscores);
            }
        }

        private double PercentageDifference(decimal originalValue, decimal newValue) =>
            (double)(Math.Abs(originalValue - newValue) / originalValue) * 100;

        //https://www.reddit.com/r/algotrading/comments/obbb5d/kalman_filter_stat_arb/
        //https://algotrading101.com/learn/quantitative-trader-guide/#how-can-I-create-a-pairs-trading-strategy
    }
}
