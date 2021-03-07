
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
            var symbols = _configuration["symbols"].Split(',');
            var fromDate = DateTime.Parse(_configuration["from"]);
            var toDate = DateTime.Parse(_configuration["to"]);
            var backlogStart = GetBacklogStart(fromDate);

            InitializeServices();

            _stockData = await GetHistoricData(backlogStart, symbols);

            //Initialize strategies
            InitializeStrategies(symbols);

            var dateRange = (toDate.Date - fromDate.Date).Days;

            //Step through every day of the analysis
            for (int i = 0; i < dateRange; i++)
            {
                var currentDay = fromDate.AddDays(i);
                var dayData = await GetHistoricData(currentDay, symbols);

                //Step through each symbol of each day
                foreach (var stockData in dayData)
                {
                    var symbol = stockData.Key;
                    var strategy = _strategies[symbol];
                    var aggregateData = stockData.Value;

                    //Step through each Aggregate of each symbol of each day
                    for (int y = 1; y < aggregateData.Count - 1; y++)
                    {
                        var range = aggregateData.GetRange(0, y);
                        var action = strategy.ProcessTick(range, _tradingContext);

                        if (action == MarketAction.Buy)
                        {
                            //_logger.LogInformation($"Buy Initiated at: {_timeUtils.GetETDatetimefromUnixS(range.Last().Time)}");
                            _tradingContext.LongOpen = true;
                            _tradingContext.Transaction = new Transaction(aggregateData[y + 1].OpenPrice, aggregateData[y + 1].Time);
                        }
                        else if (action == MarketAction.Sell)
                        {
                            _tradingContext.LongOpen = false;
                            _tradingContext.Transaction.SellPrice = aggregateData[y + 1].OpenPrice;
                            _tradingContext.Transaction.SellTime = aggregateData[y + 1].Time;

                            //_logger.LogInformation($"Sell Initiated at: {_timeUtils.GetETDatetimefromUnixS(range.Last().Time)}: {_tradingContext.Transaction.NetGain}");
                            _transactions.Add(_tradingContext.Transaction);
                        }
                        else
                        {
                            if (_tradingContext.Transaction is not null)
                            {
                                if (range.Last().HighPrice > _tradingContext.Transaction.High)
                                {
                                    _tradingContext.Transaction.High = range.Last().HighPrice;
                                }
                            }
                        }
                    }
                }

                foreach (var transaction in _transactions)
                {
                    _logger.LogInformation("[TRANSACTION]");
                    _logger.LogInformation($"But Time: {_timeUtils.GetETDatetimefromUnixS(transaction.BuyTime)}");
                    _logger.LogInformation($"Buy : {transaction.BuyPrice}");
                    _logger.LogInformation($"Sell: {transaction.SellPrice}");
                    _logger.LogInformation($"Sell Time: {_timeUtils.GetETDatetimefromUnixS(transaction.SellTime)}");
                    _logger.LogInformation($"High: {transaction.High}");
                    _logger.LogInformation($"Net : {transaction.NetGain}");
                }

                _logger.LogInformation($"Total Gain: {_transactions.Sum(x => x.NetGain)}");
            }

            FileHelper.WriteJSONToFile(_transactions, "AAPL_Run");
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

        private DateTime GetBacklogStart(DateTime start)
        {
            switch (start.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return start.AddDays(-3);
                case DayOfWeek.Sunday:
                    return start.AddDays(-2);
                case DayOfWeek.Saturday:
                    return start.AddDays(-1);
                default:
                    return start.AddDays(-1);
            }
        }
    }
}
