using Alpaca.Markets;
using AutoMapper;
using Denali.Models.Alpaca;
using Denali.Shared.Time;
using InteractiveBrokers.Models.Response;

namespace Denali.Models.Mapping
{
    public class DenaliMapper : Profile
    {
        public DenaliMapper()
        {
            CreateMap<IBar, AggregateBar>();
            CreateMap<IQuote, Quote>();

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

            // IB aggregate model to generic Denali aggregate model
            CreateMap<Aggregate, AggregateBar>()
                .ForMember(dest => dest.Open, act => act.MapFrom(src => src.o))
                .ForMember(dest => dest.Close, act => act.MapFrom(src => src.c))
                .ForMember(dest => dest.High, act => act.MapFrom(src => src.h))
                .ForMember(dest => dest.Low, act => act.MapFrom(src => src.l))
                .ForMember(dest => dest.Volume, act => act.MapFrom(src => src.v))
                .ForMember(dest => dest.TimeUtc, act => act.MapFrom(src => TimeUtils.UnixTimeStampMilliToDateTime(src.t)));

            // IB aggregate response model to list of generic Denali aggregate models
            CreateMap<HistoricAggregateResponse, List<AggregateBar>>()
                .ConvertUsing((src, dest, context) =>
                    src.data
                        .Select(bar => context.Mapper.Map<AggregateBar>(bar)
                    ).ToList()
                );
        }
    }
}
