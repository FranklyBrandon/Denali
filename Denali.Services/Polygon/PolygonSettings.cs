using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Services.Polygon
{
    public class PolygonSettings
    {
        public const string Key = "Polygon";
        public string APIKey { get; set; }
        public string APIUrl { get; set; }
        public string AggregatePath { get; set; }
        public string HistoricQuotesPath { get; set; }
        public string WebsocketUrl { get; set; }
    }
}
