using Denali.Algorithms.AggregateAnalysis.CandlestickPattern;
using Denali.Models.Shared;
using Denali.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Denali.Algorithms.AggregateAnalysis
{
    public class BarAlgorithmAnalysis
    {
        private readonly Engulfing _bullishEngulfingAlgo;
        private readonly ParabolicSAR.ParabolicSAR _sarAlgo;
        private readonly TimeUtils _timeUtils;

        public BarAlgorithmAnalysis()
        {
            this._bullishEngulfingAlgo = new Engulfing();
            this._sarAlgo = new ParabolicSAR.ParabolicSAR();
            this._timeUtils = new TimeUtils();
        }

        public void Analyze(IEnumerable<IAggregateData> barData)
        {
            long currentTime = barData.Last().Time;

            _sarAlgo.Analyze(barData);

            var time = _timeUtils.GetETDatetimefromUnixMS(currentTime);

            if (_sarAlgo.IsTrendBeginning(MarketSide.Bullish) && _bullishEngulfingAlgo.IsEngulfing(barData, MarketSide.Bullish))
            {
                Console.WriteLine($"Bullish engulfing uptrend starting at {time.ToString()}");
            }
        }
    }
}
