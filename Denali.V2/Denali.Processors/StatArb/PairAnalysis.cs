using Alpaca.Markets;
using AutoMapper;
using Denali.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors.StatArb
{
    public class PairAnalysis : StrategyProcessorBase
    {
        public PairAnalysis(AlpacaService alpacaService, IMapper mapper) : base(alpacaService, mapper)
        {

        }

        public async void Process(string tickerA, string tickerB, DateTime startDate, DateTime endDate, BarTimeFrame barTimeFrame, int numberOfBacklogDays, CancellationToken stoppingToken)
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


            }




        }

        private decimal PercentageDifference(decimal originalValue, decimal newValue)
        {
            return Math.Abs(originalValue - newValue) / originalValue;
        }

        //https://www.reddit.com/r/algotrading/comments/obbb5d/kalman_filter_stat_arb/
        //https://algotrading101.com/learn/quantitative-trader-guide/#how-can-I-create-a-pairs-trading-strategy
    }
}
