using System;
using Microsoft.Extensions.Configuration;
using System.Threading;
using Microsoft.Extensions.Logging;
using Denali.Services.Polygon;
using Alpaca.Markets;
using System.Threading.Tasks;
using Denali.Strategies;
using System.Collections.Generic;
using Denali.Models.Shared;
using AutoMapper;
using System.Linq;

namespace Denali.Processors
{
    public class LiveTradingProcessor : IProcessor
    {
        private readonly IConfiguration _configuration;
        private readonly IAggregateStrategy _strategy;
        private readonly PolygonService _polygonService;
        private readonly IMapper _mapper;
        private readonly ILogger<LiveTradingProcessor> _logger;

        private readonly AggregationPeriod _timeFrame = new AggregationPeriod(1, AggregationPeriodUnit.Minute);
        private readonly DateTime _today = DateTime.Today;

        public List<IAggregateData> Data { get; set; }

        public LiveTradingProcessor(IConfiguration configuration
            , PolygonService polygonService
            , IAggregateStrategy strategy
            , IMapper mapper
            , ILogger<LiveTradingProcessor> logger)
        {
            this._configuration = configuration;
            this._polygonService = polygonService;
            this._strategy = strategy;
            this._mapper = mapper;
            this._logger = logger;

            Data = new List<IAggregateData>();
        }

        public async Task Process(CancellationToken stoppingToken)
        {
            var ticker = _configuration["ticker"];

            _polygonService.InitializeStreamingClient();
            _polygonService.InitializeDataClient();

            var currentResponse = await _polygonService.DataClient.ListAggregatesAsync(
                new AggregatesRequest(ticker, _timeFrame)
                    .SetInclusiveTimeInterval(_today, _today.AddDays(1)));

            var backlogResponse = await _polygonService.DataClient.ListAggregatesAsync(
                new AggregatesRequest(ticker, _timeFrame)
                    .SetInclusiveTimeInterval(_today.AddDays(-1), _today));

            // Only use last 20 bars for initialization
            var lastBars = backlogResponse.Items.Skip(Math.Max(0, backlogResponse.Items.Count - 20));
            var currentBars = _mapper.Map<List<AggregateData>>(currentResponse.Items);
            var backlogBars = _mapper.Map<List<AggregateData>>(lastBars);

            _strategy.Initialize(backlogBars);
            currentBars.ForEach(x => Data.Add(x));

            //Wire up events from the streaming client
            WireEvents();
            _logger.LogInformation("Connecting and Authorizing with polygon");

            //Connect and Authenticate
            var authStatus = await _polygonService.StreamingClient.ConnectAndAuthenticateAsync();
            _logger.LogInformation($"Connection status: {authStatus}");

            //Subscribe to data stream
            _polygonService.StreamingClient.SubscribeMinuteAgg(ticker);
            _logger.LogInformation("Subscribed to minute aggregate data");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000);
            }

            await _polygonService.DisconnectStreamingClient();
        }

        public void WireEvents()
        {
            _polygonService.StreamingClient.MinuteAggReceived += MinuteAggregateReceived;
        }

        private void MinuteAggregateReceived(IStreamAgg obj)
        {
            _logger.LogInformation($"Aggregate received at {obj.EndTimeUtc}");
            _logger.LogInformation($"Open: {obj.Open}, Close: {obj.Close}, High: {obj.High}, Low: {obj.Low}, " +
                $"Volume: {obj.Volume}, Start: {obj.StartTimeUtc}, End: {obj.EndTimeUtc}");
            try
            {
                var bar = _mapper.Map<AggregateData>(obj);
                Data.Add(bar);
            }
            catch (Exception) { }

            _strategy.ProcessTick(Data);
        }
    }
}
