using Alpaca.Markets;
using AutoMapper;
using Denali.Services;

namespace Denali.Processors
{
    public abstract class StrategyProcessorBase
    {
        protected readonly AlpacaService _alpacaService;
        protected readonly IMapper _mapper;

        public StrategyProcessorBase(AlpacaService alpacaService, IMapper mapper)
        {
            _alpacaService = alpacaService;
            _mapper = mapper;
        }

        protected async Task<IEnumerable<IIntervalCalendar>> GetPastMarketDays(int pastDays, DateTime day) =>
            await GetOpenMarketDays(day.AddDays(-pastDays), day);

        protected async Task<IEnumerable<IIntervalCalendar>> GetOpenMarketDays(DateTime from, DateTime into)
        {
            var calenders = await _alpacaService.AlpacaTradingClient.ListIntervalCalendarAsync(
                new CalendarRequest().WithInterval(
                    new Interval<DateTime>(from, into)
                )
            );
            return calenders.OrderBy(x => x.GetTradingDate());
        }

        protected async Task<List<ITrade>> GetTickData(string symbol, IIntervalCalendar marketDay) =>
            await GetTickData(symbol, marketDay.GetTradingOpenTimeUtc(), marketDay.GetTradingCloseTimeUtc());

        protected async Task<List<ITrade>> GetTickData(string symbol, DateTime startTime, DateTime endTime)
        {
            string? pageToken = default;
            List<ITrade> trades = new List<ITrade>();

            do
            {
                var request = new HistoricalTradesRequest(
                        symbol,
                        startTime,
                        endTime
                    ).WithPageSize(10000);

                if (!string.IsNullOrWhiteSpace(pageToken))
                    request.WithPageToken(pageToken);

                var response = await _alpacaService.AlpacaDataClient.GetHistoricalTradesAsync(request).ConfigureAwait(false);
                pageToken = response.NextPageToken;
                trades.AddRange(response.Items[symbol]);

            } while (!string.IsNullOrWhiteSpace(pageToken));

            return trades;
        }

        protected async Task<List<IBar>> GetAggregateData(string symbol, DateTime startTime, DateTime endTime, BarTimeFrame timeFrame)
        {
            string? pageToken = default;
            List<IBar> bars = new List<IBar>();

            do
            {
                var request = new HistoricalBarsRequest(
                        symbol,
                        startTime,
                        endTime,
                        timeFrame
                ).WithPageSize(10000);

                if (!string.IsNullOrWhiteSpace(pageToken))
                    request.WithPageToken(pageToken);

                var response = await _alpacaService.AlpacaDataClient.GetHistoricalBarsAsync(request).ConfigureAwait(false);
                pageToken = response.NextPageToken;
                bars.AddRange(response.Items[symbol]);

            } while (!string.IsNullOrWhiteSpace(pageToken));

            return bars;
        }

        protected async Task<Dictionary<string, List<IBar>>> GetAggregateDataMulti(IEnumerable<string> symbols, DateTime startTime, DateTime endTime, BarTimeFrame timeFrame)
        {
            string? pageToken = default;
            Dictionary<string, List<IBar>> bars = new Dictionary<string, List<IBar>>();

            do
            {
                var request = new HistoricalBarsRequest(
                        symbols,
                        startTime,
                        endTime,
                        timeFrame
                ).WithPageSize(10000);

                if (!string.IsNullOrWhiteSpace(pageToken))
                    request.WithPageToken(pageToken);

                var response = await _alpacaService.AlpacaDataClient.GetHistoricalBarsAsync(request).ConfigureAwait(false);
                pageToken = response.NextPageToken;
                foreach (var symbolData in response.Items)
                {
                    if (bars.ContainsKey(symbolData.Key))
                    {
                        var newData = bars[symbolData.Key];
                        newData.AddRange(symbolData.Value);
                        bars[symbolData.Key] = newData;              
                    }
                    else
                        bars[symbolData.Key] = symbolData.Value.ToList();
                }

            } while (!string.IsNullOrWhiteSpace(pageToken));

            return bars;
        }


    }
}
