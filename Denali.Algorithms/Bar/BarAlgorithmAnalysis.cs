using Denali.Models.Polygon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Algorithms.BarAnalysis
{
    public class BarAlgorithmAnalysis
    {
        private readonly IEnumerable<IBarSignalAlgorithm> _algorithms;
        private readonly BulishingEngulfingAlgo _bullishEngulfingAlgo; 

        public BarAlgorithmAnalysis(IEnumerable<IBarSignalAlgorithm> algorithms)
        {
            this._algorithms = algorithms;
            this._bullishEngulfingAlgo = new BulishingEngulfingAlgo();
        }

        public void Analyze(IList<Bar> barData)
        {
            _bullishEngulfingAlgo.Analyze(barData);
        }
        //public List<StockAction> Analyze(Dictionary<string, List<Candle>> barData)
        //{
        //    var actions = new List<StockAction>();
        //    foreach (var symbolData in barData)
        //    {
        //        foreach (var algorithm in _algorithms)
        //        {
        //            var action = algorithm.Analyze(symbolData);
        //            if (action != null)
        //                actions.Add(action);
        //        }
        //    }


        //    return actions;
        //}




    }
}
