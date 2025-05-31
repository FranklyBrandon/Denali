using Alpaca.Markets;
using Denali.Models;
using Denali.Shared.Extensions;
using Denali.Shared.Time;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Services
{
    public class DataSanitizerService
    {
        public IEnumerable<ITrade> SanitizeTrades(IEnumerable<ITrade> trades)
        {
            const decimal MAX_DIFF = 1.50m;
            var result = new List<ITrade>();
            for (int i = 0; i < trades.Count(); i++)
            {
                ITrade previous = trades.ElementAtOrDefault(i -1);
                ITrade current = trades.ElementAt(i);
                ITrade next = trades.ElementAtOrDefault(i +1);

                if (previous != null && next != null)
                {
                    if (Math.Abs(current.Price - previous.Price) >= MAX_DIFF && Math.Abs(current.Price - next.Price) >= MAX_DIFF)
                    {
                        Console.WriteLine($"Outlier Trade at {TimeUtils.GetNewYorkTime(current.TimestampUtc).TimeOfDay}");
                        Console.WriteLine($"Previous Price: {previous.Price}");
                        Console.WriteLine($"Price: {current.Price}");
                        Console.WriteLine($"Next Price: {next.Price}");
                    }
                    else
                        result.Add(current);
                }
                else
                    result.Add(current);
            }

            return result;
        }

        public List<Quote> GetUniqueQuotesBySeconds(List<Quote> quotes)
        {
            List<Quote> sanitizedQuotes = new List<Quote>();
            quotes[0].RoundToSeconds();
            sanitizedQuotes.Add(quotes[0]);

            for (int i = 1; i < quotes.Count - 1; i++)
            {
                var previous = quotes[i - 1];
                var current = quotes[i];
                current.RoundToSeconds();

                if (previous.TimeStampUTC == current.TimeStampUTC)
                {
                    continue;
                }

                sanitizedQuotes.Add(current);
            }

            return sanitizedQuotes;
        }
    }
}
