using Denali.Models;
using Denali.Models.Data.Chart.CandleStick;
using Denali.Models.Data.FinnHub;
using Denali.Services.Utility;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<CandleStickChart> GetCandleStickChart(string symbol
            , CandleResolution resolution
            , DateTimeWithZone from
            , DateTimeWithZone to)
        {
            var candleResponse = await _finnHubClient
                .GetCandleData(symbol
                , resolution.GetAttributeValue()
                , from.UnixTime.ToString()
                , to.UnixTime.ToString());

            return new CandleStickChart(symbol
                , from
                , to
                , MapCandleArraysToCandleModels(candleResponse));
        }

        private IEnumerable<CandleStick> MapCandleArraysToCandleModels(CandleResponse candleArrays)
        {
            var count = candleArrays.Timestamp.Count();
            var candles = new List<CandleStick>();

            for (int i = 0; i < count; i++)
            {
                candles.Add(new CandleStick(
                    candleArrays.OpenPrice[i],
                    candleArrays.ClosePrice[i],
                    candleArrays.LowPrice[i],
                    candleArrays.HighPrice[i],
                    candleArrays.Volume[i],
                    candleArrays.Timestamp[i]
                    ));
            }

            return candles.OrderBy(x => x.Timestamp);
        }
    }
}
