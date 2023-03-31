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
    }
}
