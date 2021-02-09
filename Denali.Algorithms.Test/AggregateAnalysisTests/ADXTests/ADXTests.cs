using Denali.Algorithms.AggregateAnalysis.ADX;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Algorithms.Test.AggregateAnalysisTests.ADXTests
{
    public class ADXTests : ADXTestsFixture
    {
        [Test]
        public void JWellesWilderInitializeComparisonTest()
        {
            var bars = AggregateDataHelper.JWellesWilderADXExampleSheet();
            ADXAlgo.Initiate(bars);

            var wilderResults = AggregateDataHelper.JWellesWilderADXResults();

            foreach (var result in ADXAlgo.InitialADXResults)
            {
                ADXResult validResult;
                wilderResults.TryGetValue(result.Time, out validResult);
                if (validResult == null)
                {
                    Assert.Fail($"Time of calculated result not in valid result set. Time: {result.Time}");
                }
                else
                {
                    Assert.AreEqual(validResult.TrueRange, result.TrueRange, $"Time: {result.Time}");
                    Assert.AreEqual(validResult.ADX, result.ADX, $"Time: {result.Time}");
                    Assert.AreEqual(validResult.DMPlus, result.DMPlus, $"Time: {result.Time}");
                    Assert.AreEqual(validResult.DMMinus, result.DMMinus, $"Time: {result.Time}");
                }
            }

            var validTrueValueSum = wilderResults.Sum(x => x.Value.TrueRange);
            var TrueValueSum = ADXAlgo.InitialADXResults.Sum(x => x.TrueRange);
            Assert.AreEqual(validTrueValueSum, TrueValueSum, "True Range sum are not equal");

            var validDMPlus = wilderResults.Sum(x => x.Value.DMPlus);
            var DIPlus = ADXAlgo.InitialADXResults.Sum(x => x.DMPlus);
            Assert.AreEqual(validDMPlus, DIPlus, "DI Plus sum are not equal");

            var validDMMinus = wilderResults.Sum(x => x.Value.DMMinus);
            var DIMinus = ADXAlgo.InitialADXResults.Sum(x => x.DMMinus);
            Assert.AreEqual(validDMMinus, DIMinus, "DI Minus sum are not equal");
        }
    }
}
