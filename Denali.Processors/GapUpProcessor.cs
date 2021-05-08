using Denali.Models.Shared;
using Denali.Services.Alpaca;
using Denali.Shared.Utility;
using Microsoft.Extensions.Configuration;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public class GapUpProcessor : IProcessor
    {
        private readonly AlpacaService _alpacaService;
        private readonly TimeUtils _timeUtils;
        private readonly IConfiguration _configuration;

        private const string GAP_UP_PAGE_URL = "https://www.barchart.com/stocks/performance/gap/gap-up";
        private const string CHROME_PATH = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe";
        private const string BEGIN_STOCK_URL = "https://www.barchart.com/stocks/quotes/";
        private const string END_STOCK_URL = "/overview";
        private const string BROWSER_USER_AGENT = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.75 Safari/537.36";
        private const string PAGE_ANCHOR_SELECTOR = @"Array.from(document.querySelectorAll('a')).map(a => a.href);";

        public GapUpProcessor(AlpacaService alpacaService, IConfiguration configuration)
        {
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
            var symbols = _configuration["symbols"].Split(',');
            var la = ScrapSymbols();
            CalculateWindows(symbols);
        }

        public async Task ShutDown(CancellationToken stoppingToken)
        {
            await _alpacaService.Disconnect();
        }

        private async Task<IEnumerable<string>> ScrapSymbols()
        {
            var options = new LaunchOptions()
            {
                Headless = true,
                ExecutablePath = CHROME_PATH,
                Product = Product.Chrome           
            };

            var browser = await Puppeteer.LaunchAsync(options);
            var page = (await browser.PagesAsync()).First();
            await page.SetUserAgentAsync(BROWSER_USER_AGENT);
            await page.GoToAsync(GAP_UP_PAGE_URL);

            var urls = await page.EvaluateExpressionAsync<string[]>(PAGE_ANCHOR_SELECTOR);

            /*
             *  Foreach tr in tbody (line 312) document.getElementsByTagName("tbody")[0];
             *  Stock is equal to tr attribute: data-current-symbol
             *  Volume is inside td where class: volume
             * 
             */
            var stockUrls = urls
                .Where(x => x.StartsWith(BEGIN_STOCK_URL) && x.EndsWith(END_STOCK_URL))
                .Distinct();

            var html = await page.GetContentAsync();

            await browser.DisposeAsync();
            return stockUrls;
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
