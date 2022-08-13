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
            bool? shortX = null;
            decimal xPrice = 0;
            decimal yPrice = 0;
            decimal xMultiplier = 0;
            decimal yMultiplier = 0;
            double beta = 0;
            decimal totalProfit = 0;
            foreach (var result in statsResult.Results)
            {
                var barx = aggregateXData.First(x => x.TimeUtc.Equals(result.TimeUTC));
                var bary = aggregateYData.First(y => y.TimeUtc.Equals(result.TimeUTC));

                if (Math.Abs(result.ZScore) >= 2 && tradeOpen == false)
                {
                    // Short scenario
                    if (result.ZScore < 0 && tradeOpen == false)
                    {
                        shortX = true;
                    }
                    // Long sceanrio
                    else if(result.ZScore > 0 && tradeOpen == false)
                    {
                        shortX = false;
                    }

                    _logger.LogInformation($"Open at {result.TimeUTC.Date} {result.TimeUTC.TimeOfDay}: Short {shortX}, VTI {barx.Close}, VOO {bary.Close}, Beta {result.Beta}");
                    xPrice = barx.Close;
                    yPrice = bary.Close;
                    xMultiplier = 1000 / barx.Close;
                    yMultiplier = 1000 / bary.Close;
                    beta = result.Beta;
                    tradeOpen = true;
                }

                if (tradeOpen == true)
                {
                    decimal profit;
                    if (shortX.Value)
                    {
                        profit = (xPrice - barx.Close) * xMultiplier;
                        profit += (bary.Close - yPrice) * yMultiplier;

                    }
                    else
                    {
                        profit = (yPrice - bary.Close) * yMultiplier;
                        profit += (barx.Close - xPrice) * xMultiplier;
                    }

                    if (profit >= 0.20m)
                    {
                        totalProfit += profit;

                        _logger.LogInformation($"Closed at {result.TimeUTC.Date} {result.TimeUTC.TimeOfDay}: VTI {barx.Close}, VOO {bary.Close}");
                        _logger.LogInformation($"Profit: {profit}");
                        tradeOpen = false;
                    }

                }
            }

            _logger.LogInformation($"Total Profit: {totalProfit}");
        }

        //https://www.reddit.com/r/algotrading/comments/obbb5d/kalman_filter_stat_arb/
        //https://algotrading101.com/learn/quantitative-trader-guide/#how-can-I-create-a-pairs-trading-strategy
        //https://www.reddit.com/r/algotrading/comments/vqmv7u/my_basic_code_to_find_cointegration_in_crypto/
        //hedge ratio: https://alpaca.markets/learn/statistically-significant-statarb-01/ , https://blog.quantinsti.com/statistical-arbitrage/
    }
}
 