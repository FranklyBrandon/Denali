using Denali.Services.Polygon;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public class HistoricAggregateAnalysis : IProcessor
    {
        private readonly PolygonService _polygonService;
        private readonly IConfiguration _configuration;

        public HistoricAggregateAnalysis(PolygonService polygonService, IConfiguration configuration)
        {
            this._polygonService = polygonService;
            this._configuration = configuration;
        }

        public void Process(CancellationToken stoppingToken)
        {
            var dates = GetProcessDates();

            if (dates.Item1 > dates.Item2)
            {
                //Throw error I guess
            }
            var range = (dates.Item2 - dates.Item1).Days; 

            for (int i = 0; i < range; i++)
            {
                var currentDate = dates.Item1.AddDays(i).ToString("yyyy-MM-dd");

                _polygonService.GetAggregateData();
            }
        }

        public (DateTime, DateTime) GetProcessDates()
        {
            var fromDateString = _configuration["from"];
            var toDateString = _configuration["to"];

            return (DateTime.Parse(fromDateString), DateTime.Parse(toDateString));
        }
    }
}
