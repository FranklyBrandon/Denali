using Alpaca.Markets;
using Denali.Models.API;
using Denali.Services;
using Denali.TechnicalAnalysis;
using Microsoft.AspNetCore.Mvc;

namespace Denali.Dashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockDataController : ControllerBase
    {
        private readonly DataLayerComponent _dataLayerComponent;

        public StockDataController(DataLayerComponent dataLayerComponent)
        {
            _dataLayerComponent = dataLayerComponent;
        }

        [HttpGet]
        [Route("{symbol}")]
        public async Task<StockDataResponse> Get([FromRoute] string symbol, DateTime date)
        {
            _dataLayerComponent.InitializeDataClient();
            var fromDate = new DateTime(date.Year, date.Month, date.Day, 13, 30, 0, DateTimeKind.Utc);
            var toDate = new DateTime(date.Year, date.Month, date.Day, 20, 0, 0, DateTimeKind.Utc);
            var request = new HistoricalBarsRequest(symbol, fromDate, toDate, BarTimeFrame.Minute);
            var stockData = await _dataLayerComponent.GetAggregateDataMulti(new List<string> { symbol }, fromDate, toDate, BarTimeFrame.Minute);
            var fastEma = new ExponentialMovingAverage(3);
            fastEma.AnalyzeAll(stockData.First().Value);
            var slowEma = new ExponentialMovingAverage(8);
            slowEma.AnalyzeAll(stockData.First().Value);
            return new StockDataResponse(stockData, fastEma.MovingAverages, slowEma.MovingAverages);
        }
    }
}
