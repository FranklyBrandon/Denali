using Denali.Algorithms.ActionAnalysis;
using Denali.Models.Shared;
using Denali.Services.Alpaca;
using Denali.Services.WebScrap;
using Denali.Shared.Utility;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public class HistoricGapUpProcessor
    {
        private readonly GapUpWebScrapService _gapUpWebScrapService;
        private readonly AlpacaDataService _alpacaService;
        private readonly TimeUtils _timeUtils;
        private readonly IConfiguration _configuration;

        public HistoricGapUpProcessor(GapUpWebScrapService gapUpWebScrapService, AlpacaDataService alpacaService, IConfiguration configuration)
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
            //var stocks = await _gapUpWebScrapService.ScrapGapUpSymbols();
            var stocks = await _gapUpWebScrapService.LoadGapUpStocksFromFile("GapUpStocks_6_4_2021.txt");
            stocks = stocks.OrderByDescending(x => x.VolumeInt);
            /*
            _alpacaService.InitializeDataClient();
            var fromDate = DateTime.Parse(_configuration["from"]);
            var toDate = DateTime.Parse(_configuration["to"]);

            var barData = await _alpacaService.GetHistoricBarData(
                _timeUtils.GetNYSEOpenDateTime(fromDate)
                , _timeUtils.GetNYSECloseDateTime(toDate)
                , Alpaca.Markets.TimeFrame.Minute
                , symbols: stocks.Select(x => x.Symbol));
            */
            //foreach (var stock in stocks)
            //{
            //    var stockdata = barData[stock.Symbol];
            //    var strategy = new GapUpCoolOff(fromDate, 10, 0, stock.Symbol);

            //    for (int i = 0; i < stockdata.Count - 1; i++)
            //    {
            //        var data = stockdata[i];
            //        strategy.OnBarReceived(data);
            //    }
            //}
        }

        public async Task ShutDown(CancellationToken stoppingToken)
        {
            //await _alpacaService.Disconnect();
        }
    }
}
