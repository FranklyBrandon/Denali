using Denali.Services.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Denali.Services.Utility;

namespace Denali.Processors
{
    public class LiveTradingProcessor : IProcessor
    {
        private readonly DenaliSheetsService _sheetsService;
        private readonly IConfiguration _configuration;
        private readonly TimeUtils _timeUtils;
        public LiveTradingProcessor(DenaliSheetsService sheetsService, IConfiguration configuration, TimeUtils timeUtils)
        {
            this._sheetsService = sheetsService;
            this._configuration = configuration;
            this._timeUtils = timeUtils;
        }

        public async Task Process(CancellationToken stoppingToken)
        {
            //Fetch trading stocks
            var symbols = _sheetsService.GetTradingSettings().Where(x => x.Trading);
            if (!symbols.Any())
                return;

            //Write inital Report
            _sheetsService.WriteDenaliSheet(
                $"Denali {_configuration["status"]}-{_timeUtils.GetNYSEDateTime().ToString("g")}"
                , String.Join(',', symbols.Select(x => x.Symbol))
                , _timeUtils.GetNYSEDateTime().ToString("g")
                , "--"
                , "--"
                , null);


        }
    }
}
