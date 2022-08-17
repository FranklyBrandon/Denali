using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Services.YahooFinanceService
{
    public class YahooFinanceClientSettings
    {
        public static string Settings = "YahooFinanceClientSettings";

        public string BasePath { get; set; }
        public string QuotePath { get; set; }
    }
}
