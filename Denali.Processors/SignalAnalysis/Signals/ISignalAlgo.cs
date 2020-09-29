using Denali.Models.Data.Alpaca;
using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Processors.SignalAnalysis.Signals
{
    public interface ISignalAlgo
    {
        void Process(List<Candle> candles, int index, bool first, bool last);
    }
}
