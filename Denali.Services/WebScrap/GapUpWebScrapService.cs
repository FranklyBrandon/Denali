using Denali.Models;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Denali.Services.WebScrap
{
    public class GapUpWebScrapService
    {
        private const string GAP_UP_PAGE_URL = "https://www.barchart.com/stocks/performance/gap/gap-up";
        private const string CHROME_PATH = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe";
        private const string BROWSER_USER_AGENT = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.75 Safari/537.36";
        private readonly string _pageAnchorSelector;

        public GapUpWebScrapService()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "WebScrap", "GapUpStockSelector.js");
            _pageAnchorSelector = File.ReadAllText(path);
        }

        public async Task<IEnumerable<GapUpStock>> ScrapGapUpSymbols()
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

            var stringifiedStocks = await page.EvaluateExpressionAsync<string>(_pageAnchorSelector);
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            IEnumerable<GapUpStock> stocks = JsonSerializer.Deserialize<IEnumerable<GapUpStock>>(stringifiedStocks, serializeOptions);

            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                , @$"GapUpStocks_{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Year}.txt");

            File.WriteAllText(path, stringifiedStocks);

            await browser.DisposeAsync();

            return stocks;
        }
        public async Task<IEnumerable<GapUpStock>> LoadGapUpStocksFromFile(string path)
        {
            var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                , @$"{path}");

            var file = await File.ReadAllTextAsync(filePath);
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<IEnumerable<GapUpStock>>(file, serializeOptions);
        }
    }
}
