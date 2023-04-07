using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.TradingView
{
    public static class TradingViewMessages
    {
        private static string AddHeader(string message) =>
            $"~m~{message.Length}~m~{message}";

        public static string SetUnathorizedToken =>
            AddHeader("{\"m\":\"set_auth_token\",\"p\":[\"unauthorized_user_token\"]}");

        public static string SetLocale =>
            AddHeader("{\"m\":\"set_locale\",\"p\":[\"en\",\"US\"]}");

        public static string ChartCreateSession(string chartSessionId) =>
            AddHeader($"{{\"m\":\"chart_create_session\",\"p\":[\"{chartSessionId}\",\"\"]}}");

        public static string QuoteCreateSession(string quoteSessionId) =>
            AddHeader($"{{\"m\":\"quote_create_session\",\"p\":[\"{quoteSessionId}\"]}}");

        public static string QuoteAddSymbols(string quoteSessionId, string exchange, string symbol) =>
            AddHeader($"{{\"m\":\"quote_add_symbols\",\"p\":[\"{quoteSessionId}\",\"{exchange}:{symbol}\"]}}");

        public static string QuoteFastSymbols(string quoteSessionId, string exchange, string symbol) =>
            AddHeader($"{{\"m\":\"quote_fast_symbols\",\"p\":[\"{quoteSessionId}\",\"{exchange}:{symbol}\"]}}");

        public static string QuoteSetFields(string quoteSessionId) =>
            AddHeader($"{{\"m\":\"quote_set_fields\",\"p\":[\"{quoteSessionId}\",\"base-currency-logoid\",\"ch\",\"chp\",\"currency-logoid\",\"currency_code\",\"currency_id\",\"base_currency_id\",\"current_session\",\"description\",\"exchange\",\"format\",\"fractional\",\"is_tradable\",\"language\",\"local_description\",\"listed_exchange\",\"logoid\",\"lp\",\"lp_time\",\"minmov\",\"minmove2\",\"original_name\",\"pricescale\",\"pro_name\",\"short_name\",\"type\",\"typespecs\",\"update_mode\",\"volume\",\"value_unit_id\"]}}");

        public static string ChartSwitchTimeZones(string chartSessionId) =>
            AddHeader($"{{\"m\":\"switch_timezone\",\"p\":[\"{chartSessionId}\",\"Etc/UTC\"]}}");

        public static string ChartResolveSymbol(string chartSessionId, string exchange, string symbol) =>
            AddHeader($"{{\"m\":\"resolve_symbol\",\"p\":[\"{chartSessionId}\",\"sds_sym_1\",\"={{\\\"adjustment\\\":\\\"splits\\\",\\\"symbol\\\":\\\"{exchange}:{symbol}\\\"}}\"]}}");

        public static string ChartResolveSymbolExtended(string chartSessionId, string exchange, string symbol) =>
            AddHeader($"{{\"m\":\"resolve_symbol\",\"p\":[\"{chartSessionId}\",\"sds_sym_1\",\"={{\\\"adjustment\\\":\\\"splits\\\",\\\"currency-id\\\":\\\"USD\\\",\\\"session\\\":\\\"extended\\\",\\\"settlement-as-close\\\":false,\\\"symbol\\\":\\\"{exchange}:{symbol}\\\"}}\"]}}");

        public static string ChartCreateSeries(string chartSessionId, string timeframe = "1", int length = 300) =>
            AddHeader($"{{\"m\":\"create_series\",\"p\":[\"{chartSessionId}\",\"sds_1\",\"s1\",\"sds_sym_1\",\"{timeframe}\",{length},\"\"]}}");
    }
}
