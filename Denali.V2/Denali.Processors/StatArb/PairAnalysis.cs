using Alpaca.Markets;
using AutoMapper;
using Denali.Models;
using Denali.Services;
using Denali.Shared.Extensions;
using Denali.TechnicalAnalysis;
using Denali.TechnicalAnalysis.StatisticalArbitrage;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<PairAnalysis> _logger;

        private decimal _tickerAPrice;
        private decimal _tickerBPrice;
        private bool _tradeOpen = false;
        private bool _tickerALong = false;

        public PairAnalysis(AlpacaService alpacaService, FileService fileService, ILogger<PairAnalysis> logger, IMapper mapper) : base(alpacaService, mapper)
        {
            _fileService = fileService;
            _pairSpread = new PairSpreadCalculation(100);
            _logger = logger;
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
                    //_logger.LogInformation($"Z score {_pairSpread.PairSpreads.Last().zScore}, Spread: {_pairSpread.PairSpreads.Last().spread}");

                    //A - B is negative -> Long A short B
                    //A - B is positive -> Short A long B
                    //https://alpaca.markets/learn/pairs-trading/#example5 (use beta to weigh hedge ratio?)

                    if(_tradeOpen)
                    {
                        if(Math.Abs(_pairSpread.PairSpreads.Last().zScore) <= 0.5)
                        {
                            decimal profit = 0m;
                            if (_tickerALong)
                            {
                                profit += currentA.Close - _tickerAPrice;
                                profit += _tickerBPrice - currentB.Close;
                            }
                            else
                            {
                                profit += _tickerAPrice - currentA.Close;
                                profit += currentB.Close - _tickerBPrice;
                            }

                            _logger.LogInformation($"Trade Closed: {currentA.TimeUtc}, Profit: {profit}, Spread: {_pairSpread.PairSpreads.Last().spread}");
                            _tradeOpen = false;
                        }    
                    }
                    else if (Math.Abs(_pairSpread.PairSpreads.Last().zScore) >= 2)
                    {
                        _tradeOpen = true;
                        _tickerAPrice = currentA.Close;
                        _tickerBPrice = currentB.Close;
                        if (_pairSpread.PairSpreads.Last().spread > 0)
                            _tickerALong = false;
                        else
                            _tickerALong = true;

                        _logger.LogInformation($"Trade Opened: {currentA.TimeUtc}, Spread: {_pairSpread.PairSpreads.Last().spread}");
                    }
                }

                var zscores = _pairSpread.PairSpreads.Select(x =>
                    new PairSpread(x.varienceMean, x.standardDeviation, x.zScore.RoundToFourPlaces(), x.spread, x.timeUTC)
                ).ToList();

                var json = JsonSerializer.Serialize(zscores);


            }
        }

        //https://www.reddit.com/r/algotrading/comments/obbb5d/kalman_filter_stat_arb/
        //https://algotrading101.com/learn/quantitative-trader-guide/#how-can-I-create-a-pairs-trading-strategy
    }
}
