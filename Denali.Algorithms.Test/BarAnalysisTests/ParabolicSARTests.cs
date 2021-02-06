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
            StepThroughAnalyze(WilderSARDataExample);
            var la = JsonSerializer.Serialize(Algo.SARSegments);
        }
    }
}
