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
        public AlpacaStreamingClient StreamingClient { get; private set; }

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

        public void InitializeStreamingClient()
        {
            if (StreamingClient != null)
            {
                StreamingClient.Dispose();
                StreamingClient = null;
            }

            var config = new AlpacaStreamingClientConfiguration
            {
                ApiEndpoint = new Uri(_settings.StreamingUrl),
                SecurityId = new SecretKey(_settings.APIKey, _settings.APISecret)
            };

            StreamingClient = new AlpacaStreamingClient(config);
        }

        public async Task Disconnect()
        {
            if (StreamingClient != null)
            {
                await StreamingClient.DisconnectAsync();
                StreamingClient.Dispose();
                StreamingClient = null;
            }

            if (TradingClient != null)
            {
                TradingClient.Dispose();
                TradingClient = null;
            }
        }
    }
}
