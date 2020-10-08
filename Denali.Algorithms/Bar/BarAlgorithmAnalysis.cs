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

        public List<StockAction> Analyze(Dictionary<string, List<Candle>> barData)
        {
            var actions = new List<StockAction>();
            foreach (var symbolData in barData)
            {
                foreach (var algorithm in _algorithms)
                {
                    var action = algorithm.Analyze(symbolData);
                    if (action != null)
                        actions.Add(action);
                }
            }


            return actions;
        }
    }
}
