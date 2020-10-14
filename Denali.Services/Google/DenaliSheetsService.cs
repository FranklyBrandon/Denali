using Denali.Models.Trading;
using Denali.Services.Utility;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.Collections.Generic;
using System.Linq;

namespace Denali.Services.Google
{
    public class DenaliSheetsService
    {
        private readonly GoogleSheetsService _sheetsService;
        private readonly DenaliSettings _denaliSettings;
        private readonly TimeUtils _timeUtils;
        private int _rowIndex;

        public DenaliSheetsService(GoogleSheetsService sheetsService, DenaliSettings denaliSettings, TimeUtils timeUtils)
        {
            this._sheetsService = sheetsService;
            this._denaliSettings = denaliSettings;
            this._timeUtils = timeUtils;
        }

        public Spreadsheet WriteDenaliSheet(string title, string symbols, string startDateTime, string endDateTime, string resolution, List<Position> positions)
        {
            var sheet = _sheetsService.CreateSheet(title);

            _sheetsService.UpdateSheet(sheet.SpreadsheetId
                , new List<ValueRange> {
                     CreateMetadata(symbols, startDateTime, endDateTime, resolution)
                    , CreatePositionsHeader()
                    , CreatePositions(positions) }
                , "RAW");

            if (positions != null && positions.Any())
            {
                _sheetsService.UpdateSheet(sheet.SpreadsheetId
                    , new List<ValueRange> { AddTotalSummation(4 + positions.Count()) }
                    , "USER_ENTERED");
            }

            _sheetsService.UpdateSheetWithPositiveNegativeFormat(sheet.SpreadsheetId);

            return sheet;
        }
        public void AppendPositions(string spreadsheetId, List<Position> positions)
        {
            var positionsRange = CreatePositions(positions);
            _sheetsService.UpdateSheet(spreadsheetId, new List<ValueRange> { positionsRange }, "RAW");

            _sheetsService.UpdateSheet(spreadsheetId
                , new List<ValueRange> { AddTotalSummation(4 + positions.Count()) }
                , "USER_ENTERED");

        }
        public IEnumerable<StockSymbol> GetTradingSettings()
        {
            var fetchRange = "Sheet1!A:B";
            var values = _sheetsService.GetFromSheet(_denaliSettings.WorkerSheetId, fetchRange);
            values.RemoveAt(0);
            return values.Select(x => new StockSymbol(x[0], x[1]));
        }
        private ValueRange AddTotalSummation(int index)
        {
            //Write to next row and add buffer
            var totalIndex = index + 1;

            var values = new List<object> { "Per Share Total:", $"=SUM(G5:G{index})" };
            var data = new List<IList<object>> { values };

            var totalRange = new ValueRange();
            totalRange.MajorDimension = "ROWS";
            totalRange.Values = data;
            totalRange.Range = $"Sheet1!F{totalIndex}:G{totalIndex}";

            return totalRange;
        }
        private ValueRange CreateMetadata(string symbols, string startDateTime, string endDateTime, string resolution)
        {
            //Metadata starts at the top of sheet
            _rowIndex = 1;

            //Metadata Body
            var metaDataHeaders = new List<object> { "Symbol", "Start Time", "End Time", "Resolution" };
            var metaDataValues = new List<object> { string.Concat(symbols, ','), startDateTime, endDateTime, resolution };
            var data = new List<IList<object>> { metaDataHeaders, metaDataValues };

            //MetaData Range
            var metadataRange = new ValueRange();
            metadataRange.MajorDimension = "ROWS";
            metadataRange.Values = data;
            metadataRange.Range = $"Sheet1!{_rowIndex}:{++_rowIndex}";

            return metadataRange;
        }
        private ValueRange CreatePositionsHeader()
        {
            _rowIndex = 4;

            //Positions Header
            var positionHeaders = new List<object> { "Symbol", "Signal", "Open Time", "Close Time", "Open Price", "Close Price", "Profit" };
            var positionsHeadersData = new List<IList<object>>();
            positionsHeadersData.Add(positionHeaders);

            //Positions Range
            var positionsRange = new ValueRange();
            positionsRange.MajorDimension = "ROWS";
            positionsRange.Values = positionsHeadersData;
            positionsRange.Range = $"Sheet1!{_rowIndex}:{_rowIndex}";

            return positionsRange;
        }
        private ValueRange CreatePositions(List<Position> positions)
        {
            if (positions == null || !positions.Any())
                return default;

            //Write to next row;
            var index = 5;

            //Positions Body
            var positionValues = positions.Select(x => new List<object>
            { x.Symbol
                , x.Signal.Type.ToString()
                , _timeUtils.GetNewYorkTimeFromEpoch(x.OpenTimestamp).LocalTime.ToString("g")
                , _timeUtils.GetNewYorkTimeFromEpoch(x.CloseTimestamp).LocalTime.ToString("g")
                , x.OpenPrice
                , x.ClosePrice
                , x.Profit });

            var positionsData = new List<IList<object>>();
            positionsData.AddRange(positionValues);

            //Positions Range
            var positionsRange = new ValueRange();
            positionsRange.MajorDimension = "ROWS";
            positionsRange.Values = positionsData;
            positionsRange.Range = $"Sheet1!{index}:{index + positions.Count() - 1}";

            return positionsRange;
        }
    } 
}
