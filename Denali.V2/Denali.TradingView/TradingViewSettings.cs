using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.TradingView
{
    public class TradingViewSettings
    {
        public string TradingViewWebSocketURL { get; set; } = "wss://data.tradingview.com/socket.io/websocket";
        public int MessageBufferSize { get; set; } = 256;
        public string Origin { get; set; } = "https://www.tradingview.com";
    }
}
