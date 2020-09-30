using Denali.Models.Data.Alpaca;
using Denali.Models.Data.Trading;
using System.Collections.Generic;

namespace Denali.Processors.SignalAnalysis.Signals
{
    public interface ISignalAlgo
    {
        Signal Process(List<Candle> candles, int index, bool first, bool last);
    }
}