using Denali.Models.Polygon;
using Denali.Models.Shared;
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
        private readonly PolygonService _polygonService;
        private readonly IAggregateStrategy _aggregateStrategy;
        private readonly TimeUtils _timeUtils;
        private readonly IConfiguration _configuration;

        public HistoricAggregateAnalysis(PolygonService polygonService, IAggregateStrategy aggregateStrategy, IConfiguration configuration)
        {
            this._polygonService = polygonService;
            this._configuration = configuration;
            this._aggregateStrategy = aggregateStrategy;
            this._timeUtils = new TimeUtils();
        }

        public async Task Process(CancellationToken stoppingToken)
        {
            var ticker = _configuration["ticker"];
            var timeSpan = EnumExtensions.ToEnum<BarTimeSpan>(_configuration["timespan"]);

            var dates = GetProcessDates();
            var range = (dates.Item2 - dates.Item1).Days;

            var backlogData = await GetBackLogData(ticker, timeSpan, dates.Item1);
            _aggregateStrategy.Initialize(backlogData);

            var stepDate = dates.Item1;
            for (int i = 0; i < range +1; i++)
            {
                var dayOpenTimestamp = _timeUtils.GetNYSEOpenUnixMS(stepDate);
                var dayCloseTimestamp = _timeUtils.GetNYSECloseUnixMS(stepDate);

                //var aggregateData = await _polygonService.GetAggregateData(ticker, 1, timeSpan, dayOpenTimestamp, dayCloseTimestamp, 1000);
                //StepThroughAggregateData(aggregateData);
            }
        }

        public (DateTime, DateTime) GetProcessDates()
        {
            var fromDateString = _configuration["from"];
            var toDateString = _configuration["to"];

            return (DateTime.Parse(fromDateString), DateTime.Parse(toDateString));
        }

        public void StepThroughAggregateData(AggregateResponse aggregateData)
        {
            var size = aggregateData.Bars.Count();
            if (size <= 1)
                return;

            var stepData = aggregateData.Bars.ToList();

            for (int i = 0; i < size; i++)
            {
                var batchRange = stepData.GetRange(0, i + 1);
                _aggregateStrategy.ProcessTick(batchRange);
            }
         }

        public async Task<IEnumerable<IAggregateData>> GetBackLogData(string ticker, BarTimeSpan timespan, DateTime startTime)
        {
            var data = new List<IAggregateData>();

            //var day2Open = _timeUtils.GetNYSEOpenUnixMS(startTime.AddDays(-2));
            //var day2Close = _timeUtils.GetNYSECloseUnixMS(startTime.AddDays(-2));
            //data.AddRange((await _polygonService.GetAggregateData(ticker, 1, timespan, day2Open, day2Close, 1000)).Bars);
            //data.RemoveAt(data.Count - 1);

            //var day1Open = _timeUtils.GetNYSEOpenUnixMS(startTime.AddDays(-1));
            //var day1Close = _timeUtils.GetNYSECloseUnixMS(startTime.AddDays(-1));
            //data.AddRange((await _polygonService.GetAggregateData(ticker, 1, timespan, day1Open, day1Close, 1000)).Bars);
            //data.RemoveAt(data.Count - 1);

            return data;
        }
    }
}
