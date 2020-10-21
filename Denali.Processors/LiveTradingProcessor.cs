using Denali.Services.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Denali.Services.Utility;
using Microsoft.Extensions.Logging;
using Denali.Algorithms.Bar;
using Denali.Services.Market;
using Denali.Services.Polygon;

namespace Denali.Processors
{
    public class LiveTradingProcessor : IProcessor
    {
        private readonly DenaliSheetsService _sheetsService;
        private readonly IConfiguration _configuration;
        private readonly TimeUtils _timeUtils;
        private readonly ILogger<LiveTradingProcessor> _logger;
        private readonly PolygonStreamingClient _polygonStreamingClient;
        private string _spreadSheetId;
        public LiveTradingProcessor(DenaliSheetsService sheetsService
            , IConfiguration configuration
            , TimeUtils timeUtils
            , PolygonStreamingClient polygonStreamingClient
            , ILogger<LiveTradingProcessor> logger)
        {
            this._sheetsService = sheetsService;
            this._configuration = configuration;
            this._timeUtils = timeUtils;
            this._polygonStreamingClient = polygonStreamingClient;
            this._logger = logger;
        }

        public async Task Process(CancellationToken stoppingToken)
        {
            //Fetch trading stocks
            var symbols = _sheetsService.GetTradingSettings().Where(x => x.Trading).Select(x => x.Symbol);
            if (!symbols.Any())
                return;

            //Write inital Report
            //_spreadSheetId = _sheetsService.WriteDenaliSheet(
            //    $"Denali {_configuration["status"]}-{_timeUtils.GetNYSEDateTime().ToString("g")}"
            //    , String.Join(',', symbols)
            //    , _timeUtils.GetNYSEDateTime().ToString("g")
            //    , "--"
            //    , "--"
            //    , null).SpreadsheetId;


            //var pos1 = new Position("AAPL", 5.01, new Signal(SignalType.BullishEngulfing, Models.Data.Trading.Action.Long, 1,1,0,0));
            //pos1.Profit = 1;
            //_sheetsService.AppendPositions(_spreadSheetId, new List<Position> { pos1 });
            //_sheetsService.AppendPositions(_spreadSheetId, new List<Position> { pos1, pos1 });
            //_sheetsService.AppendPositions(_spreadSheetId, new List<Position> { pos1, pos1, pos1 });

            await _polygonStreamingClient.ConnectToPolygonStreams(CancellationToken.None);
        }

        //private async Task ProcessTick(IEnumerable<string> symbols)
        //{
        //    _logger.LogInformation("Processor is ticking...");
        //    var start = _timeUtils.GetNYSEOpenISO();

        //    var end = _timeUtils.GetNYSECloseISO();
        //    var bars = await _dataProvider.GetBarData(resolution: "1Min", start: start, end: end, symbols: symbols.ToArray());

        //    var entryActions = _barAlgorithmAnalysis.Analyze(bars);
        //    var resultActions = _marketService.ManagePositions(bars, entryActions);
        //    //Update sheets with result actions       
        //}
    }
}
