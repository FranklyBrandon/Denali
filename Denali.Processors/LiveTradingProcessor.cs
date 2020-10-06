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

namespace Denali.Processors
{
    public class LiveTradingProcessor : IProcessor
    {
        private readonly DenaliSheetsService _sheetsService;
        private readonly IConfiguration _configuration;
        private readonly TimeUtils _timeUtils;
        private readonly IMarketDataProvider _dataProvider;
        private string _spreadSheetId;
        public LiveTradingProcessor(DenaliSheetsService sheetsService, IConfiguration configuration, TimeUtils timeUtils, IMarketDataProvider dataProvider)
        {
            this._sheetsService = sheetsService;
            this._configuration = configuration;
            this._timeUtils = timeUtils;
            this._dataProvider = dataProvider;
        }

        public async Task Process(CancellationToken stoppingToken)
        {
            //Fetch trading stocks
            var symbols = _sheetsService.GetTradingSettings().Where(x => x.Trading);
            if (!symbols.Any())
                return;

            //Write inital Report
            _spreadSheetId = _sheetsService.WriteDenaliSheet(
                $"Denali {_configuration["status"]}-{_timeUtils.GetNYSEDateTime().ToString("g")}"
                , String.Join(',', symbols.Select(x => x.Symbol))
                , _timeUtils.GetNYSEDateTime().ToString("g")
                , "--"
                , "--"
                , null).SpreadsheetId;



            //var pos1 = new Position("AAPL", 5.01, new Signal(SignalType.BullishEngulfing, Models.Data.Trading.Action.Long, 1,1,0,0));
            //pos1.Profit = 1;
            //_sheetsService.AppendPositions(_spreadSheetId, new List<Position> { pos1 });
            //_sheetsService.AppendPositions(_spreadSheetId, new List<Position> { pos1, pos1 });
            //_sheetsService.AppendPositions(_spreadSheetId, new List<Position> { pos1, pos1, pos1 });


        }
    }
}
