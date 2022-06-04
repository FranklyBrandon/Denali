using Alpaca.Markets;
using Denali.Services;
using Denali.TechnicalAnalysis.ElephantBars;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors.ElephantStrategy
{
    public class ElephantRideStrategy
    {
        private readonly AlpacaService _alpacaService;
        private readonly ElephantBarSettings _elephantBarSettings;
        private readonly ILogger<ElephantRideStrategy> _logger;

        private const int BACKLOG_DAYS = 2;
        private const int BACKLOG_MARKET_DAYS = 5;

        public ElephantBars ElephantBars { get; private set; }
        public ElephantRideStrategy(AlpacaService alpacaService, ElephantBarSettings elephantBarSettings, ILogger<ElephantRideStrategy> logger)
        {
            _alpacaService = alpacaService ?? throw new ArgumentNullException(nameof(alpacaService));
            _elephantBarSettings = elephantBarSettings ?? throw new ArgumentNullException(nameof(elephantBarSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Requires Trading Client Initialized. Requires Data Client Initialized
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task Setup(DateTime date)
        {
            ElephantBars = new ElephantBars(_elephantBarSettings);
            var marketDays = await _alpacaService.GeOpenMarketDays(BACKLOG_MARKET_DAYS, date);

            // Get backlog market days plus the current date
            var daysThatNeedData = marketDays.Take(BACKLOG_DAYS + 1);
            var currentDate = daysThatNeedData.First();
            var lastBacklogDate = daysThatNeedData.Last();

            var allData = _alpacaService.AlpacaDataClient.ListHistoricalBarsAsync(
                new HistoricalBarsRequest("AAPL", lastBacklogDate.GetSessionOpenTimeUtc(), currentDate.GetTradingCloseTimeUtc(), new BarTimeFrame(5, BarTimeFrameUnit.Minute)));
        }

        public async Task OnBarReceived()
        {

        }

        public async Task OnTradePrice()
        {

        }
    }
}
