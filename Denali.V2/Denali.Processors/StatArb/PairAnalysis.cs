using Alpaca.Markets;
using AutoMapper;
using Denali.Models;
using Denali.Services;
using Denali.TechnicalAnalysis;
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

        public PairAnalysis(AlpacaService alpacaService, FileService fileService, IMapper mapper) : base(alpacaService, mapper)
        {
            _fileService = fileService;
            _spreadAverage = new SimpleMovingAverageDouble(100);
            _std = new StandardDeviation();
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

                // Ignore first bar because we don't want to calculate percentage change from yesterday or pre-market
                int start = 1;
                int length = tickerAData.Count() - 1;
                List<object> zScoreValues = new List<object>();
                List<double> spreadValues = new List<double>();

                for (int i = start; i < length; i++)
                {
                    var originalA = tickerAData.ElementAt(i - 1);
                    var originalB = tickerBData.ElementAt(i - 1);
                    var newA = tickerAData.ElementAt(i);
                    var newB = tickerBData.ElementAt(i);

                    var percentageChangeA = PercentageDifference(originalA.Close, newA.Close);
                    var percentageChangeB = PercentageDifference(originalB.Close, newB.Close);

                    var spread = percentageChangeA - percentageChangeB;
                    spreadValues.Add(spread);
  

                    _spreadAverage.Analyze(spread);
                    var mean = _spreadAverage.MovingAverages.Last();
                    var std = _std.CalculateStandardDeviation(spreadValues, mean, 100);
                    var zScore = (spread - mean) / std;

                    zScoreValues.Add(new
                    {
                        Time = originalA.TimeUtc,
                        Value = zScore
                    });
                }

                var json = JsonSerializer.Serialize(zScoreValues);
            }
        }

        private double PercentageDifference(decimal originalValue, decimal newValue) =>
            (double)(Math.Abs(originalValue - newValue) / originalValue) * 100;

        //https://www.reddit.com/r/algotrading/comments/obbb5d/kalman_filter_stat_arb/
        //https://algotrading101.com/learn/quantitative-trader-guide/#how-can-I-create-a-pairs-trading-strategy
    }
}
