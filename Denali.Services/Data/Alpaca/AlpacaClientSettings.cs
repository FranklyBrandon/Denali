using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Services.Data.Alpaca
{
    public class AlpacaClientSettings
    {
        public const string Key = "Alpaca";
        public string APIKey { get; set; }
        public string APISecretKey { get; set; }
        public string DataUrl { get; set; }
        public string MarketUrl { get; set; }
        public string BarsPath { get; set; }
    }
}
