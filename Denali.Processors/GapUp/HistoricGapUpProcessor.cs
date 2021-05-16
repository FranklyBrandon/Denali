using Denali.Models;
using Denali.Models.Shared;
using Denali.Services.Alpaca;
using Denali.Services.WebScrap;
using Denali.Shared.Utility;
using Microsoft.Extensions.Configuration;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public class HistoricGapUpProcessor : IProcessor
    {
        private readonly GapUpWebScrapService _gapUpWebScrapService;
        private readonly AlpacaService _alpacaService;
        private readonly TimeUtils _timeUtils;
        private readonly IConfiguration _configuration;

        public HistoricGapUpProcessor(GapUpWebScrapService gapUpWebScrapService, AlpacaService alpacaService, IConfiguration configuration)
        {
            _gapUpWebScrapService = gapUpWebScrapService;
            _alpacaService = alpacaService;
            _configuration = configuration;
            _timeUtils = new();
        }

        public void OnBarReceived(IAggregateData barData)
        {
            throw new NotImplementedException();
        }

        public async Task Process(DateTime startTime, CancellationToken stoppingToken)
        {
            var stocks = await _gapUpWebScrapService.LoadGapUpStocksFromFile("GapUpStocks_5_14_2021.txt");
            _alpacaService.InitializeDataClient();
            var fromDate = DateTime.Parse(_configuration["from"]);
            var toDate = DateTime.Parse(_configuration["to"]);
            var barData = await _alpacaService.GetHistoricBarData(
                _timeUtils.GetNYSEOpenDateTime(fromDate)
                , _timeUtils.GetNYSECloseDateTime(toDate)
                , Alpaca.Markets.TimeFrame.Day
                , symbols: stocks.Select(x => x.Symbol));

        }

        public async Task ShutDown(CancellationToken stoppingToken)
        {
            await _alpacaService.Disconnect();
        }

      
        private async void CalculateWindows(string[] symbols)
        {
            _alpacaService.InitializeDataClient();
            var fromDate = DateTime.Parse(_configuration["from"]);
            var toDate = DateTime.Parse(_configuration["to"]);

            var barData = await _alpacaService.GetHistoricBarData(_timeUtils.GetNYSEOpenDateTime(fromDate), _timeUtils.GetNYSECloseDateTime(toDate), Alpaca.Markets.TimeFrame.Day, symbols: symbols);
            foreach (var symbol in barData)
            {
                var bar = symbol.Value.First();
                var log = $"Ticker {symbol.Key} is ";

                if (bar.IsOpen)
                    log += "OPEN";
                else
                    log += "CLOSED";

                var change = Math.Round(((bar.HighPrice - bar.OpenPrice) / bar.OpenPrice) * 100, 2, MidpointRounding.AwayFromZero);
                log += $". Profit window is {change}%";

                Console.WriteLine(log);
            }
        }
    }
}
