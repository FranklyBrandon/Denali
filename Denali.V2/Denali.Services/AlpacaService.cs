using Alpaca.Markets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Services
{
    public class AlpacaService
    {
        private readonly IAlpacaStreamingClient _alpacaStreamingclient;
        private readonly IAlpacaDataStreamingClient _alpacaDataStreamingClient;
        private readonly IAlpacaDataClient _alpacaDataClient;
        private readonly IAlpacaTradingClient _alpacaTradingClient;
        private readonly SecretKey _secretKey;
        private readonly IHostEnvironment _hostEnvironment;
        public AlpacaService(IHostEnvironment hostEnvironment, IConfiguration configuration)
        {
            _hostEnvironment = hostEnvironment;
            _secretKey = new SecretKey(configuration["Alpaca:API-Key"], configuration["Alpaca:API-Secret"]);
        }

        private IAlpacaStreamingClient BuildStreamingclient() => _hostEnvironment.IsProduction()
            ? Alpaca.Markets.Environments.Live.GetAlpacaStreamingClient(_secretKey) 
            : Alpaca.Markets.Environments.Paper.GetAlpacaStreamingClient(_secretKey);

        private IAlpacaDataStreamingClient BuildDataStreamingClient() => _hostEnvironment.IsProduction()
            ? Alpaca.Markets.Environments.Live.GetAlpacaDataStreamingClient(_secretKey)
            : Alpaca.Markets.Environments.Paper.GetAlpacaDataStreamingClient(_secretKey);

        private IAlpacaDataClient BuildDataclient() => _hostEnvironment.IsProduction()
            ? Alpaca.Markets.Environments.Live.GetAlpacaDataClient(_secretKey)
            : Alpaca.Markets.Environments.Paper.GetAlpacaDataClient(_secretKey);

        private IAlpacaTradingClient BuildTradingClient() => _hostEnvironment.IsProduction()
            ? Alpaca.Markets.Environments.Live.GetAlpacaTradingClient(_secretKey)
            : Alpaca.Markets.Environments.Paper.GetAlpacaTradingClient(_secretKey);
    }
}
