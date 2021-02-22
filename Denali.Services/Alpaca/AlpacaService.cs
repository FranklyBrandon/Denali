using Alpaca.Markets;
using AutoMapper;
using Denali.Models.Shared;
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
        public AlpacaDataClient DataClient { get; private set; }
        public AlpacaDataStreamingClient DataStreamingClient { get; private set; }

        private readonly AlpacaSettings _settings;
        private readonly IMapper _mapper;
        public AlpacaService(AlpacaSettings settings, IMapper mapper)
        {
            this._settings = settings;
            this._mapper = mapper;
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

        public void InitializeDataClient()
        {
            if (DataClient != null)
            {
                DataClient.Dispose();
                DataClient = null;
            }

            var config = new AlpacaDataClientConfiguration
            {
                ApiEndpoint = new Uri(_settings.DataUrl),
                SecurityId = new SecretKey(_settings.APIKey, _settings.APISecret)
            };

            DataClient = new AlpacaDataClient(config);
        }

        public void InitializeDataStreamingclient()
        {
            if (DataStreamingClient != null)
            {
                DataClient.Dispose();
                DataClient = null;
            }

            var config = new AlpacaDataStreamingClientConfiguration
            {
                ApiEndpoint = new Uri(_settings.StreamingUrl),
                SecurityId = new SecretKey(_settings.APIKey, _settings.APISecret)
            };

            DataStreamingClient = new AlpacaDataStreamingClient(config);
        }

        public async Task<Dictionary<string, List<IAggregateData>>> GetHistoricBarData(DateTime from, DateTime to, TimeFrame timeframe, int limit = 1000, params string[] symbols)
        {
            var response = await DataClient.GetBarSetAsync(
                new BarSetRequest(symbols, timeframe) { Limit = limit }.SetInclusiveTimeInterval(from, to));

            return response.ToDictionary(x => x.Key, y => _mapper.Map<IEnumerable<AggregateData>>(y.Value).ToList<IAggregateData>());
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
