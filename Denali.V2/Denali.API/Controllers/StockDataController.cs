using Alpaca.Markets;
using Denali.Models.API;
using Denali.Processors.DenaliClimbStrategy;
using Denali.Services;
using Denali.TechnicalAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace Denali.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockDataController : ControllerBase
    {
        private readonly DataLayerComponent _dataLayerComponent;
        private readonly GapUpStreamer _streamer;

        public StockDataController(DataLayerComponent dataLayerComponent, GapUpStreamer streamer)
        {
            _dataLayerComponent = dataLayerComponent;
            _streamer = streamer;
        }

        [HttpGet]
        [Route("{symbol}")]
        public async Task<StockDataResponse> Get([FromRoute] string symbol, DateTime date)
        {
            _dataLayerComponent.InitializeDataClient();
            var fromDate = new DateTime(date.Year, date.Month, date.Day, 13, 30, 0, DateTimeKind.Utc);
            var toDate = new DateTime(date.Year, date.Month, date.Day, 20, 0, 0, DateTimeKind.Utc);
            var startTime = new DateTime(date.Year, date.Month, date.Day, 14, 0, 0, DateTimeKind.Utc);

            await _streamer.InitializeMetrics(new List<string> { symbol }, startTime.AddMinutes(-1), fromDate);

            var request = new HistoricalBarsRequest(symbol, startTime, toDate, BarTimeFrame.Minute);
            var streamData = await _dataLayerComponent.GetAggregateDataMulti(new List<string> { symbol }, fromDate, toDate, BarTimeFrame.Minute);

            var entrySignals = new List<EntrySignal>();
            _streamer.OnEntryAction = async (EntrySignal entrySignal) => { entrySignals.Add(entrySignal); };

            var totalMinutes = (toDate - startTime).TotalMinutes;
            for (int i = 0; i < totalMinutes; i++)
            {
                var time = startTime.AddMinutes(i);
                var dataFrames = streamData.SelectMany(x => x.Value.Where(x => x.TimeUtc == time));

                foreach (var frame in dataFrames)
                {
                    _streamer.OnStreamedData(frame);
                }
            }



            /*
            var fastEma = new ExponentialMovingAverage(8);
            fastEma.AnalyzeAll(streamData.First().Value);
            var slowEma = new ExponentialMovingAverage(21);
            slowEma.AnalyzeAll(streamData.First().Value);
            */
            return new StockDataResponse(_streamer.StreamedData, _streamer.FastEMA[symbol].MovingAverages, _streamer.SlowEMA[symbol].MovingAverages);
        }
    }
}
