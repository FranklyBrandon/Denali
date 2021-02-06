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

        protected List<Bar> WilderSARDataExample => _wilderSARDataExample ??=
            BuildJWellesWilderSARExampleSheet();
        private List<Bar> _wilderSARDataExample;

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

        /// <summary>
        /// The example bar data given on page 12 of "New Concepts in Technical Trading Systems" 
        /// Wilder uses this example data to demonstrate the parabolic stop and reversal calculation
        /// </summary>
        /// <returns></returns>
        private List<Bar> BuildJWellesWilderSARExampleSheet()
        {
            return new List<Bar>
            {
                //Date 4
                new Bar
                {
                    HighPrice = 52.35,
                    LowPrice = 51.50,
                    Time = 4
                },
                //Date 5
                new Bar
                {
                    HighPrice = 52.10,
                    LowPrice = 51.00,
                    Time = 5
                },
                //Date 6
                new Bar
                {
                    HighPrice = 51.80,
                    LowPrice = 50.50,
                    Time = 6
                },
                //Date 7
                new Bar
                {
                    HighPrice = 52.10,
                    LowPrice = 51.25,
                    Time = 7
                },
                //Date 8
                new Bar
                {
                    HighPrice = 52.50,
                    LowPrice = 51.70,
                    Time = 8
                },
                //Date 9
                new Bar
                {
                    HighPrice = 52.80,
                    LowPrice = 51.85,
                    Time = 9
                },
                //Date 10
                new Bar
                {
                    HighPrice = 52.50,
                    LowPrice = 51.50,
                    Time = 10
                },
                
                //Date 11
                new Bar
                {
                    HighPrice = 53.50,
                    LowPrice = 52.30,
                    Time = 11
                },
                //Date 12
                new Bar
                {
                    HighPrice = 53.50,
                    LowPrice = 52.50,
                    Time = 12
                },         
                //Date 13
                new Bar
                {
                    HighPrice = 53.80,
                    LowPrice = 53.00,
                    Time = 13
                },
                //Date 14
                new Bar
                {
                    HighPrice = 54.20,
                    LowPrice = 53.50,
                    Time = 14
                },
                //Date 15
                new Bar
                {
                    HighPrice = 53.40,
                    LowPrice = 52.50,
                    Time = 15
                },
                //Date 16
                new Bar
                {
                    HighPrice = 53.50,
                    LowPrice = 52.10,
                    Time = 16
                },
                //Date 17
                new Bar
                {
                    HighPrice = 54.40,
                    LowPrice = 53.00,
                    Time = 17
                },
                //Date 18
                new Bar
                {
                    HighPrice = 55.20,
                    LowPrice = 54.00,
                    Time = 18
                },
                //Date 19
                new Bar
                {
                    HighPrice = 55.70,
                    LowPrice = 55.00,
                    Time = 19
                },
                //Date 20
                new Bar
                {
                    HighPrice = 57.00,
                    LowPrice = 56.00,
                    Time = 20
                },
                //Date 21
                new Bar
                {
                    HighPrice = 57.50,
                    LowPrice = 56.50,
                    Time = 21
                },
                //Date 22
                new Bar
                {
                    HighPrice = 58.00,
                    LowPrice = 57.00,
                    Time = 22
                },
                //Date 23
                new Bar
                {
                    HighPrice = 57.70,
                    LowPrice = 56.50,
                    Time = 23
                },
                //Date 24
                new Bar
                {
                    HighPrice = 58.00,
                    LowPrice = 57.30,
                    Time = 24
                },
                //Date 25
                new Bar
                {
                    HighPrice = 57.50,
                    LowPrice = 56.70,
                    Time = 25
                },
                //Date 26
                new Bar
                {
                    HighPrice = 57.00,
                    LowPrice = 56.30,
                    Time = 26
                },
                //Date 27
                new Bar
                {
                    HighPrice = 56.70,
                    LowPrice = 56.20,
                    Time = 27
                },
                //Date 28
                new Bar
                {
                    HighPrice = 57.50,
                    LowPrice = 56.00,
                    Time = 28
                },
                //Date 29
                new Bar
                {
                    HighPrice = 56.70,
                    LowPrice = 55.50,
                    Time = 29
                },
                //Date 30
                new Bar
                {
                    HighPrice = 56.00,
                    LowPrice = 55.00,
                    Time = 30
                },
                //Date 31
                new Bar
                {
                    HighPrice = 56.20,
                    LowPrice = 54.90,
                    Time = 31
                },
                //Date 32
                new Bar
                {
                    HighPrice = 54.80,
                    LowPrice = 54.00,
                    Time = 32
                },
                //Date 33
                new Bar
                {
                    HighPrice = 55.50,
                    LowPrice = 54.50,
                    Time = 33
                },
                //Date 34
                new Bar
                {
                    HighPrice = 54.70,
                    LowPrice = 53.80,
                    Time = 34
                },
                //Date 35
                new Bar
                {
                    HighPrice = 54.00,
                    LowPrice = 53.00,
                    Time = 35
                },       
                //Date 36
                new Bar
                {
                    HighPrice = 52.50,
                    LowPrice = 51.50,
                    Time = 36
                },
                //Date 37
                new Bar
                {
                    HighPrice = 51.00,
                    LowPrice = 50.00,
                    Time = 37
                },          
                //Date 38
                new Bar
                {
                    HighPrice = 51.50,
                    LowPrice = 50.50,
                    Time = 38
                },
                //Date 39
                new Bar
                {
                    HighPrice = 51.70,
                    LowPrice = 50.20,
                    Time = 39
                },        
                //Date 40
                new Bar
                {
                    HighPrice = 53.00,
                    LowPrice = 51.50,
                    Time = 40
                },
            };
        }
    }
}
