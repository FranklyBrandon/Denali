using Alpaca.Markets;
using AutoMapper;
using Denali.Models;
using Denali.Services;
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

        public PairAnalysis(AlpacaService alpacaService, FileService fileService, IMapper mapper) : base(alpacaService, mapper)
        {
            _fileService = fileService;
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

                //Ignore first bar because we don't want to calculate percentage change from yesterday or pre-market
                int start = 1;
                int length = tickerAData.Count() - 1;
                List<object> spread = new List<object>();

                for (int i = start; i < length; i++)
                {
                    var originalA = tickerAData.ElementAt(i - 1);
                    var originalB = tickerBData.ElementAt(i - 1);
                    var newA = tickerAData.ElementAt(i);
                    var newB = tickerBData.ElementAt(i);

                    var percentageChangeA = PercentageDifference(originalA.Close, newA.Close);
                    var percentageChangeB = PercentageDifference(originalB.Close, newB.Close);

                    spread.Add(new
                    {
                        Time = originalA.TimeUtc,
                        Value = percentageChangeA - percentageChangeB
                    });
                }

                var json = JsonSerializer.Serialize(spread);
            }
        }

        private decimal PercentageDifference(decimal originalValue, decimal newValue) =>
            (Math.Abs(originalValue - newValue) / originalValue) * 100;

        //https://www.reddit.com/r/algotrading/comments/obbb5d/kalman_filter_stat_arb/
        //https://algotrading101.com/learn/quantitative-trader-guide/#how-can-I-create-a-pairs-trading-strategy
    }
}
