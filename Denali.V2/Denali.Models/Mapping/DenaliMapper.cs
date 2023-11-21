using Alpaca.Markets;
using AutoMapper;
using Denali.Models.Alpaca;

namespace Denali.Models.Mapping
{
    public class DenaliMapper : Profile
    {
        public DenaliMapper()
        {
            CreateMap<IBar, AggregateBar>();

            CreateMap<HistoricalBarsResponse, List<AggregateBar>>()
                .AfterMap((src, dest) =>
                {
                    dest.ForEach(x => x.SetSymbol(src.Symbol));
                })
                .ConvertUsing((x, y, c) => c.Mapper.Map<List<AggregateBar>>(x));

            CreateMap<AggregateTimeFrame, BarTimeFrame>().ConvertUsing((value, destination) =>
            {
                switch (value)
                {
                    case AggregateTimeFrame.Minute:
                        return BarTimeFrame.Minute;
                    case AggregateTimeFrame.Minute15:
                        return new BarTimeFrame(15, BarTimeFrameUnit.Minute);
                    default:
                        return BarTimeFrame.Minute;
                }
            });
        }
    }
}
