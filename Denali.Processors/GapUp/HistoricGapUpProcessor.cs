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
            //var la = await _gapUpWebScrapService.ScrapGapUpSymbols();

            var stocks = await _gapUpWebScrapService.LoadGapUpStocksFromFile("GapUpStocks_5_17_2021.txt");
            //stocks = stocks.Where(x => x.VolumeInt >= 500000);

            _alpacaService.InitializeDataClient();
            var fromDate = DateTime.Parse(_configuration["from"]);
            var toDate = DateTime.Parse(_configuration["to"]);

            var barData = await _alpacaService.GetHistoricBarData(
                _timeUtils.GetNYSEOpenDateTime(fromDate)
                , _timeUtils.GetNYSECloseDateTime(toDate)
                , Alpaca.Markets.TimeFrame.Minute
                , symbols: stocks.Select(x => x.Symbol));

            var tenTimestamp = _timeUtils.GetUnixSecondStamp(
                _timeUtils.GetEasternLocalTime(fromDate, 10, 0, 0));

            foreach (var stock in stocks)
            {
                var stockdata = barData[stock.Symbol];

                bool brokeOpening = false;
                decimal maxOpeningPrice = 0;
                decimal maxPrice = 0;
                decimal buyPrice = 0;
                for (int i = 0; i < stockdata.Count - 1; i++)
                {
                    var data = stockdata[i];

                    if (data.Time < tenTimestamp)
                        maxOpeningPrice = (data.HighPrice > maxOpeningPrice) ? data.HighPrice : maxOpeningPrice;
                    else
                    {
                        maxPrice = (data.HighPrice > maxPrice) ? data.HighPrice : maxPrice;

                        if ((data.ClosePrice > maxOpeningPrice) && !brokeOpening)
                        {
                            buyPrice = stockdata[i + 1].OpenPrice;
                            brokeOpening = true;
                        }
                    }
                }

                if (maxOpeningPrice > maxPrice)
                    maxPrice = maxOpeningPrice;
                if (brokeOpening)
                {
                    Console.WriteLine("========");
                    Console.WriteLine($"{stock.Symbol}, Window: {maxPrice - buyPrice}");
                    Console.WriteLine($"Two percent: {buyPrice * .02m}");
                }
            }
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
