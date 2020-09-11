using Denali.Models;
using Denali.Models.Data.FinnHub;
using Denali.Services.FinnHub;
using Denali.Services.Utility;
using System;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public class HistoricAnalysisPrcessor
    {
        private readonly FinnHubService _finnHubService;
        public HistoricAnalysisPrcessor(FinnHubService finnHubService)
        {
            this._finnHubService = finnHubService;
        }

        public async Task Process(string symbol, DateTimeWithZone from, DateTimeWithZone to, CandleResolution resolution)
        {
            var utils = new TimeUtils();

            var open = utils.GetNYSEOpen(DateTime.Today);
            var close = utils.GetNYSEClose(DateTime.Today);

            await _finnHubService.GetCandleStickChart("WMT", CandleResolution.Sixty, open, close);
        }
    }
}
