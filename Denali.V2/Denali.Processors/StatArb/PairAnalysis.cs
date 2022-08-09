using Alpaca.Markets;
using AutoMapper;
using Denali.Models;
using Denali.Services;
using Denali.Services.PythonInterop;
using Denali.TechnicalAnalysis.StatisticalArbitrage;
using Microsoft.Extensions.Logging;

namespace Denali.Processors.StatArb
{
    public class PairAnalysis : StrategyProcessorBase
    {
        private readonly FileService _fileService;
        private readonly PairReturnsCalculation _pairReturns;
        private readonly IPythonInteropClient _pythonInteropClient;
        private readonly ILogger<PairAnalysis> _logger;
        private readonly int _backlog = 100;

        public PairAnalysis(AlpacaService alpacaService, FileService fileService, IPythonInteropClient pythonInteropClient, ILogger<PairAnalysis> logger, IMapper mapper) : base(alpacaService, mapper)
        {
            _fileService = fileService;
            _pairReturns = new PairReturnsCalculation(_backlog);
            _pythonInteropClient = pythonInteropClient;
            _logger = logger;
        }

        public async Task Process(string tickerX, string tickerY, DateTime startDate, DateTime endDate, BarTimeFrame barTimeFrame, int numberOfBacklogDays, CancellationToken stoppingToken)
        {
            _alpacaService.InitializeTradingclient();
            _alpacaService.InitializeDataClient();

            var lastMarketDates = await GetOpenBacklogDays(numberOfBacklogDays + 3, startDate);
            var backlogDays = lastMarketDates.Skip(1).Take(numberOfBacklogDays).Reverse().Select(x => x);

            var backlogXBars = new List<IBar>();
            var backlogYBars = new List<IBar>();
            foreach (var backlogDay in backlogDays)
            {
                var backlogX = await _alpacaService.AlpacaDataClient.ListHistoricalBarsAsync(
                    new HistoricalBarsRequest(tickerX, backlogDay.GetTradingOpenTimeUtc(), backlogDay.GetTradingCloseTimeUtc(), barTimeFrame));

                backlogXBars.AddRange(backlogX.Items);

                var backlogY = await _alpacaService.AlpacaDataClient.ListHistoricalBarsAsync(
                    new HistoricalBarsRequest(tickerY, backlogDay.GetTradingOpenTimeUtc(), backlogDay.GetTradingCloseTimeUtc(), barTimeFrame));

                backlogYBars.AddRange(backlogY.Items);

                // TODO: In this scenario we should get historic quote price and hydrate the missing bars
                //if (backlogA.Items.Count != backlogB.Items.Count)
                //    throw new ArgumentOutOfRangeException("Historic pair timeframe data frames do not match");
            }

            var backlogXData = _mapper.Map<List<AggregateBar>>(backlogXBars);
            var backlogYData = _mapper.Map<List<AggregateBar>>(backlogYBars);

            _pairReturns.Initialize(backlogXData, backlogYData);
            var movingXBars = backlogXData;
            var movingYBars = backlogYData;

            var marketDays = await GetOpenMarketDays(startDate, endDate);
            if (marketDays != null && marketDays.Any())
            {
                var tickerXbars = await _alpacaService.AlpacaDataClient.ListHistoricalBarsAsync(
                    new HistoricalBarsRequest(tickerX, marketDays.First().GetTradingOpenTimeUtc(), marketDays.Last().GetTradingCloseTimeUtc(), barTimeFrame));

                var tickerYbars = await _alpacaService.AlpacaDataClient.ListHistoricalBarsAsync(
                    new HistoricalBarsRequest(tickerY, marketDays.First().GetTradingOpenTimeUtc(), marketDays.Last().GetTradingCloseTimeUtc(), barTimeFrame));

                if (tickerXbars.Items.Count != tickerYbars.Items.Count)
                    throw new ArgumentOutOfRangeException("Historic pair timeframe data frames do not match");

                var tickerXData = _mapper.Map<List<AggregateBar>>(tickerXbars.Items);
                var tickerYData = _mapper.Map<List<AggregateBar>>(tickerYbars.Items);

                var length = tickerXData.Count() - 1;
                // Add first bar as a starting point
                movingXBars.Add(tickerXData.First());
                movingYBars.Add(tickerYData.First());

                // Start at the second bar because a change percentage is needed
                for (int i = 1; i < length; i++)
                {
                    var origninalX = tickerXData.ElementAt(i - 1);
                    var currentX = tickerXData.ElementAt(i);
                    var origninalY = tickerYData.ElementAt(i - 1);
                    var currentY = tickerYData.ElementAt(i);

                    _pairReturns.AnalyzeStep(origninalX, currentX, origninalY, currentY);

                    movingXBars.Add(currentX);
                    movingYBars.Add(currentY);

                    var olsResults = await _pythonInteropClient.GetOLSCalculation(movingXBars, movingYBars, _backlog);
                }
            }
        }

        //https://www.reddit.com/r/algotrading/comments/obbb5d/kalman_filter_stat_arb/
        //https://algotrading101.com/learn/quantitative-trader-guide/#how-can-I-create-a-pairs-trading-strategy
        //https://www.reddit.com/r/algotrading/comments/vqmv7u/my_basic_code_to_find_cointegration_in_crypto/
        //hedge ratio: https://alpaca.markets/learn/statistically-significant-statarb-01/ , https://blog.quantinsti.com/statistical-arbitrage/
    }
}
