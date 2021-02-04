using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Algorithms.Test.BarAnalysisTests
{
    public class ParabolicSARTests : ParabolicSARTestFixture
    {
        [Test]
        public void ShouldReturnWhenNotEnoughDataGiven()
        {
            var barData = AggregateStockData1.Bars.GetRange(0, 1);
            StepThroughAnalyze(barData);

            Assert.IsTrue(Algo.SARSegments.Count == 0);
        }

        [Test]
        public void ShouldCalculateInitialSegmentAndTrendFromFirstTwoBars()
        {
            var barData = AggregateStockData1.Bars.GetRange(0, 2);
            StepThroughAnalyze(barData);

            Assert.IsTrue(Algo.SARSegments.Count == 1);

            var firstSAR = Algo.SARSegments.First()
                            .SARs.First();

            Assert.AreEqual(firstSAR.Value, barData[1].HighPrice);
        }

        [Test]
        public void ShouldAddToCurrentSegment()
        {
            var barData = AggregateStockData1.Bars.GetRange(0, 25);
            StepThroughAnalyze(barData);
        }
    }
}
