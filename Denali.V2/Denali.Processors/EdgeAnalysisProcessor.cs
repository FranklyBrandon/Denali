using Alpaca.Markets;
using AutoMapper;
using Denali.Models;
using Denali.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public class EdgeAnalysisProcessor : StrategyProcessorBase
    {
        private readonly ILogger _logger;

        public EdgeAnalysisProcessor(AlpacaService alpacaService, ILogger<EdgeAnalysisProcessor> logger, IMapper mapper) : base(alpacaService, mapper)
        {
            _logger = logger;
        }

        public async Task Process(string symbol, CancellationToken stoppingToken)
        {
            _alpacaService.InitializeDataClient();
            var beginDate = new DateTime(2016, 1, 1); // Begginning of Alpaca data
            var endDate = DateTime.Now.Date;

            var aggregateBarData = await GetAggregateData(symbol, beginDate, endDate, BarTimeFrame.Day);
            var aggregateBars = _mapper.Map<List<AggregateBar>>(aggregateBarData);

            int alternatingCount = 0;
            int streakCount = 0;

            var count = aggregateBars.Count;
            for ( var i = 1; i < count; i++ )
            {
                var current = aggregateBars[i - 1];
                var next = aggregateBars[i];

                if (SameColor(current, next))
                    streakCount++;
                else
                    alternatingCount++;
            }

            _logger.LogInformation($"Alternating count: {alternatingCount}");
            _logger.LogInformation($"Streak count: {streakCount}");
        }

        private bool SameColor(AggregateBar current, AggregateBar next)
        {
            return current.Green() == next.Green();
        }
    }
}
