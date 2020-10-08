using Denali.Services.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Denali.Services.Utility;
using Denali.Models.Data.Trading;
using Denali.Services.Data;
using Microsoft.Extensions.Logging;
using Denali.Algorithms.Bar;
using Denali.Services.Market;

namespace Denali.Processors
{
    public class LiveTradingProcessor : IProcessor
    {
        private readonly DenaliSheetsService _sheetsService;
        private readonly IConfiguration _configuration;
        private readonly TimeUtils _timeUtils;
        private readonly IMarketDataProvider _dataProvider;
        private readonly ILogger<LiveTradingProcessor> _logger;
        private readonly BarAlgorithmAnalysis _barAlgorithmAnalysis;
        private readonly IMarketService _marketService;
        private string _spreadSheetId;
        public LiveTradingProcessor(DenaliSheetsService sheetsService
            , IConfiguration configuration
            , TimeUtils timeUtils
            , IMarketDataProvider dataProvider
            , BarAlgorithmAnalysis barAlgorithmAnalysis
            , ILogger<LiveTradingProcessor> logger)
        {
            this._sheetsService = sheetsService;
            this._configuration = configuration;
            this._timeUtils = timeUtils;
            this._dataProvider = dataProvider;
            this._barAlgorithmAnalysis = barAlgorithmAnalysis;
            this._logger = logger;
        }

        public async Task Process(CancellationToken stoppingToken)
        {
            //Fetch trading stocks
            var symbols = _sheetsService.GetTradingSettings().Where(x => x.Trading).Select(x => x.Symbol);
            if (!symbols.Any())
                return;

            //Write inital Report
            _spreadSheetId = _sheetsService.WriteDenaliSheet(
                $"Denali {_configuration["status"]}-{_timeUtils.GetNYSEDateTime().ToString("g")}"
                , String.Join(',', symbols)
                , _timeUtils.GetNYSEDateTime().ToString("g")
                , "--"
                , "--"
                , null).SpreadsheetId;

            ProcessTick(symbols);

            var timer = new System.Timers.Timer();
            timer.Interval = 15000;
            timer.Elapsed += delegate { ProcessTick(symbols);  };
            timer.Start();

            while (!stoppingToken.IsCancellationRequested)
            {

            }

            timer.Stop();
            timer.Dispose();
            //var pos1 = new Position("AAPL", 5.01, new Signal(SignalType.BullishEngulfing, Models.Data.Trading.Action.Long, 1,1,0,0));
            //pos1.Profit = 1;
            //_sheetsService.AppendPositions(_spreadSheetId, new List<Position> { pos1 });
            //_sheetsService.AppendPositions(_spreadSheetId, new List<Position> { pos1, pos1 });
            //_sheetsService.AppendPositions(_spreadSheetId, new List<Position> { pos1, pos1, pos1 });


        }

        private async void ProcessTick(IEnumerable<string> symbols)
        {
            _logger.LogInformation("Processor is ticking...");
            var start = _timeUtils.GetNYSEOpenISO();
            var end = _timeUtils.GetNYSECloseISO();
            var bars = await _dataProvider.GetBarData(resolution: "1Min", start: start, end: end, symbols: symbols.ToArray());

            var entryActions = _barAlgorithmAnalysis.Analyze(bars);
            var resultActions = _marketService.ManagePositions(bars, entryActions);
            //Update sheets with result actions       
        }
    }
}
