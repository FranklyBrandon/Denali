using Denali.Algorithms.BarAnalysis.CandlestickPattern;
using Denali.Algorithms.BarAnalysis.ParabolicSAR;
using Denali.Models.Polygon;
using Denali.Models.Shared;
using Denali.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Denali.Algorithms.BarAnalysis
{
    public class BarAlgorithmAnalysis
    {
        private readonly BulishingEngulfingAlgo _bullishEngulfingAlgo;
        private readonly ParabolicSARAlgo _sarAlgo;
        private readonly TimeUtils _timeUtils;

        public BarAlgorithmAnalysis()
        {
            this._bullishEngulfingAlgo = new BulishingEngulfingAlgo();
            this._sarAlgo = new ParabolicSARAlgo();
            this._timeUtils = new TimeUtils();
        }

        public void Analyze(IList<Bar> barData)
        {
            long currentTime = barData.Last().Time;

            _sarAlgo.Analyze(barData);

            var time = _timeUtils.GetETDatetimefromUnixMS(currentTime);

            if (_sarAlgo.IsTrendBeginning(Trend.UpTrend) && _bullishEngulfingAlgo.IsEngulfing(barData, MarketSide.Bullish))
            {
                Console.WriteLine($"Bullish engulfing uptrend starting at {time.ToString()}");
            }
        }
    }
}
