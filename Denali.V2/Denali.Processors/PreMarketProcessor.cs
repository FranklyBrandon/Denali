using Alpaca.Markets;
using Denali.Processors.DenaliClimbStrategy;
using Denali.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public class PreMarketProcessor
    {
        private readonly DataLayerComponent _dataLayer;
        private readonly GapUpScreener _gapUpScreener;
        private readonly ILogger _logger;

        public PreMarketProcessor(DataLayerComponent dataLayer, GapUpScreener gapUpScreener, ILogger<PreMarketProcessor> logger)
        {
            _dataLayer = dataLayer;
            _gapUpScreener = gapUpScreener;
            _logger = logger;
        }

        public async Task Process()
        {
            await _dataLayer.Initialize();
            var allTradableAssets = await _dataLayer.GetAllTradableAssets();

            var date = DateTime.UtcNow;

            var marketBacklogDays = await _dataLayer.GetPastMarketDays(date, 4);
            var previousMarketDay = marketBacklogDays.ElementAt(marketBacklogDays.Count() - 2);
            var currentMarketDay = marketBacklogDays.Last();

            var startTime = previousMarketDay.GetTradingCloseTimeUtc();
            var endTime = currentMarketDay.GetTradingOpenTimeUtc().Date;

            var gapUps = await _gapUpScreener.GetGapUpBetween(startTime, endTime, allTradableAssets, 10m, 0m, new BarTimeFrame(30, BarTimeFrameUnit.Minute));
        }
    }
}
