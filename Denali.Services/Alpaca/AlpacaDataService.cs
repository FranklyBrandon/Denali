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
    public class AlpacaDataService
    {
        /*
        public AlpacaDataClient DataClient { get; private set; }
        public AlpacaDataClient alpacaDataClient;
        public IAlpacaDataStreamingClient DataStreamingClient { get; private set; }

        private readonly AlpacaSettings _settings;
        private readonly IMapper _mapper;
        public AlpacaDataService(AlpacaSettings settings, IMapper mapper)
        {
            this._settings = settings;
            this._mapper = mapper;
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
                DataStreamingClient.Dispose();
                DataStreamingClient = null;
            }

            var config = new AlpacaDataStreamingClientConfiguration
            {
                ApiEndpoint = new Uri(_settings.DataStreamingUrl),
                SecurityId = new SecretKey(_settings.APIKey, _settings.APISecret)
            };

            DataStreamingClient = new AlpacaDataStreamingClient(config);
        }

        public async Task<Dictionary<string, List<IAggregateData>>> GetHistoricBarData(DateTime from, DateTime to, TimeFrame timeframe, IEnumerable<string> symbols, int limit = 1000)
        {
            var response = await DataClient.GetBarSetAsync(
                new BarSetRequest(symbols, timeframe) { Limit = limit }.SetInclusiveTimeInterval(from, to));

            return response.ToDictionary(x => x.Key, y => _mapper.Map<IEnumerable<AggregateData>>(y.Value).ToList<IAggregateData>());
        }

        public async Task Disconnect()
        {
            if (DataStreamingClient != null)
            {
                await DataStreamingClient.DisconnectAsync();
                DataStreamingClient.Dispose();
                DataStreamingClient = null;
            }

            if (DataClient != null)
            {
                DataClient.Dispose();
                DataClient = null;
            }
        }
        */
    }
}
