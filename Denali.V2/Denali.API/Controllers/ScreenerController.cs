using Alpaca.Markets;
using Denali.Processors.DenaliClimbStrategy;
using Denali.Services;
using Microsoft.AspNetCore.Mvc;

namespace Denali.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScreenerController : ControllerBase
    {
        private readonly DataLayerComponent _dataLayer;
        private readonly GapUpScreener _gapUpScreener;

        public ScreenerController(DataLayerComponent dataLayerComponent, GapUpScreener gapUpScreener)
        {
            _dataLayer = dataLayerComponent;
            _gapUpScreener = gapUpScreener;
        }

        [HttpGet]
        public async Task<Dictionary<string, decimal>> Get(DateTime date)
        {
            await _dataLayer.Initialize();
            var marketBacklogDays = await _dataLayer.GetPastMarketDays(date, 4);
            var previousMarketDay = marketBacklogDays.ElementAt(marketBacklogDays.Count() - 2);
            var currentMarketDay = marketBacklogDays.Last();
            var assets = await _dataLayer.GetAllTradableAssets();

            return await _gapUpScreener.GetGapUpBetween(previousMarketDay.GetTradingCloseTimeUtc(), currentMarketDay.GetTradingOpenTimeUtc().AddMinutes(-3), assets, 10m, 3);
        }
    }
}
