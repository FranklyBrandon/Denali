using Denali.Models.Shared;
using Denali.Shared.Utility;
using Denali.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Denali.Processors.Analysis
{
    public class BarOverBarAnalysisProcessor : IAnalysisProcessor
    {
        public async Task Process(DateTime startTime, IEnumerable<string> tickers, CancellationToken stoppingToken)
        {
            var strategy = new BarOverBarStrategy();
            var previousDayData = await FileHelper.DeserializeJSONFromFile<AlpacaAggregateData>("Resources\\11_1_2021_AAPL.txt", stoppingToken);
            var currentDaydata = await FileHelper.DeserializeJSONFromFile<AlpacaAggregateData>("Resources\\11_2_2021_AAPL.txt", stoppingToken);
        }
    }
}
