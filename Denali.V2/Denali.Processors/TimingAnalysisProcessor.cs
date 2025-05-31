using AutoMapper;
using Denali.Models;
using Denali.Services;
using Denali.Shared.Extensions;

namespace Denali.Processors
{
    public class TimingAnalysisProcessor : StrategyProcessorBase
    {
        private readonly DataSanitizerService _sanitizerService;
        private readonly FileService _fileService;
        public TimingAnalysisProcessor(AlpacaService alpacaService, IMapper mapper) : base(alpacaService, mapper)
        {
            _sanitizerService = new DataSanitizerService();
            _fileService = new FileService();
        }

        public async Task Process(string symbol, DateTime startDate, CancellationToken stoppingToken)
        {
            _alpacaService.InitializeTradingclient();
            _alpacaService.InitializeDataClient();

            var marketDays = await GetPastMarketDays(startDate);

            var quotesData = await GetHistoricQuotes(symbol, marketDays.First());
            var quotes = _mapper.Map<List<Quote>>(quotesData);
            quotes = _sanitizerService.GetUniqueQuotesBySeconds(quotes);

            var firstPrice = quotes.First().MidPoint;
            var percentage = 0.05m / 100m;
            var target = (firstPrice * percentage).RoundToMoney();

            var marks = InitializeMarks();

            for ( var i = 1; i < quotes.Count - 1; i++ )
            {
                var currentQuote = quotes[i];
                var difference = currentQuote.MidPoint - firstPrice;
                var sign = difference > 0 ? 1 : -1;

                if (TargetReached(i, quotes, target * sign))
                {
                    marks.TryGetValue(currentQuote.TimeStampUTC.TimeOfDay, out int count);
                    marks[currentQuote.TimeStampUTC.TimeOfDay] = ++count;
                }
            }

            var date = DateTime.Now.Date;
            var outData = marks.Select(x => 
                new { time = ((DateTimeOffset)(date + x.Key)).ToUnixTimeSeconds(), value = x.Value }
            ).ToArray();

            await _fileService.WriteJSONResourceToFile("bar_data.json", outData);
        }

        private bool TargetReached(int index, List<Quote> quotes, decimal target)
        {
            var enterQuote = quotes[index];
            for (int i = index + 1; i < quotes.Count - index - 1; i++)
            {
                var current = quotes[i];
                if (target > 0)
                {
                    if (current.MidPoint - enterQuote.MidPoint >= target)
                        return true;
                }
                else
                {
                    if (current.MidPoint - enterQuote.MidPoint <= target)
                        return true;
                }
            }

            return false;
        }

        private Dictionary<TimeSpan, int> InitializeMarks()
        {
            var timeSpan = TimeSpan.FromHours(13.5);
            var map = new Dictionary<TimeSpan, int>();

            while (timeSpan.Hours != 20)
            {
                map[timeSpan] = 0;
                timeSpan = timeSpan.Add(TimeSpan.FromSeconds(1));
            }

            return map;
        }
    }
}
