using Denali.Algorithms.BarAnalysis.ParabolicSAR;
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

            StepThroughAnalyze(WilderSARDataExample);
            var la = JsonSerializer.Serialize(Algo.SARSegments);
        }
    }
}
