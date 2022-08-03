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
        private readonly PairSpreadCalculation _pairSpread;

        public PairAnalysis(AlpacaService alpacaService, FileService fileService, IMapper mapper) : base(alpacaService, mapper)
        {
            _fileService = fileService;
            _pairSpread = new PairSpreadCalculation(100);
        }

        public async Task Process(string tickerA, string tickerB, DateTime startDate, DateTime endDate, BarTimeFrame barTimeFrame, int numberOfBacklogDays, CancellationToken stoppingToken)
        {
            _alpacaService.InitializeTradingclient();
            _alpacaService.InitializeDataClient();

            var lastMarketDates = await GetOpenBacklogDays(numberOfBacklogDays + 3, startDate);
            var backlogDays = lastMarketDates.Skip(1).Take(numberOfBacklogDays).Reverse().Select(x => x);

            var backlogABars = new List<IBar>();
            var backlogBBars = new List<IBar>();
            foreach (var backlogDay in backlogDays)
            {
                var backlogA = await _alpacaService.AlpacaDataClient.ListHistoricalBarsAsync(
                    new HistoricalBarsRequest(tickerA, backlogDay.GetTradingOpenTimeUtc(), backlogDay.GetTradingCloseTimeUtc(), barTimeFrame));

                backlogABars.AddRange(backlogA.Items);

                var backlogB = await _alpacaService.AlpacaDataClient.ListHistoricalBarsAsync(
                    new HistoricalBarsRequest(tickerB, backlogDay.GetTradingOpenTimeUtc(), backlogDay.GetTradingCloseTimeUtc(), barTimeFrame));

                backlogBBars.AddRange(backlogB.Items);

                // TODO: In this scenario we should get historic quote price and hydrate the missing bars
                //if (backlogA.Items.Count != backlogB.Items.Count)
                //    throw new ArgumentOutOfRangeException("Historic pair timeframe data frames do not match");
            }

            var backlogAData = _mapper.Map<List<AggregateBar>>(backlogABars);
            var backlogBData = _mapper.Map<List<AggregateBar>>(backlogBBars);

            _pairSpread.Initialize(backlogAData, backlogBData);

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

                var length = tickerAData.Count() - 1;
                // Start at the second bar because a change percentage is needed
                for (int i = 1; i < length; i++)
                {
                    var origninalA = tickerAData.ElementAt(i - 1);
                    var currentA = tickerAData.ElementAt(i);
                    var origninalB = tickerBData.ElementAt(i - 1);
                    var currentB = tickerBData.ElementAt(i);

                    _pairSpread.AnalyzeStep(origninalA, currentA, origninalB, currentB);
                }

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
