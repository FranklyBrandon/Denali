using Alpaca.Markets;
using AutoMapper;
using Denali.Models.Shared;
using Denali.Services.Alpaca;
using Denali.Shared.Utility;
using Denali.Strategies;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public class TradingProcessor : IProcessor
    {
        private AlpacaService _alpacaService;
        private TimeUtils _timeUtils;
        private IMapper _mapper;
        private Dictionary<string, List<IAggregateData>> _stockData;
        private Dictionary<string, IAggregateStrategy> _strategies;

        private readonly ILogger<TradingProcessor> _logger;

        public TradingProcessor(
            AlpacaService alpacaService
            , TimeUtils timeUtils
            , IMapper mapper
            , ILogger<TradingProcessor> logger )
        {
            this._alpacaService = alpacaService;
            this._timeUtils = timeUtils;

            _stockData = new Dictionary<string, List<IAggregateData>>();
            _strategies = new Dictionary<string, IAggregateStrategy>();
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Process(DateTime startDate, IEnumerable<string> symbols, CancellationToken stoppingToken)
        {
            InitializeServices();
            _logger.LogInformation("Fetching Backlog Data");
            _stockData = await GetBacklogData(startDate.AddDays(-2), symbols.ToArray());

            //Initialize strategies
            _logger.LogInformation("Connecting and Authenticating with Data Stream");
            var authStatus = await _alpacaService.DataStreamingClient.ConnectAndAuthenticateAsync();
            _logger.LogInformation($"Authentication Status: {authStatus}");

            _logger.LogInformation("Subscribing to Channels");
            var subscription = _alpacaService.DataStreamingClient.GetMinuteAggSubscription("AAPL");
            subscription.Received += Subscription_Received;
            _alpacaService.DataStreamingClient.Subscribe(subscription);

            _logger.LogInformation("===Begin Strategy===");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000);
            }
        }

        private void Subscription_Received(IStreamAgg obj)
        {
            var aggregate = _mapper.Map<AggregateData>(obj);
            OnBarReceived(aggregate);
        }

        public void OnBarReceived(IAggregateData aggregate)
        {
            _logger.LogInformation($"O: {aggregate.OpenPrice}, H: {aggregate.ClosePrice}, L: {aggregate.LowPrice}, C: {aggregate.ClosePrice}, T: {aggregate.Time}");
            _stockData[aggregate.Symbol].Add(aggregate);
        }

        public async Task ShutDown(CancellationToken stoppingToken)
        {
            await _alpacaService.Disconnect();
        }

        private void InitializeServices()
        {
            _alpacaService.InitializeDataClient();
            _alpacaService.InitializeDataStreamingclient();
        }

        private async Task<Dictionary<string, List<IAggregateData>>> GetBacklogData(DateTime startDate, params string[] symbols)
        {
            var backlogStartTime = _timeUtils.GetNYSEOpenDateTime(startDate);
            var backlogEndtime = _timeUtils.GetNYSECloseDateTime(startDate);

            return await _alpacaService.GetHistoricBarData(backlogStartTime, backlogEndtime, TimeFrame.Minute, symbols: symbols);
        }

        public Task Process(DateTime startTime, CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
