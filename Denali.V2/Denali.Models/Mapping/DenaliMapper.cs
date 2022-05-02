using Alpaca.Markets;
using AutoMapper;
using Denali.Models.Alpaca;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
    }
}
