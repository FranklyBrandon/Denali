using Denali.Algorithms.BarAnalysis;
using Denali.Models.Polygon;
using Denali.Models.Shared;
using Denali.Services.Polygon;
using Denali.Shared.Utility;
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
        private readonly BarAlgorithmAnalysis _barAlogirthmnAnalysis;
        private readonly TimeUtils _timeUtils;
        private readonly IConfiguration _configuration;

        public HistoricAggregateAnalysis(PolygonService polygonService, BarAlgorithmAnalysis barAlogirthmnAnalysis, IConfiguration configuration)
        {
            this._polygonService = polygonService;
            this._configuration = configuration;
            this._barAlogirthmnAnalysis = barAlogirthmnAnalysis;
            this._timeUtils = new TimeUtils();
        }

        public async void Process(CancellationToken stoppingToken)
        {
            var ticker = _configuration["ticker"];
            var dates = GetProcessDates();

            if (dates.Item1 > dates.Item2)
            {
                //Throw error I guess
            }
            var range = (dates.Item2 - dates.Item1).Days; 

            for (int i = 0; i < range +1; i++)
            {
                var stepDate = dates.Item1.AddDays(i);
                var dayOpenTimestamp = _timeUtils.GetNYSEOpenUnixMS(stepDate);
                var dayCloseTimestamp = _timeUtils.GetNYSECloseUnixMS(stepDate);

                var aggregateData = await _polygonService.GetAggregateData(ticker, 1, BarTimeSpan.Minute, dayOpenTimestamp, dayCloseTimestamp, 1000);
                StepThroughAggregateData(aggregateData);
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
                _barAlogirthmnAnalysis.Analyze(batchRange);
            }
         }
    }
}
