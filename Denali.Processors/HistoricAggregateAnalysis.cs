
using Alpaca.Markets;
using Denali.Models.Shared;
using Denali.Services.Alpaca;
using Denali.Services.Polygon;
using Denali.Shared;
using Denali.Shared.Utility;
using Denali.Strategies;
using Microsoft.Extensions.Configuration;
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
        private readonly IAggregateStrategy _aggregateStrategy;
        private readonly TimeUtils _timeUtils;
        private readonly IConfiguration _configuration;
        private Dictionary<string, List<IAggregateData>> _stockData;

        public HistoricAggregateAnalysis(AlpacaService alpacaService, IAggregateStrategy aggregateStrategy, IConfiguration configuration)
        {
            this._alpacaService = alpacaService;
            this._configuration = configuration;
            this._aggregateStrategy = aggregateStrategy;
            this._timeUtils = new TimeUtils();
        }

        public async Task Process(DateTime startTime, CancellationToken stoppingToken)
        {
            InitializeServices();
            _stockData = await GetBacklogData(startTime.AddDays(-1), new []{ "AAPL" });

            var dayData = GetBacklogData(startTime, new[] { "AAPL" });
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

        private async Task<Dictionary<string, List<IAggregateData>>> GetBacklogData(DateTime startDate, params string[] symbols)
        {
            var backlogStartTime = _timeUtils.GetNYSEOpenDateTime(startDate);
            var backlogEndtime = _timeUtils.GetNYSECloseDateTime(startDate);

            return await _alpacaService.GetHistoricBarData(backlogStartTime, backlogEndtime, TimeFrame.Minute, symbols: symbols);
        }
    }
}
