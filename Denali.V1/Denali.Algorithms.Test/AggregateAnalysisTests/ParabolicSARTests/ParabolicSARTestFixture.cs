using Denali.Algorithms.AggregateAnalysis.ParabolicSAR;
using Denali.Models.Shared;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Algorithms.Test.AggregateAnalysisTests
{
    [TestFixture]
    public class ParabolicSARTestFixture
    {
        protected ParabolicSAR Algo = new ParabolicSAR();

        public void StepThroughAnalyze(IList<IAggregateData> barData)
        {
            var currentBarData = new List<IAggregateData>();

            for (int i = 0; i <= barData.Count - 1; i++)
            {
                currentBarData.Add(barData[i]);
                Algo.Analyze(currentBarData);
            }
        }


       
    }
}
