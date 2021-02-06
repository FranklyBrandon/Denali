using Denali.Algorithms.BarAnalysis.ParabolicSAR;
using Denali.Models.Polygon;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Algorithms.Test.BarAnalysisTests
{
    [TestFixture]
    public class ParabolicSARTestFixture
    {
        protected AggregateResponse AggregateStockData1 => _aggregateStockData1 ??=
            AggregateDataHelper.GetStockDataFile(AggregateDataHelper.AAPL_STOCK_DATA);
        private AggregateResponse _aggregateStockData1;

        protected ParabolicSARAlgo Algo = new ParabolicSARAlgo();

        public void StepThroughAnalyze(IList<Bar> barData)
        {
            var currentBarData = new List<Bar>();

            for (int i = 0; i <= barData.Count - 1; i++)
            {
                currentBarData.Add(barData[i]);
                Algo.Analyze(currentBarData);
            }
        }


       
    }
}
