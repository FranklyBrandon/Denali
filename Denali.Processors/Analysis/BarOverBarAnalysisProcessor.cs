using AutoMapper;
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
        private readonly IMapper _mapper;
        public BarOverBarAnalysisProcessor(IMapper mapper)
        {
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task Process(DateTime startTime, IEnumerable<string> tickers, CancellationToken stoppingToken)
        {
            var strategy = new BarOverBarStrategy();
            var previousDayData = await FileHelper.DeserializeJSONFromFile<AlpacaAggregateData>("Resources\\11_1_2021_AAPL.txt", stoppingToken);
            var previousBars = _mapper.Map<List<AggregateData>>(previousDayData).Cast<IAggregateData>().ToList();
            var currentDaydata = await FileHelper.DeserializeJSONFromFile<AlpacaAggregateData>("Resources\\11_2_2021_AAPL.txt", stoppingToken);
            var currentBars = _mapper.Map<List<AggregateData>>(currentDaydata).Cast<IAggregateData>().ToList();

            strategy.Initialize(previousBars);

            currentBars.ForEach(async x => await strategy.ProcessTick(x, null));

            await FileHelper.SerializeJSONToFile(strategy.MarketEvents, "Resources\\events.txt");
            await FileHelper.SerializeJSONToFile(strategy.EMAs, "Resources\\emas.txt");
        }
    }
}
