using Denali.Models.Data.FinnHub;
using Denali.Services.FinnHub;
using Denali.Services.Utility;
using System;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public class HistoricAnalyze
    {
        private readonly FinnHubService _finnHubService;
        public HistoricAnalyze(FinnHubService finnHubService)
        {
            this._finnHubService = finnHubService;
        }

        public async Task Process()
        {
            var utils = new TimeUtils();

            var open = utils.GetNYSEOpen(DateTime.Today);
            var close = utils.GetNYSEClose(DateTime.Today);

            await _finnHubService.GetCandleStickChart("WMT", CandleResolution.Sixty, open, close);
        }
    }
}
