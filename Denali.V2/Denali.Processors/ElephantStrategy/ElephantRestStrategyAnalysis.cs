using IBApi;
using InteractiveBrokers.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors.ElephantStrategy
{
    public class ElephantRestStrategyAnalysis
    {
        private readonly IBService _ibService;
        public ElephantRestStrategyAnalysis(IBService ibService)
        {
            this._ibService = ibService ?? throw new ArgumentNullException(nameof(ibService));
        }
        public async Task Process(CancellationToken stoppingToken, DateTime day)
        {
            //var processTask = _ibService.Start(stoppingToken);
            var contract = new Contract();
            contract.ConId = 12087792;
            contract.Exchange = "IDEALPRO";

            //_ibService.GetHistoricalData(1, contract, new DateTime(2022, 06, 17, 20, 0, 0, DateTimeKind.Utc));
        }
    }
}
