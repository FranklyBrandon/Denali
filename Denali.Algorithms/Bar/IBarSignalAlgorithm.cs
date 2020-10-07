using Denali.Models.Data.Alpaca;
using Denali.Models.Data.Trading;
using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Algorithms.Bar
{
    public interface IBarSignalAlgorithm
    {
        StockAction Analyze(KeyValuePair<string, List<Candle>> barData);
    }
}
