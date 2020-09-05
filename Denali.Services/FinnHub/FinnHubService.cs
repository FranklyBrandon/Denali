using Denali.Models.Data.FinnHub;
using Denali.Services.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Services.FinnHub
{
    public class FinnHubService
    {
        private readonly FinnHubClient _finnHubClient;
        public FinnHubService(FinnHubClient finnHubClient)
        {
            _finnHubClient = finnHubClient;
        }

        public async Task<string> GetCandleStickData(string symbol, CandleResolution resolution, long from, long to)
        {
            var la = await _finnHubClient.GetCandleData(symbol, resolution.GetAttributeValue(), from.ToString(), to.ToString());
            return "la";
        }
    }
}
