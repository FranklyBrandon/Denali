using Alpaca.Markets;
using AutoMapper;
using Denali.Models.Shared;
using Denali.Services.Alpaca;
using Denali.Shared.Utility;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public class RibbonTrendProcessor : IProcessor
    {
        private readonly IConfiguration _configuration;
        private readonly AlpacaDataService _alpacaService;
        private readonly TimeUtils _timeUtils;
        private readonly IMapper _mapper;

        public RibbonTrendProcessor(
            IConfiguration configuration,
            AlpacaDataService alpacaService,
            TimeUtils timeUtils,
            IMapper mapper)
        {
            this._configuration = configuration;
            this._alpacaService = alpacaService;
            this._timeUtils = timeUtils;
            this._mapper = mapper;
        }
        public async Task Process(DateTime startTime, CancellationToken stoppingToken)
        {
            var ticker = _configuration["ticker"];

            InitializeClients();





            return;
        }

        public Task ShutDown(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }

        public void OnBarReceived(IAggregateData barData)
        {
            throw new NotImplementedException();
        }

        private void InitializeClients()
        {
            //_alpacaService.InitializeDataClient();
            //_alpacaService.InitializeDataStreamingclient();
        }
    }
}
