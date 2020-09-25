using Denali.Models;
using Denali.Models.Data.FinnHub;
using Denali.Services.FinnHub;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Denali.Services.Analysis
{
    public class HistoricAnalysisService
    {
        private FinnHubService _finnHubService;

        public HistoricAnalysisService(FinnHubService finnHubService)
        {
            this._finnHubService = finnHubService;
        }

        public async void RunHistoricAnalysis(string symbol, DateTimeWithZone from, DateTimeWithZone to, CandleResolution resolution)
        {
            var la = await _finnHubService.GetCandleStickChart(symbol, resolution, from, to);
        }
    }
}
