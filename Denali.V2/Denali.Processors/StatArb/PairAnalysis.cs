using Alpaca.Markets;
using AutoMapper;
using Denali.Models;
using Denali.Services;
using Denali.Services.PythonInterop;
using Denali.TechnicalAnalysis.StatisticalArbitrage;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Denali.Processors.StatArb
{
    public class PairAnalysis : StrategyProcessorBase
    {
        private readonly FileService _fileService;
        private readonly PairReturnsCalculation _pairReturns;
        private readonly IPythonInteropClient _pythonInteropClient;
        private readonly ILogger<PairAnalysis> _logger;
        private readonly int _backlog = 20;

        public PairAnalysis(AlpacaService alpacaService, FileService fileService, IPythonInteropClient pythonInteropClient, ILogger<PairAnalysis> logger, IMapper mapper) : base(alpacaService, mapper)
        {
            _fileService = fileService;
            _pairReturns = new PairReturnsCalculation(_backlog);
            _pythonInteropClient = pythonInteropClient;
            _logger = logger;
        }

        public async Task Process(string tickerX, string tickerY, DateTime startDate, DateTime endDate, BarTimeFrame barTimeFrame, CancellationToken stoppingToken = default)
        {
            _alpacaService.InitializeTradingclient();
            _alpacaService.InitializeDataClient();

            var marketDays = await GetOpenMarketDays(startDate, endDate);

            var tickerXBars = new List<IBar>();
            var tickerYBars = new List<IBar>();

            foreach (var marketDay in marketDays)
            {
                var dataX = await _alpacaService.AlpacaDataClient.ListHistoricalBarsAsync(
                    new HistoricalBarsRequest(tickerX, marketDay.GetTradingOpenTimeUtc(), marketDay.GetTradingCloseTimeUtc(), barTimeFrame));

                tickerXBars.AddRange(dataX.Items);

                var dataY = await _alpacaService.AlpacaDataClient.ListHistoricalBarsAsync(
                    new HistoricalBarsRequest(tickerY, marketDay.GetTradingOpenTimeUtc(), marketDay.GetTradingCloseTimeUtc(), barTimeFrame));

                tickerYBars.AddRange(dataY.Items);
            }

            var aggregateXData = _mapper.Map<List<AggregateBar>>(tickerXBars);
            var aggregateYData = _mapper.Map<List<AggregateBar>>(tickerYBars);

            var statsResult = await _pythonInteropClient.GetOLSCalculation(aggregateXData, aggregateYData, _backlog);
            var json = JsonSerializer.Serialize(statsResult.Results);

            bool tradeOpen = false;
            bool? positive = null;
            decimal xPrice = 0;
            decimal yPrice = 0;
            double beta = 0;
            foreach (var result in statsResult.Results)
            {
                var barx = aggregateXData.First(x => x.TimeUtc.Equals(result.TimeUTC));
                var bary = aggregateYData.First(y => y.TimeUtc.Equals(result.TimeUTC));

                if (tradeOpen == true && xPrice != 0 && yPrice != 0)
                {
                    decimal profit = 0m;
                    if (positive.Value)
                    {
                        profit = (yPrice - bary.Close);
                        //_logger.LogInformation($"Y: (Entry Price - Current Price) ({yPrice} - {bary.Close})");
                        profit += (barx.Close - xPrice) * (decimal)beta;
                        //_logger.LogInformation($"X: (Current Price - EntryPrice) * beta ({barx.Close} - {xPrice} * {beta}");
                    } else
                    {
                        profit = (bary.Close - yPrice);
                        profit += (xPrice - barx.Close) * (decimal)beta;
                    }
                    _logger.LogInformation($"Running Profit at {barx.TimeUtc.TimeOfDay}: {profit}, Spread: {result.Spread}");
                }

                if (Math.Abs(result.ZScore) >= 2)
                {
                    _logger.LogInformation($"New Trade Opened at {barx.TimeUtc.Date.ToShortDateString()} {barx.TimeUtc.TimeOfDay}, Spread: {result.Spread}");
                    if (result.ZScore >= 2)
                        // Short Y, Long X
                        positive = true;
                    else if (result.ZScore <= -2)
                        // Long Y, Short X
                        positive = false;
                    else
                        throw new Exception("What?");

                    tradeOpen = true;
                    xPrice = barx.Close;
                    yPrice = bary.Close;
                    beta = result.Beta;
                }
            }

        }

        //https://www.reddit.com/r/algotrading/comments/obbb5d/kalman_filter_stat_arb/
        //https://algotrading101.com/learn/quantitative-trader-guide/#how-can-I-create-a-pairs-trading-strategy
        //https://www.reddit.com/r/algotrading/comments/vqmv7u/my_basic_code_to_find_cointegration_in_crypto/
        //hedge ratio: https://alpaca.markets/learn/statistically-significant-statarb-01/ , https://blog.quantinsti.com/statistical-arbitrage/
    }
}
 