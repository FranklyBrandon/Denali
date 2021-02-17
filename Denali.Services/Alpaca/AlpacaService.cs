using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Services.Alpaca
{
    public class AlpacaService
    {
        public AlpacaTradingClient TradingClient { get; private set; }

        private readonly AlpacaSettings _settings;
        public AlpacaService(AlpacaSettings settings)
        {
            this._settings = settings;
        }

        public void InitializeTradingClient()
        {
            if (TradingClient != null)
            {
                TradingClient.Dispose();
                TradingClient = null;
            }

            var config = new AlpacaTradingClientConfiguration
            {
                ApiEndpoint = new Uri(_settings.MarketUrl),
                SecurityId = new SecretKey(_settings.APIKey, _settings.APISecret)
            };

            TradingClient = new AlpacaTradingClient(config);
        }
    }
}
