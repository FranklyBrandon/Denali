
using Alpaca.Markets;
using Denali.Models.Shared;
using Denali.Services.Alpaca;
using Denali.Services.Polygon;
using Denali.Shared;
using Denali.Shared.Utility;
using Denali.Strategies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public class HistoricAggregateAnalysis : IProcessor
    {
        private readonly AlpacaService _alpacaService;
        private readonly TimeUtils _timeUtils;
        private readonly IConfiguration _configuration;
        private readonly ITradingContext _tradingContext;
        private readonly ILogger _logger;
        private Dictionary<string, List<IAggregateData>> _stockData;
        private Dictionary<string, IAggregateStrategy> _strategies;
        private List<Transaction> _transactions;

        public HistoricAggregateAnalysis(AlpacaService alpacaService, IConfiguration configuration, ILogger<HistoricAggregateAnalysis> logger)
        {
            this._alpacaService = alpacaService;
            this._configuration = configuration;
            this._timeUtils = new();
            this._tradingContext = new TradingContext();
            this._transactions = new();
            this._logger = logger;
        }

        public async Task Process(DateTime startTime, CancellationToken stoppingToken)
        {
            InitializeServices();
            _stockData = await GetHistoricData(startTime.AddDays(-1), new []{ "AAPL" });

            //Initialize strategies
            InitializeStrategies(new[] { "AAPL" });

            var dayData = await GetHistoricData(startTime, new[] { "AAPL" });

            //Step through strategies
            foreach (var stockData in dayData)
            {
                var symbol = stockData.Key;
                var strategy = _strategies[symbol];
                var aggregateData = stockData.Value;

                for (int i = 1; i < aggregateData.Count - 1; i++)
                {
                    var range = aggregateData.GetRange(0, i);
                    var action = strategy.ProcessTick(range, _tradingContext);

                    if (action == MarketAction.Buy)
                    {
                        _logger.LogInformation($"Buy Initiated at: {_timeUtils.GetETDatetimefromUnixS(range.Last().Time)}");
                        _tradingContext.LongOpen = true;
                        _tradingContext.Transaction = new Transaction(aggregateData[i + 1].OpenPrice, aggregateData[i + 1].Time);
                    }
                    else if (action == MarketAction.Sell)
                    {
                        _logger.LogInformation($"Sell Initiated at: {_timeUtils.GetETDatetimefromUnixS(range.Last().Time)}");
                        _tradingContext.LongOpen = false;
                        _tradingContext.Transaction.SellPrice = aggregateData[i + 1].OpenPrice;
                        _tradingContext.Transaction.SellTime = aggregateData[i + 1].Time;
                        _transactions.Add(_tradingContext.Transaction);
                    }
                }
            }

            FileHelper.WriteJSONToFile(_transactions, "AAPL_Run");
            _logger.LogInformation($"Total Gain: {_transactions.Sum(x => x.NetGain)}");
        }

        public Task ShutDown(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }

        public void OnBarReceived(IAggregateData barData)
        {
            throw new NotImplementedException();
        }

        private void InitializeServices()
        {
            _alpacaService.InitializeDataClient();
        }

        private void InitializeStrategies(IEnumerable<string> symbols)
        {
            _strategies = new();

            foreach (var symbol in symbols)
            {
                _strategies[symbol] = new RibbonTrendStrategy();
                _strategies[symbol].Initialize(_stockData[symbol]);
            }
        }

        private async Task<Dictionary<string, List<IAggregateData>>> GetHistoricData(DateTime startDate, params string[] symbols)
        {
            var backlogStartTime = _timeUtils.GetNYSEOpenDateTime(startDate);
            var backlogEndtime = _timeUtils.GetNYSECloseDateTime(startDate);

            return await _alpacaService.GetHistoricBarData(backlogStartTime, backlogEndtime, TimeFrame.Minute, symbols: symbols);
        }
    }
}
