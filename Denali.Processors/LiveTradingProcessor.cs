using Denali.Services.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Denali.Services.Utility;
using Microsoft.Extensions.Logging;
using Denali.Services.Market;
using Denali.Services.Polygon;
using Denali.Models.Polygon;

namespace Denali.Processors
{
    public class LiveTradingProcessor : IProcessor
    {
        private readonly IConfiguration _configuration;
        private readonly TimeUtils _timeUtils;
        private readonly PolygonStreamingClient _polygonStreamingClient;

        public LiveTradingProcessor(IConfiguration configuration
            , TimeUtils timeUtils
            , PolygonStreamingClient polygonStreamingClient
            , ILogger<LiveTradingProcessor> logger)
        {
            this._configuration = configuration;
            this._timeUtils = timeUtils;
            this._polygonStreamingClient = polygonStreamingClient;
        }

        public async void Process(CancellationToken stoppingToken)
        {
            await _polygonStreamingClient.ConnectToPolygonStreams(CancellationToken.None);
            var ticker = _configuration["ticker"];
            await _polygonStreamingClient.SubscribeToChannel(Channel.AggregateMinute, stoppingToken, ticker);
        }
    }
}
