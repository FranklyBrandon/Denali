using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.TradingView
{
    public class TradingViewMessageBuilder
    {
        private readonly string _chartSessionId;
        private readonly string _quoteSessionId;

        public TradingViewMessageBuilder()
        {
            _chartSessionId = GenerateSessionId("cs");
            _quoteSessionId = GenerateSessionId("qs");
        }
        public static string BuildMessage(string messageId, string payload) =>
            AddHeader(
            $@"
                {{
                    ""m"":""{messageId}"",
                    ""p"":[{payload}]
                }}
            ");
        public static string SetUnathorizedToken() =>
            AddHeader(
            @"
                {
                    ""m"":""set_auth_token"",
                    ""p"":[""unauthorized_user_token""]
                }
            ");

        public static string CreateChartSession(string chartSessionId) =>
            AddHeader(
            $@"
                {{
                    ""m"":""chart_create_session"",
                    ""p"":[""{chartSessionId}"",""""]
                }}
            ");

        public static string CreateQuoteSession(string quoteSessionId) =>
            AddHeader(
            $@"
                {{
                    ""m"":""quote_create_session"",
                    ""p"":[""{quoteSessionId}""]
                }}
            ");

        public static string SetLocale() =>
            AddHeader(
            $@"
                {{
                    ""m"":""set_locale"",
                    ""p"":[""en"",""US""]
                }}
            ");

        private static string AddHeader(string message)
        {
            return $"~m~{message.Length}~m~{message}";
        }

        private string GenerateSessionId(string prefix) =>
            $"{prefix}_{GenerateRandomTwelveChars()}";

        private string GenerateRandomTwelveChars() =>
             Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 12);
    }
}
