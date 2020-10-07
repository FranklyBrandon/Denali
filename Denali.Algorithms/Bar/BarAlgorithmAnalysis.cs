using Denali.Models.Data.Alpaca;
using Denali.Models.Data.Trading;
using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Algorithms.Bar
{
    public class BarAlgorithmAnalysis
    {
        private readonly IEnumerable<IBarSignalAlgorithm> _algorithms;

        public BarAlgorithmAnalysis(IEnumerable<IBarSignalAlgorithm> algorithms)
        {
            this._algorithms = algorithms;
        }

        public StockAction Analyze(KeyValuePair<string, List<Candle>> barData)
        {
            return default;
        }
    }
}
