using AutoMapper;
using Denali.Services;
using Denali.Shared.Extensions;
using InteractiveBrokers.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public class DenaliClimbIBProcessor : StrategyProcessorBase
    {
        private readonly IInteractiveBrokersService _interactiveBrokersService;
        private readonly ILogger<DenaliClimbIBProcessor> _logger;

        public DenaliClimbIBProcessor(AlpacaService alpacaService, IInteractiveBrokersService interactiveBrokersService, IMapper mapper, ILogger<DenaliClimbIBProcessor> logger) : base(alpacaService, mapper)
        {
            _interactiveBrokersService = interactiveBrokersService;
            _logger = logger;
        }

        public async Task Process(DateTime startDate, CancellationToken stoppingToken)
        {
            await _interactiveBrokersService.InitializeHttpAuth();
            _logger.NewLine();
            await _alpacaService.InittializeTradingClientAuth();
            _logger.NewLine();

            _logger.LogInformation($"Processing day {startDate.ToShortDateString()}");
            //var la = await _interactiveBrokersService.GetHistoricAggregates(265598, new(2025, 8, 22, 9, 30, 0));
            var allContracts = await _interactiveBrokersService.GetContractsByExchanges("NYSE", "NASDAQ");

            var date = new DateTime(2025, 8, 22, 9, 30, 0);
            foreach (var contract in allContracts)
            {
                var la = await _interactiveBrokersService.GetHistoricAggregates(contract.conid, new(2025, 8, 22, 9, 30, 0));
            }
            _logger.LogInformation("holy shit it worked");
        }
    }
}
