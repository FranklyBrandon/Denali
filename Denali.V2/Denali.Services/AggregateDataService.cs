using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Denali.Services
{
    public class AggregateDataService
    {
        private readonly DataLayerComponent _dataLayer;

        public AggregateDataService(DataLayerComponent dataLayer)
        {
            _dataLayer = dataLayer;
        }

        public async Task<Dictionary<string, List<IBar>>> GetAggregates(IEnumerable<string> symbols, DateTime start, DateTime? end, string timeFrame)
        {
            start = new DateTime(start.Year, start.Month, start.Day, 13, 30, 0, DateTimeKind.Utc);
            if (end.HasValue)
                end = new DateTime(end.Value.Year, end.Value.Month, end.Value.Day, 20, 0, 0, DateTimeKind.Utc);
            else
                end = new DateTime(start.Year, start.Month, start.Day, 20, 0, 0, DateTimeKind.Utc);

            return await _dataLayer.GetAggregateDataMulti(symbols, start, end.Value, GetTimeFrame(timeFrame));
        }

        private BarTimeFrame GetTimeFrame(string timeFrame)
        {
            // Split a string into a number followed by letters 
            string[] parts = Regex.Split(timeFrame, @"(?<=\d)(?=[A-Za-z])");
            if (parts.Length != 2)
                throw new ArgumentException("Invalid time frame string");

            if (!int.TryParse(parts[0], out int value))
                value = 1;

            BarTimeFrameUnit unit;
            switch (parts[1])
            {
                case "Min":
                    unit = BarTimeFrameUnit.Minute;
                break;
                case "D":
                    unit = BarTimeFrameUnit.Day;
                break;
                default:
                    unit = BarTimeFrameUnit.Minute;
                    break;
            }

            return new BarTimeFrame(value, unit);
        }
    }
}
