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
using Denali.Shared.Utility;
using Denali.Models.Polygon;
using AutoMapper;

namespace Denali.Processors
{
    public class LiveTradingProcessor : IProcessor
    {
        private readonly IConfiguration _configuration;
        private readonly IAggregateStrategy _strategy;
        private readonly PolygonStreaming _polygonStreamingClient;
        private readonly IMapper _mapper;
        private readonly ILogger<LiveTradingProcessor> _logger;

        public List<IAggregateData> Data { get; set; }

        public LiveTradingProcessor(IConfiguration configuration
            , PolygonStreaming polygonStreamingClient
            , IAggregateStrategy strategy
            , IMapper mapper
            , ILogger<LiveTradingProcessor> logger)
        {
            this._configuration = configuration;
            this._polygonStreamingClient = polygonStreamingClient;
            this._strategy = strategy;
            this._mapper = mapper;
            this._logger = logger;

            Data = new List<IAggregateData>();
        }

        public async Task Process(CancellationToken stoppingToken)
        {
            var ticker = _configuration["ticker"];

            //TODO: Get backlog data
            //TODO: Get current data (if any)

            //Initialize strategy
            _strategy.Initialize(Data);

            //Wire up events from the streaming client
            WireEvents();
            _logger.LogInformation("Connecting to polygon");

            //Connect and Authenticate
            var authStatus = await _polygonStreamingClient.Client.ConnectAndAuthenticateAsync();
            _logger.LogInformation($"Connection status: {authStatus}");

            //Subscribe to data stream
            _logger.LogInformation("Subscribing to minute data");
            _polygonStreamingClient.Client.SubscribeMinuteAgg(ticker);
        }

        public void WireEvents()
        {
            _polygonStreamingClient.Client.MinuteAggReceived += MinuteAggregateReceived;
        }

        private void MinuteAggregateReceived(IStreamAgg obj)
        {
            _logger.LogInformation($"Aggregate received at {obj.EndTimeUtc}");
            _logger.LogInformation($"Open: {obj.Open}, Close: {obj.Close}, High: {obj.High}, Low: {obj.Low}, " +
                $"Volume: {obj.Volume}, Start: {obj.StartTimeUtc}, End: {obj.EndTimeUtc}");
            try
            {
                var bar = _mapper.Map<Bar>(obj);
                Data.Add(bar);
            }
            catch (Exception) { }

            //TODO: call strategy step when backlog is filled
        }
    }
}
