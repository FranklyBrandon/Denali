using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Services.AlphaAdvantage
{
    public class AlphaAdvantageClientSettings
    {
        public static string Settings = "AlphaAdvantageClientSettings";

        public string BasePath { get; set; }
        public string QuotePath { get; set; }
        public string APIKey { get; set; }
    }
}
