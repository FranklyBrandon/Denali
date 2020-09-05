
using System;

namespace Denali.Services.FinnHub
{
    public class FinnHubClientSettings
    {
        public const string Key = "FinnHub";

        public string BaseUrl { get; set; }
        public string APIKey { get; set; }
        public string CandlePath { get; set; }
    }
}
