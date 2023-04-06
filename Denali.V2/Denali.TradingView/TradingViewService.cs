using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Denali.TradingView
{
    public class TradingViewService
    {
        private readonly TextWebSocket _websocket;
        private readonly TradingViewSettings _settings;
        private readonly string _chartSessionId;
        private readonly string _quoteSessionId;
        private readonly Regex _heartbeatRegex;

        public TradingViewService(TradingViewSettings settings = default)
        {
            _settings = settings ??= new TradingViewSettings();
            _websocket = new TextWebSocket(_settings.MessageBufferSize);
            _settings = settings;
            _heartbeatRegex = new Regex(@"[~][h][~]\d*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            _chartSessionId = GenerateSessionId("cs");
            _quoteSessionId = GenerateSessionId("qs");
        }

        public async Task ConnectToTradingView(CancellationToken cancellationToken = default)
        {
            _websocket.OnMessage += OnMessage;
            await _websocket.Connect(new Uri(_settings.TradingViewWebSocketURL), Headers(), cancellationToken);
            await _websocket.SendAsync(TradingViewMessages.SetUnathorizedToken);
            await _websocket.SendAsync(TradingViewMessages.SetLocale);
            await _websocket.SendAsync(TradingViewMessages.ChartCreateSession(_chartSessionId));
            await _websocket.SendAsync(TradingViewMessages.ChartSwitchTimeZones(_chartSessionId));
            await _websocket.SendAsync(TradingViewMessages.QuoteCreateSession(_quoteSessionId));
            await _websocket.SendAsync(TradingViewMessages.QuoteSetFields(_quoteSessionId));
            await _websocket.SendAsync(TradingViewMessages.QuoteAddSymbols(_quoteSessionId, "AMEX", "SPY"));
            await _websocket.SendAsync(TradingViewMessages.QuoteFastSymbols(_quoteSessionId, "AMEX", "SPY"));
            await _websocket.SendAsync(TradingViewMessages.ChartResolveSymbol(_chartSessionId, "AMEX", "SPY"));
            //await _websocket.SendAsync(TradingViewMessages.ChartCreateSeries(_chartSessionId));
        }

        private async void OnMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Received:");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);

            if (_heartbeatRegex.IsMatch(message))
                await _websocket.SendAsync(message);
        }
        private string GenerateSessionId(string prefix) =>
            $"{prefix}_{GenerateRandomTwelveChars()}";

        private string GenerateRandomTwelveChars() =>
             Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 12);

        private IDictionary<string, string> Headers() =>
            new Dictionary<string, string>
            {
                {"Origin", _settings.Origin }
            };
    }
}
