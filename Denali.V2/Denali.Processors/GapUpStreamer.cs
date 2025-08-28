using Alpaca.Markets;
using Alpaca.Markets.Extensions;
using AutoMapper;
using Denali.Models;
using Denali.Services;
using Denali.TechnicalAnalysis;
using InteractiveBrokers.Models.Response;
using InteractiveBrokers.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public class GapUpStreamer
    {
        public Dictionary<string, List<IAggregateBar>> StreamedData;
        public Dictionary<string, ExponentialMovingAverage> SlowEMA;
        public Dictionary<string, ExponentialMovingAverage> FastEMA;

        private readonly AlpacaService _alpacaService;
        private readonly IInteractiveBrokersService _interactiveBrokersService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public GapUpStreamer(AlpacaService alpacaService, IInteractiveBrokersService interactiveBrokersService, IMapper mapper, ILogger logger)
        {
            _alpacaService = alpacaService;
            _interactiveBrokersService = interactiveBrokersService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task SubscribeToTickerStream(List<Contract> contracts, DateTime startTime, DateTime marketOpenTime)
        {
            StreamedData = new();
            SlowEMA = new();
            FastEMA = new();

            foreach (var contract in contracts)
            {
                StreamedData[contract.ticker] = new List<IAggregateBar>();
                SlowEMA[contract.ticker] = new ExponentialMovingAverage(CONSTANTS.SLOW_EMA_BACKLOG);
                FastEMA[contract.ticker] = new ExponentialMovingAverage(CONSTANTS.FAST_EMA_BACKLOG);

                var historicData = await _interactiveBrokersService.GetHistoricAggregates(contract.conid, marketOpenTime);
            }

            var subscription = await _alpacaService.AlpacaDataStreamingClient.SubscribeMinuteBarAsync(contracts.Select(x => x.ticker));
            subscription.Received += OnStreamedData;
        }

        private void OnStreamedData(IBar bar)
        {
            var aggregate = _mapper.Map<IAggregateBar>(bar);
            StreamedData[aggregate.Symbol].Append(aggregate);

            var aggregates = StreamedData[aggregate.Symbol];
            SlowEMA[aggregate.Symbol].Analyze(aggregates);
            FastEMA[aggregate.Symbol].Analyze(aggregates);
        }
    }
}
