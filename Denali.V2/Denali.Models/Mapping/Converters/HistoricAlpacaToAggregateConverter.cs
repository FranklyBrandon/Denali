using AutoMapper;
using Denali.Models.Alpaca;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Models.Mapping.Converters
{
    public class HistoricAlpacaToAggregateConverter : ITypeConverter<HistoricalBarsResponse, List<AggregateBar>>
    {
        public List<AggregateBar> Convert(HistoricalBarsResponse source, List<AggregateBar> destination, ResolutionContext context)
        {
            var data = context.Mapper.Map<List<AggregateBar>>(source.Bars);
            foreach (var item in data)
            {
                item.SetSymbol(source.Symbol);
            }

            return data;
        }
    }
}
