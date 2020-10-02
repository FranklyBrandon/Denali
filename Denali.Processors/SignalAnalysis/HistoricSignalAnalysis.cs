using Denali.Models.Data.Alpaca;
using Denali.Models.Data.Trading;
using Denali.Processors.SignalAnalysis.Signals;
using Denali.Services.Alpaca;
using Denali.Services.Google;
using Denali.Services.Utility;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors.SignalAnalysis
{
    public class HistoricSignalAnalysis : IProcessor
    {
        private readonly AlpacaService _alpacaService;
        private readonly ISignalAlgo _signalAlgo;
        private readonly List<Position> _positions;
        private readonly GoogleSheetsService _sheetsService;
        private readonly TimeUtils _timeUtils;
        public HistoricSignalAnalysis(AlpacaService alpacaService, ISignalAlgo signalAlgo)
        {
            _alpacaService = alpacaService;
            _signalAlgo = signalAlgo;
            _positions = new List<Position>();
            _sheetsService = new GoogleSheetsService();
            _timeUtils = new TimeUtils();
        }

        public async Task Process(Dictionary<string, string> arguments)
        {
            var barChart = await GetBarData(arguments);

            foreach (var symbol in barChart)
            {
                var max = symbol.Value.Count;
                for (int i = 0; i < max; i++)
                {
                    var currentCandle = symbol.Value[i];
                    ManagePositions(currentCandle);
                    var signal = _signalAlgo.Process(symbol.Value, i, (i == 0), (i == max));
                    if (signal != null)
                        ExecuteSignal(symbol.Key, signal, currentCandle);
                }
            }

            WriteResults(arguments);
        }

        private async Task<Dictionary<string, List<Candle>>> GetBarData(Dictionary<string, string> arguments)
        {
            var resolution = arguments["resolution"];
            var start = arguments["start"];
            var end = arguments["end"];
            var symbols = arguments["symbols"];
            return await _alpacaService.GetBarData(resolution: resolution, start: start, end: end, symbols: symbols);
        }

        private void ExecuteSignal(string symbol, Signal signal, Candle candle)
        {
            if (signal.Action == Models.Data.Trading.Action.Long)
            {
                Console.WriteLine($"{signal.Type.ToString()} Pattern: Buy at {candle.ClosePrice}");
                _positions.Add(new Position(symbol, candle.ClosePrice, signal));
            }
        }

        private void ManagePositions(Candle currentCandle)
        {
            foreach (var position in _positions)
            {
                if (!position.Open)
                    continue;

                if (currentCandle.ClosePrice >= position.Signal.Exit)
                {
                    position.Close(currentCandle.Timestamp, currentCandle.ClosePrice);
                    Console.WriteLine($"Cash out for a profit of {position.Profit}");
                }
                else if (currentCandle.ClosePrice <= position.Signal.StopLoss)
                {
                    position.Close(currentCandle.Timestamp, currentCandle.ClosePrice);
                    Console.WriteLine($"Stop loss for a loss of {position.Profit}");
                }
            }
        }

        private void WriteResults(Dictionary<string, string> arguments)
        {
            //Create Spreadsheet
            var sheet = _sheetsService.CreateSheet(DateTime.Now.ToString("g"));

            //Metadata Body
            var metaDataHeaders = new List<object> { "Symbol", "Start Time", "End Time", "Resolution" };
            var metaDataValues = new List<object> { arguments["symbols"], arguments["start"], arguments["end"], arguments["resolution"] };
            var data = new List<IList<object>> { metaDataHeaders, metaDataValues };

            //MetaData Range
            var metadataRange = new ValueRange();
            metadataRange.MajorDimension = "ROWS";
            metadataRange.Values = data;
            metadataRange.Range = "Sheet1!1:2";

            //Positions Body
            var positionHeaders = new List<object> { "Signal", "Open Time", "Close Time", "Open Price", "Close Price", "Profit" };
            var positionValues = _positions.Select(x => new List<object> 
                { x.Signal.Type.ToString(), _timeUtils.GetNewYorkTimeFromEpoch(x.OpenTimestamp).LocalTime.ToString("g"), _timeUtils.GetNewYorkTimeFromEpoch(x.CloseTimestamp).LocalTime.ToString("g"), x.OpenPrice, x.ClosePrice, x.Profit });
            var positionsData = new List<IList<object>>();
            positionsData.Add(positionHeaders);
            positionsData.AddRange(positionValues);

            //Positions Range
            var positionsRange = new ValueRange();
            positionsRange.MajorDimension = "ROWS";
            positionsRange.Values = positionsData;
            positionsRange.Range = $"Sheet1!4:{4 + positionValues.Count()}";

            //Batch Request
            var request = new BatchUpdateValuesRequest();
            request.Data = new List<ValueRange> { metadataRange, positionsRange };
            request.ValueInputOption = "RAW";

            _sheetsService.UpdateSheet(sheet.SpreadsheetId, request);

            
        }
    }
}
