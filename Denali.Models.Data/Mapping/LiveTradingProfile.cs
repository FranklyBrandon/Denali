using Alpaca.Markets;
using AutoMapper;
using Denali.Models.Polygon;
using Denali.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Models.Mapping
{
    public class LiveTradingProfile : Profile
    {
        public LiveTradingProfile()
        {
            CreateMap<IStreamAgg, Bar>()
                .ForMember(dest => dest.OpenPrice, opt => opt.MapFrom(src => src.Open))
                .ForMember(dest => dest.ClosePrice, opt => opt.MapFrom(src => src.Close))
                .ForMember(dest => dest.HighPrice, opt => opt.MapFrom(src => src.High))
                .ForMember(dest => dest.LowPrice, opt => opt.MapFrom(src => src.Low))
                .ForMember(dest => dest.Volume, opt => opt.MapFrom(src => src.Volume))
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => ((DateTimeOffset)src.EndTimeUtc).ToUnixTimeSeconds()));

            CreateMap<IAgg, Bar>()
                .ForMember(dest => dest.OpenPrice, opt => opt.MapFrom(src => src.Open))
                .ForMember(dest => dest.ClosePrice, opt => opt.MapFrom(src => src.Close))
                .ForMember(dest => dest.HighPrice, opt => opt.MapFrom(src => src.High))
                .ForMember(dest => dest.LowPrice, opt => opt.MapFrom(src => src.Low))
                .ForMember(dest => dest.Volume, opt => opt.MapFrom(src => src.Volume))
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => ((DateTimeOffset)src.TimeUtc).ToUnixTimeSeconds()));
        }
    }
}
