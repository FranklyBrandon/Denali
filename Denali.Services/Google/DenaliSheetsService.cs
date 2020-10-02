using Denali.Models.Data.Trading;
using Denali.Services.Utility;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;

namespace Denali.Services.Google
{
    public class DenaliSheetsService
    {
        private readonly GoogleSheetsService _sheetsService;
        private readonly TimeUtils _timeUtils;
        private int _rowIndex;

        public DenaliSheetsService(GoogleSheetsService sheetsService)
        {
            this._sheetsService = sheetsService;
        }

        public Spreadsheet WriteDenaliSheet(string title, string symbols, string startDateTime, string endDateTime, string resolution, List<Position> positions)
        {
            var sheet = _sheetsService.CreateSheet(title);
            var metadataRange = CreateMetadata(symbols, startDateTime, endDateTime, resolution);
            var positionHeadersRange = CreatePositionsHeader();
            var positionsRange = CreatePositions(positions);

            _sheetsService.UpdateSheet(sheet.SpreadsheetId
                , new List<ValueRange> { metadataRange, positionHeadersRange, positionsRange }
                , "RAW");

            return sheet;
        }

        public void AppendPositions(string spreadsheetId, List<Position> positions)
        {
            //Write to next row;
            _rowIndex++;
            var positionsRange = CreatePositions(positions);
            _sheetsService.UpdateSheet(spreadsheetId, new List<ValueRange> { positionsRange }, "RAW");
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



            //Add buffer after metadata to create seperation
            _rowIndex++;

            return metadataRange;
        }

        private ValueRange CreatePositions(List<Position> positions)
        {
            //Write to next row;
            _rowIndex++;

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
            positionsRange.Range = $"Sheet1!{_rowIndex}:{_rowIndex + positionValues.Count()}";

            return positionsRange;
        }

        private ValueRange CreatePositionsHeader()
        {
            //Write to next row;
            _rowIndex++;

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
    }
}
