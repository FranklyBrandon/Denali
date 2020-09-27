using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Services.Alpaca
{
    public class AlpacaClientSettings
    {
        public const string Key = "Alpaca";
        public string APIKey { get; set; }
        public string APISecretKey { get; set; }
        public string BarsPath { get; set; }
    }
}
