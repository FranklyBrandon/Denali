using Alpaca.Markets;

namespace Denali.Models.API
{
    public record StockDataResponse(
        Dictionary<string, List<IBar>> StockData, 
        Dictionary<string, IList<EMA>> FastEmas, 
        Dictionary<string, IList<EMA>> SlowEmas,
        List<DenaliClimbEntrySignal> EntrySignals);
}
