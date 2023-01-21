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
            var xPrice = 0m;
            var yPrice = 0m;
            var zscore = 0d;
            // profit += (barx.Close - xPrice) * (decimal)beta;
            foreach (var result in statsResult.Results)
            {
                var barx = aggregateXData.First(x => x.TimeUtc.Equals(result.TimeUTC));
                var bary = aggregateYData.First(y => y.TimeUtc.Equals(result.TimeUTC));

                if (tradeOpen)
                {
                    var totalProfit = 0m;
                    // Short X, Long Y
                    if (zscore > 0)
                    {
                        var xProfit = xPrice - barx.Close;
                        var yProfit = bary.Close - yPrice;
                        var xMult = 5000 / xPrice;
                        var yMult = 5000 / yPrice;

                        totalProfit = (xProfit * xMult) + (yProfit * yMult);
                        _logger.LogInformation($"Profit: {totalProfit}");
                    }
                    // Long X, Short Y
                    else
                    {
                        var xProfit = barx.Close - xPrice;
                        var yProfit = yPrice - bary.Close;
                        var xMult = 5000 / xPrice;
                        var yMult = 5000 / yPrice;

                        totalProfit = (xProfit * xMult) + (yProfit * yMult);
                        _logger.LogInformation($"Profit: {totalProfit}");
                    }

                    if (Math.Sign(zscore) != Math.Sign(result.ZScore))
                    {
                        _logger.LogInformation($"Trade Closed: {result.TimeUTC.Day} {result.TimeUTC.TimeOfDay}, X:{barx.Close}, Y:{bary.Close}");
                        tradeOpen = false;
                    }
                }
                else if (Math.Abs(result.ZScore) >= 2.50)
                {
                    _logger.LogInformation($"Trade Opened: {result.TimeUTC.Day} {result.TimeUTC.TimeOfDay}, X:{barx.Close}, Y:{bary.Close}");
                    xPrice = barx.Close;
                    yPrice = bary.Close;
                    zscore = result.ZScore;
                    tradeOpen = true;
                }
            }
        }

        //https://www.reddit.com/r/algotrading/comments/obbb5d/kalman_filter_stat_arb/
        //https://algotrading101.com/learn/quantitative-trader-guide/#how-can-I-create-a-pairs-trading-strategy
        //https://www.reddit.com/r/algotrading/comments/vqmv7u/my_basic_code_to_find_cointegration_in_crypto/
        //hedge ratio: https://alpaca.markets/learn/statistically-significant-statarb-01/ , https://blog.quantinsti.com/statistical-arbitrage/
    }
}
 