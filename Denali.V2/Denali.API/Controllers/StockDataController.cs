using Alpaca.Markets;
using Denali.Models;
using Denali.Models.API;
using Denali.Processors.DenaliClimbStrategy;
using Denali.Services;
using Denali.TechnicalAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;

namespace Denali.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockDataController : ControllerBase
    {
        private readonly DataLayerComponent _dataLayerComponent;
        private readonly AggregateDataService _aggregateDataService;
        private readonly GapUpStreamer _streamer;

        public StockDataController(DataLayerComponent dataLayerComponent, AggregateDataService aggregateDataService, GapUpStreamer streamer)
        {
            _dataLayerComponent = dataLayerComponent;
            _aggregateDataService = aggregateDataService;
            _streamer = streamer;
        }

        [HttpGet]
        [Route("{symbol}")]
        public async Task<Dictionary<string, List<IBar>>> Get([FromRoute] string symbol, DateTime start, DateTime? end, string timeFrame) => 
            await _aggregateDataService.GetAggregates(new List<string> { symbol }, start, end, timeFrame);

            //_dataLayerComponent.InitializeDataClient();
            /*
            var fromDate = new DateTime(date.Year, date.Month, date.Day, 13, 30, 0, DateTimeKind.Utc);
            var toDate = new DateTime(date.Year, date.Month, date.Day, 20, 0, 0, DateTimeKind.Utc);


            //var startTime = new DateTime(date.Year, date.Month, date.Day, 14, 0, 0, DateTimeKind.Utc);

            //await _streamer.InitializeMetrics(new List<string> { symbol }, startTime.AddMinutes(-1), fromDate);

            //var request = new HistoricalBarsRequest(symbol, startTime, toDate, BarTimeFrame.Minute);
            var streamData = await _dataLayerComponent.GetAggregateDataMulti(new List<string> { symbol }, fromDate, toDate, BarTimeFrame.Minute);

            var slowEma = new ExponentialMovingAverage(8);
            slowEma.AnalyzeAll(streamData[symbol]);
            var fastEma = new ExponentialMovingAverage(3);
            fastEma.AnalyzeAll(streamData[symbol]);
            /*
            var entrySignals = new List<DenaliClimbEntrySignal>();
            _streamer.OnEntryAction = async (DenaliClimbEntrySignal entrySignal) => { entrySignals.Add(entrySignal); };

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



            return new StockDataResponse(
                _streamer.StreamedData, 
                _streamer.FastEMA.ToDictionary(x => x.Key, x => x.Value.MovingAverages), 
                _streamer.SlowEMA.ToDictionary(x => x.Key, x => x.Value.MovingAverages), 
                entrySignals);
            */

            /*
            return new StockDataResponse(
                streamData,
                new Dictionary<string, IList<EMA>> { { symbol, fastEma.MovingAverages} },
                new Dictionary<string, IList<EMA>> { { symbol, slowEma.MovingAverages } },
                null);
            */
        
    }
}
