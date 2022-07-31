using Alpaca.Markets;
using AutoMapper;
using Denali.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        protected async Task<IEnumerable<IIntervalCalendar>> GetOpenBacklogDays(int pastDays, DateTime day)
        {
            var calenders = await _alpacaService.AlpacaTradingClient.ListIntervalCalendarAsync(new CalendarRequest().WithInterval(new Interval<DateTime>(day.AddDays(-pastDays), day)));
            return calenders.OrderByDescending(x => x.GetTradingDate());
        }

        protected async Task<IEnumerable<IIntervalCalendar>> GetOpenMarketDays(DateTime from, DateTime into)
        {
            var calenders = await _alpacaService.AlpacaTradingClient.ListIntervalCalendarAsync(new CalendarRequest().WithInterval(new Interval<DateTime>(from, into)));
            return calenders.OrderByDescending(x => x.GetTradingDate());
        }
    }
}
