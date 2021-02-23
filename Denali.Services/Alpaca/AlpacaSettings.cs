using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Services.Alpaca
{
    public class AlpacaSettings
    {
        public const string Key = "Alpaca";

        public string MarketUrl { get; set; }
        public string APIKey { get; set; }
        public string APISecret { get; set; }
        public string DataStreamingUrl { get; set; }
        public string TradingStreamingURL { get; set; }
        public string DataUrl { get; set; }
    }
}
