using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Services.Data.Alpaca
{
    public class AlpacaClientSettings
    {
        public const string Key = "Alpaca";
        public const string IdKey = "APCA-API-KEY-ID";
        public const string SecretKey = "APCA-API-SECRET-KEY";
        public string APIKey { get; set; }
        public string APISecretKey { get; set; }
        public string DataUrl { get; set; }
        public string MarketUrl { get; set; }
        public string BarsPath { get; set; }
    }
}
