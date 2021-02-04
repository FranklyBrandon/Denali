using Denali.Models.Polygon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Denali.Algorithms.Test
{
    public static class AggregateDataHelper
    {
        public const string AAPL_STOCK_DATA = "StockDataFile1.txt";

        public static AggregateResponse GetStockDataFile(string dataFile)
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), dataFile);
            string text = File.ReadAllText(path);
            return JsonSerializer.Deserialize<AggregateResponse>(text);
        }
    }
}
