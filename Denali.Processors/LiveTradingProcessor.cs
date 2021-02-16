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
        private readonly PolygonService _polygonService;
        private readonly TimeUtils _timeUtils;
        private readonly IMapper _mapper;
        private readonly ILogger<LiveTradingProcessor> _logger;

        public List<IAggregateData> Data { get; set; }

        public LiveTradingProcessor(IConfiguration configuration
            , PolygonStreaming polygonStreamingClient
            , PolygonService polygonService
            , TimeUtils timeUtils
            , IAggregateStrategy strategy
            , IMapper mapper
            , ILogger<LiveTradingProcessor> logger)
        {
            this._configuration = configuration;
            this._polygonStreamingClient = polygonStreamingClient;
            this._polygonService = polygonService;
            this._polygonService = polygonService;
            this._timeUtils = timeUtils;
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

        public async Task<IEnumerable<IAggregateData>> GetBackLogData(string ticker, BarTimeSpan timespan, DateTime startTime)
        {
            var data = new List<IAggregateData>();

            var day2Open = _timeUtils.GetNYSEOpenUnixMS(startTime.AddDays(-1));
            var day2Close = _timeUtils.GetNYSECloseUnixMS(startTime.AddDays(-1));
            data.AddRange((await _polygonService.GetAggregateData(ticker, 1, timespan, day2Open, day2Close, 1000)).Bars);
            data.RemoveAt(data.Count - 1);

            return data;
        }
    }
}
