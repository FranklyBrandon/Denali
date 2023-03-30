using Denali.Models;
using Denali.Shared.Time;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Services.YahooFinanceService
{
    public interface IYahooFinanceService
    {
        Task<IAggregateBar> GetLatestQuote(string symbol);
        Task<List<IAggregateBar>> GetQuotes(string symbol, string interval, string range);
    }

    public class YahooFinanceService : IYahooFinanceService
    {
        private readonly IYahooFinanceClient _yahooFinanceClient;

        public YahooFinanceService(IYahooFinanceClient yahooFinanceClient)
        {
            _yahooFinanceClient = yahooFinanceClient;
        }

        public async Task<IAggregateBar> GetLatestQuote(string symbol)
        {
            var response = await _yahooFinanceClient.GetLatestQuote(symbol);

            var quote = response.Chart.Results.First().Indicators.Quotes.Last();
            var time = response.Chart.Results.First().TimeStamps.Last();

            // TODO: convert to AutoMapper
            var open = quote.Open.Last();
            var high = quote.High.Last();
            var low = quote.Low.Last();
            var close = quote.Close.Last();
            var volume = quote.Volume.Last();
            var timeUTC = TimeUtils.UnixTimeStampToDateTime(time);

            var aggregate = new AggregateBar
            {
                Open = open.HasValue ? open.Value : 0m,
                High = high.HasValue ? high.Value : 0m,
                Low = low.HasValue ? low.Value : 0m,
                Close = close.HasValue ? close.Value : 0m,
                Volume = volume.HasValue ? volume.Value : 0m,
                TimeUtc = timeUTC
            };
            aggregate.SetSymbol(symbol);

            return aggregate;
        }

        public async Task<List<IAggregateBar>> GetQuotes(string symbol, string interval, string range)
        {
            var response = await _yahooFinanceClient.GetQuotes(symbol, interval, range);
            var result = response.Chart.Results.First().Indicators.Quotes.First();

            // TODO: convert to AutoMapper
            var opens = result.Open;
            var highs = result.High;
            var lows = result.Low;
            var closes = result.Close;
            var volumes = result.Volume;

            var timeStamps = response.Chart.Results.First().TimeStamps;
            List<IAggregateBar> aggregates = new List<IAggregateBar>();
            
            var length = timeStamps.Count() - 1;
            for (int i = 0; i < length; i++)
            {
                var aggregate = new AggregateBar
                {
                    Open = opens[i].HasValue ? opens[i].Value : 0,
                    High = highs[i].HasValue ? highs[i].Value : 0,
                    Low = lows[i].HasValue ? lows[i].Value : 0,
                    Close = closes[i].HasValue ? closes[i].Value : 0,
                    Volume = volumes[i].HasValue ? volumes[i].Value : 0,
                    TimeUtc = TimeUtils.UnixTimeStampToDateTime(timeStamps[i])
            };
                aggregate.SetSymbol(symbol);

                aggregates.Add(aggregate);
            }

            return aggregates;
        }
    }
}
