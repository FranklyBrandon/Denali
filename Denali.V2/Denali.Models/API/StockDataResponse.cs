using Alpaca.Markets;

namespace Denali.Models.API
{
    public record StockDataResponse(Dictionary<string, List<IBar>> StockData, IEnumerable<EMA> FastEmas, IEnumerable<EMA> SlowEmas);
}
