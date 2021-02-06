using Denali.Algorithms.BarAnalysis.ParabolicSAR;
using Denali.Algorithms.Test.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Denali.Algorithms.Test.BarAnalysisTests
{
    public class ParabolicSARTests : ParabolicSARTestFixture
    {
        [Test]
        public void JWellesWilderComparisonTest()
        {
            var segment = new SARSegment(52.35, 4, Trend.UpTrend);
            var sar = new SAR(50.00, 4);
            segment.SARs.Add(sar);

            Algo.SetInitialSegment(segment);

            var barData = AggregateDataHelper.JWellesWilderSARExampleSheet();
            barData = barData.GetRange(1, barData.Count - 1);
            StepThroughAnalyze(barData);

            var wilderResults = AggregateDataHelper.JWellsWilderSARResults();
            foreach (var segmentResult in Algo.SARSegments)
            {
                foreach (var sarResult in segmentResult.SARs)
                {
                    ParabolicSARResult validResult;
                    wilderResults.TryGetValue(sarResult.Time, out validResult);
                    if (validResult == null)
                    {
                        Assert.Fail($"Time of calculated result not in valid result set. Time: {sarResult.Time}");
                    }
                    else
                    {
                        Assert.AreEqual(validResult.SAR, sarResult.Value, $"Time: {sarResult.Time}");
                    }
                }
            }
        }

        [Test]
        public void CalculateInitialSARValueTest()
        {
            var barData = AggregateDataHelper.JWellesWilderSARExampleSheet();
            StepThroughAnalyze(barData);
        }
    }
}
