using Denali.Models.Data.FinnHub;
using Denali.Services.FinnHub;
using Denali.Services.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Runner.Processors
{
    public class AnalyzeProcessor
    {
        private readonly FinnHubService _finnHubService;
        public AnalyzeProcessor(FinnHubService finnHubService)
        {
            this._finnHubService = finnHubService;
        }

        public async Task Process()
        {
            var utils = new TimeUtils();

            var open = utils.GetNYSEOpen(DateTime.Today);
            var close = utils.GetNYSEClose(DateTime.Today);

            await _finnHubService.GetCandleStickData("WMT", CandleResolution.Sixty, open, close);
        }
    }
}
