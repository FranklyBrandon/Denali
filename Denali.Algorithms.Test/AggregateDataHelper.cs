using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Linq;
using Denali.Models.Shared;
using Denali.Algorithms.AggregateAnalysis.ADX;
using Denali.Algorithms.Test.Models;

namespace Denali.Algorithms.Test
{
    public static class AggregateDataHelper
    {
        public const string AAPL_STOCK_DATA = "StockDataFile1.txt";

        /// <summary>
        /// The example bar data given on page 12 of "New Concepts in Technical Trading Systems" 
        /// Wilder uses this example data to demonstrate the parabolic stop and reversal calculation
        /// </summary>
        /// <returns></returns>
        public static List<IAggregateData> JWellesWilderSARExampleSheet()
        {
            return new List<IAggregateData>
            {
                //Date 4
                new AggregateData
                {
                    HighPrice = 52.35M,
                    LowPrice = 51.50M,
                    Time = 4
                },
                //Date 5
                new AggregateData
                {
                    HighPrice = 52.10M,
                    LowPrice = 51.00M,
                    Time = 5
                },
                //Date 6
                new AggregateData
                {
                    HighPrice = 51.80M,
                    LowPrice = 50.50M,
                    Time = 6
                },
                //Date 7
                new AggregateData
                {
                    HighPrice = 52.10M,
                    LowPrice = 51.25M,
                    Time = 7
                },
                //Date 8
                new AggregateData
                {
                    HighPrice = 52.50M,
                    LowPrice = 51.70M,
                    Time = 8
                },
                //Date 9
                new AggregateData
                {
                    HighPrice = 52.80M,
                    LowPrice = 51.85M,
                    Time = 9
                },
                //Date 10
                new AggregateData
                {
                    HighPrice = 52.50M,
                    LowPrice = 51.50M,
                    Time = 10
                },
                
                //Date 11
                new AggregateData
                {
                    HighPrice = 53.50M,
                    LowPrice = 52.30M,
                    Time = 11
                },
                //Date 12
                new AggregateData
                {
                    HighPrice = 53.50M,
                    LowPrice = 52.50M,
                    Time = 12
                },         
                //Date 13
                new AggregateData
                {
                    HighPrice = 53.80M,
                    LowPrice = 53.00M,
                    Time = 13
                },
                //Date 14
                new AggregateData
                {
                    HighPrice = 54.20M,
                    LowPrice = 53.50M,
                    Time = 14
                },
                //Date 15
                new AggregateData
                {
                    HighPrice = 53.40M,
                    LowPrice = 52.50M,
                    Time = 15
                },
                //Date 16
                new AggregateData
                {
                    HighPrice = 53.50M,
                    LowPrice = 52.10M,
                    Time = 16
                },
                //Date 17
                new AggregateData
                {
                    HighPrice = 54.40M,
                    LowPrice = 53.00M,
                    Time = 17
                },
                //Date 18
                new AggregateData
                {
                    HighPrice = 55.20M,
                    LowPrice = 54.00M,
                    Time = 18
                },
                //Date 19
                new AggregateData
                {
                    HighPrice = 55.70M,
                    LowPrice = 55.00M,
                    Time = 19
                },
                //Date 20
                new AggregateData
                {
                    HighPrice = 57.00M,
                    LowPrice = 56.00M,
                    Time = 20
                },
                //Date 21
                new AggregateData
                {
                    HighPrice = 57.50M,
                    LowPrice = 56.50M,
                    Time = 21
                },
                //Date 22
                new AggregateData
                {
                    HighPrice = 58.00M,
                    LowPrice = 57.00M,
                    Time = 22
                },
                //Date 23
                new AggregateData
                {
                    HighPrice = 57.70M,
                    LowPrice = 56.50M,
                    Time = 23
                },
                //Date 24
                new AggregateData
                {
                    HighPrice = 58.00M,
                    LowPrice = 57.30M,
                    Time = 24
                },
                //Date 25
                new AggregateData
                {
                    HighPrice = 57.50M,
                    LowPrice = 56.70M,
                    Time = 25
                },
                //Date 26
                new AggregateData
                {
                    HighPrice = 57.00M,
                    LowPrice = 56.30M,
                    Time = 26
                },
                //Date 27
                new AggregateData
                {
                    HighPrice = 56.70M,
                    LowPrice = 56.20M,
                    Time = 27
                },
                //Date 28
                new AggregateData
                {
                    HighPrice = 57.50M,
                    LowPrice = 56.00M,
                    Time = 28
                },
                //Date 29
                new AggregateData
                {
                    HighPrice = 56.70M,
                    LowPrice = 55.50M,
                    Time = 29
                },
                //Date 30
                new AggregateData
                {
                    HighPrice = 56.00M,
                    LowPrice = 55.00M,
                    Time = 30
                },
                //Date 31
                new AggregateData
                {
                    HighPrice = 56.20M,
                    LowPrice = 54.90M,
                    Time = 31
                },
                //Date 32
                new AggregateData
                {
                    HighPrice = 54.80M,
                    LowPrice = 54.00M,
                    Time = 32
                },
                //Date 33
                new AggregateData
                {
                    HighPrice = 55.50M,
                    LowPrice = 54.50M,
                    Time = 33
                },
                //Date 34
                new AggregateData
                {
                    HighPrice = 54.70M,
                    LowPrice = 53.80M,
                    Time = 34
                },
                //Date 35
                new AggregateData
                {
                    HighPrice = 54.00M,
                    LowPrice = 53.00M,
                    Time = 35
                },       
                //Date 36
                new AggregateData
                {
                    HighPrice = 52.50M,
                    LowPrice = 51.50M,
                    Time = 36
                },
                //Date 37
                new AggregateData
                {
                    HighPrice = 51.00M,
                    LowPrice = 50.00M,
                    Time = 37
                },          
                //Date 38
                new AggregateData
                {
                    HighPrice = 51.50M,
                    LowPrice = 50.50M,
                    Time = 38
                },
                //Date 39
                new AggregateData
                {
                    HighPrice = 51.70M,
                    LowPrice = 50.20M,
                    Time = 39
                }
            };
        }

        /// <summary>
        /// The example parabolic SAR calculation results given on page 13 of "New Concepts in Technical Trading Systems" 
        /// </summary>
        /// <returns></returns>
        public static Dictionary<long, ParabolicSARResult> JWellsWilderSARResults()
        {
            return new List<ParabolicSARResult>()
            {
                new ParabolicSARResult
                {
                    SAR = 50.00M,
                    ExtremePoint = 52.35M,
                    AccelerationFactor = 0.02M,
                    Time = 4               
                },
                new ParabolicSARResult
                {
                    SAR = 50.05M,
                    ExtremePoint = 52.35M,
                    AccelerationFactor = 0.02M,
                    Time = 5
                },
                new ParabolicSARResult
                {
                    SAR = 50.10M,
                    ExtremePoint = 52.35M,
                    AccelerationFactor = 0.02M,
                    Time = 6
                },
                new ParabolicSARResult
                {
                    SAR = 50.15M,
                    ExtremePoint = 52.35M,
                    AccelerationFactor = 0.02M,
                    Time = 7
                },
                new ParabolicSARResult
                {
                    SAR = 50.19M,
                    ExtremePoint = 52.50M,
                    AccelerationFactor = 0.04M,
                    Time = 8
                },
                new ParabolicSARResult
                {
                    SAR = 50.28M,
                    ExtremePoint = 50.28M,
                    AccelerationFactor = 0.06M,
                    Time = 9
                },
                new ParabolicSARResult
                {
                    SAR = 50.43M,
                    ExtremePoint = 52.80M,
                    AccelerationFactor = 0.06M,
                    Time = 10
                },
                new ParabolicSARResult
                {
                    SAR = 50.57M,
                    ExtremePoint = 53.50M,
                    AccelerationFactor = 0.08M,
                    Time = 11
                },
                new ParabolicSARResult
                {
                    SAR = 50.80M,
                    ExtremePoint = 53.50M,
                    AccelerationFactor = 0.08M,
                    Time = 12
                },
                new ParabolicSARResult
                {
                    SAR = 51.02M,
                    ExtremePoint = 53.80M,
                    AccelerationFactor = 0.10M,
                    Time = 13
                },
                new ParabolicSARResult
                {
                    SAR = 51.30M,
                    ExtremePoint = 54.20M,
                    AccelerationFactor = 0.12M,
                    Time = 14
                },
                new ParabolicSARResult
                {
                    SAR = 51.65M,
                    ExtremePoint = 54.20M,
                    AccelerationFactor = 0.12M,
                    Time = 15
                },
                new ParabolicSARResult
                {
                    SAR = 51.96M,
                    ExtremePoint = 54.20M,
                    AccelerationFactor = 0.12M,
                    Time = 16
                },
                new ParabolicSARResult
                {
                    SAR = 52.10M,
                    ExtremePoint = 54.40M,
                    AccelerationFactor = 0.14M,
                    Time = 17
                },

                new ParabolicSARResult
                {
                    SAR = 52.10M,
                    ExtremePoint = 55.20M,
                    AccelerationFactor = 0.16M,
                    Time = 18
                },

                new ParabolicSARResult
                {
                    SAR = 52.60M,
                    ExtremePoint = 55.70M,
                    AccelerationFactor = 0.18M,
                    Time = 19
                },

                new ParabolicSARResult
                {
                    SAR = 53.16M,
                    ExtremePoint = 57.00M,
                    AccelerationFactor = 0.20M,
                    Time = 20
                },
                new ParabolicSARResult
                {
                    SAR = 53.93M,
                    ExtremePoint = 57.50M,
                    AccelerationFactor = 0.20M,
                    Time = 21
                },
                new ParabolicSARResult
                {
                    SAR = 54.64M,
                    ExtremePoint = 58.00M,
                    AccelerationFactor = 0.20M,
                    Time = 22
                },
                new ParabolicSARResult
                {
                    SAR = 55.31M,
                    ExtremePoint = 58.00M,
                    AccelerationFactor = 0.20M,
                    Time = 23
                },
                new ParabolicSARResult
                {
                    SAR = 55.85M,
                    ExtremePoint = 58.00M,
                    AccelerationFactor = 0.20M,
                    Time = 24
                },
                new ParabolicSARResult
                {
                    SAR = 56.28M,
                    ExtremePoint = 58.00M,
                    AccelerationFactor = 0.20M,
                    Time = 25
                },
                new ParabolicSARResult
                {
                    SAR = 58.00M,
                    ExtremePoint = 56.30M,
                    AccelerationFactor = 0.02M,
                    Time = 26
                },

                new ParabolicSARResult
                {
                    SAR = 57.97M,
                    ExtremePoint = 56.20M,
                    AccelerationFactor = 0.04M,
                    Time = 27
                },
                new ParabolicSARResult
                {
                    SAR = 57.90M,
                    ExtremePoint = 56.00M,
                    AccelerationFactor = 0.06M,
                    Time = 28
                },
                new ParabolicSARResult
                {
                    SAR = 57.79M,
                    ExtremePoint = 55.50M,
                    AccelerationFactor = 0.08M,
                    Time = 29
                },
                new ParabolicSARResult
                {
                    SAR = 57.61M,
                    ExtremePoint = 55.00M,
                    AccelerationFactor = 0.10M,
                    Time = 30
                },
                new ParabolicSARResult
                {
                    SAR = 57.35M,
                    ExtremePoint = 54.90M,
                    AccelerationFactor = 0.12M,
                    Time = 31
                },
                new ParabolicSARResult
                {
                    SAR = 57.06M,
                    ExtremePoint = 54.00M,
                    AccelerationFactor = 0.14M,
                    Time = 32
                },
                new ParabolicSARResult
                {
                    SAR = 56.63M,
                    ExtremePoint = 54.00M,
                    AccelerationFactor = 0.14M,
                    Time = 33
                },
                new ParabolicSARResult
                {
                    SAR = 56.26M,
                    ExtremePoint = 53.80M,
                    AccelerationFactor = 0.16M,
                    Time = 34
                },
                new ParabolicSARResult
                {
                    SAR = 55.87M,
                    ExtremePoint = 53.00M,
                    AccelerationFactor = 0.18M,
                    Time = 35
                },
                new ParabolicSARResult
                {
                    SAR = 55.35M,
                    ExtremePoint = 51.50M,
                    AccelerationFactor = 0.20M,
                    Time = 36
                },
                new ParabolicSARResult
                {
                    SAR = 54.58M,
                    ExtremePoint = 50.00M,
                    AccelerationFactor = 0.20M,
                    Time = 37
                },
                new ParabolicSARResult
                {
                    SAR = 53.66M,
                    ExtremePoint = 50.00M,
                    AccelerationFactor = 0.20M,
                    Time = 38
                },
                new ParabolicSARResult
                {
                    SAR = 52.93M,
                    ExtremePoint = 50.00M,
                    AccelerationFactor = 0.20M,
                    Time = 39
                }
            }.ToDictionary(x => x.Time, y => y);
        }

        public static List<IAggregateData> JWellesWilderADXExampleSheet()
        {
            return new List<IAggregateData>
            {
                new AggregateData
                {
                    HighPrice = 274M,
                    LowPrice = 272M,
                    ClosePrice = 272.75M,
                    Time = 1
                },
                new AggregateData
                {
                    HighPrice = 273.25M,
                    LowPrice = 270.25M,
                    ClosePrice = 270.75M,
                    Time = 2
                },
                new AggregateData
                {
                    HighPrice = 272M,
                    LowPrice = 269.75M,
                    ClosePrice = 270M,
                    Time = 3
                },
                new AggregateData
                {
                    HighPrice = 270.75M,
                    LowPrice = 268M,
                    ClosePrice = 269.25M,
                    Time = 4
                },
                new AggregateData
                {
                    HighPrice = 270M,
                    LowPrice = 269M,
                    ClosePrice = 269.75M,
                    Time = 5
                },
                new AggregateData
                {
                    HighPrice = 270.50M,
                    LowPrice = 268M,
                    ClosePrice = 270M,
                    Time = 6
                },
                new AggregateData
                {
                    HighPrice = 268.50M,
                    LowPrice = 266.50M,
                    ClosePrice = 266.50M,
                    Time = 7
                },
                new AggregateData
                {
                    HighPrice = 265.50M,
                    LowPrice = 263M,
                    ClosePrice = 263.25M,
                    Time = 8
                },
                new AggregateData
                {
                    HighPrice = 262.50M,
                    LowPrice = 259M,
                    ClosePrice = 260.25M,
                    Time = 9
                },
                new AggregateData
                {
                    HighPrice = 263.50M,
                    LowPrice = 260M,
                    ClosePrice = 263M,
                    Time = 10
                },
                new AggregateData
                {
                    HighPrice = 269.50M,
                    LowPrice = 263M,
                    ClosePrice = 266.50M,
                    Time = 11
                },
                new AggregateData
                {
                    HighPrice = 267.25M,
                    LowPrice = 265M,
                    ClosePrice = 267M,
                    Time = 12
                },
                new AggregateData
                {
                    HighPrice = 267.50M,
                    LowPrice = 265.50M,
                    ClosePrice = 265.75M,
                    Time = 13
                },
                new AggregateData
                {
                    HighPrice = 269.75M,
                    LowPrice = 266M,
                    ClosePrice = 268.50M,
                    Time = 14
                },
                new AggregateData
                {
                    HighPrice = 268.25M,
                    LowPrice = 263.25M,
                    ClosePrice = 264.25M,
                    Time = 15
                },
                new AggregateData
                {
                    HighPrice = 264M,
                    LowPrice = 261.50M,
                    ClosePrice = 264M,
                    Time = 16
                },
                new AggregateData
                {
                    HighPrice = 268M,
                    LowPrice = 266.25M,
                    ClosePrice = 266.50M,
                    Time = 17
                },
                new AggregateData
                {
                    HighPrice = 266M,
                    LowPrice = 264.25M,
                    ClosePrice = 265.25M,
                    Time = 18
                },
                new AggregateData
                {
                    HighPrice = 274M,
                    LowPrice = 267M,
                    ClosePrice = 273M,
                    Time = 19
                },
                new AggregateData
                {
                    HighPrice = 277.50M,
                    LowPrice = 273.50M,
                    ClosePrice = 276.75M,
                    Time = 20
                },
                new AggregateData
                {
                    HighPrice = 277M,
                    LowPrice = 272.50M,
                    ClosePrice = 273M,
                    Time = 21
                },
                new AggregateData
                {
                    HighPrice = 272M,
                    LowPrice = 269.50M,
                    ClosePrice = 270.25M,
                    Time = 22
                },
                new AggregateData
                {
                    HighPrice = 267.75M,
                    LowPrice = 264M,
                    ClosePrice = 266.75M,
                    Time = 23
                },
                new AggregateData
                {
                    HighPrice = 269.25M,
                    LowPrice = 263M,
                    ClosePrice = 263M,
                    Time = 24
                },
                new AggregateData
                {
                    HighPrice = 266M,
                    LowPrice = 263.50M,
                    ClosePrice = 265.50M,
                    Time = 25
                },
                new AggregateData
                {
                    HighPrice = 265M,
                    LowPrice = 262M,
                    ClosePrice = 262.25M,
                    Time = 26
                },
                new AggregateData
                {
                    HighPrice = 264.75M,
                    LowPrice = 261.50M,
                    ClosePrice = 262.75M,
                    Time = 27
                },
                new AggregateData
                {
                    HighPrice = 261M,
                    LowPrice = 255.50M,
                    ClosePrice = 255.50M,
                    Time = 28
                },
                new AggregateData
                {
                    HighPrice = 257.50M,
                    LowPrice = 253M,
                    ClosePrice = 253M,
                    Time = 29
                },
                new AggregateData
                {
                    HighPrice = 259M,
                    LowPrice = 254M,
                    ClosePrice = 257.50M,
                    Time = 30
                },
                new AggregateData
                {
                    HighPrice = 259.75M,
                    LowPrice = 257.50M,
                    ClosePrice = 257.50M,
                    Time = 31
                },
                new AggregateData
                {
                    HighPrice = 257.25M,
                    LowPrice = 250M,
                    ClosePrice = 250M,
                    Time = 32
                },
                new AggregateData
                {
                    HighPrice = 250M,
                    LowPrice = 247M,
                    ClosePrice = 249.75M,
                    Time = 33
                },
                new AggregateData
                {
                    HighPrice = 254.25M,
                    LowPrice = 252.75M,
                    ClosePrice = 253.75M,
                    Time = 34
                },
                new AggregateData
                {
                    HighPrice = 254M,
                    LowPrice = 250.50M,
                    ClosePrice = 251.25M,
                    Time = 35
                },
                new AggregateData
                {
                    HighPrice = 253.25M,
                    LowPrice = 250.25M,
                    ClosePrice = 250.50M,
                    Time = 36
                },
                new AggregateData
                {
                    HighPrice = 253.25M,
                    LowPrice = 251M,
                    ClosePrice = 253M,
                    Time = 37
                },
                new AggregateData
                {
                    HighPrice = 251.75M,
                    LowPrice = 250.50M,
                    ClosePrice = 251.50M,
                    Time = 38
                },
                new AggregateData
                {
                    HighPrice = 253M,
                    LowPrice = 249.50M,
                    ClosePrice = 250M,
                    Time = 39
                },
                new AggregateData
                {
                    HighPrice = 251.50M,
                    LowPrice = 245.25M,
                    ClosePrice = 245.75M,
                    Time = 40
                },
                new AggregateData
                {
                    HighPrice = 246.25M,
                    LowPrice = 240M,
                    ClosePrice = 242.75M,
                    Time = 41
                },
                new AggregateData
                {
                    HighPrice = 244.25M,
                    LowPrice = 241.25M,
                    ClosePrice = 243.50M,
                    Time = 42
                },
            };
        }

        /// <summary>
        /// Page 40
        /// </summary>
        /// <returns></returns>
        public static Dictionary<long, ADXResult> JWellesWilderADXResults()
        {
            return new List<ADXResult>
            {
                new ADXResult
                {
                    TrueRange = 3.00M,
                    DMPlus = 0M,
                    DMMinus = 1.75M,
                    Time = 2
                },
                new ADXResult
                {
                    TrueRange = 2.25M,
                    DMPlus = 0M,
                    DMMinus = 0.50M,
                    Time = 3
                },
                new ADXResult
                {
                    TrueRange = 2.75M,
                    DMPlus = 0M,
                    DMMinus = 1.75M,
                    Time = 4
                },
                new ADXResult
                {
                    TrueRange = 1.00M,
                    DMPlus = 0M,
                    DMMinus = 0M,
                    Time = 5
                },
                new ADXResult
                {
                    TrueRange = 2.50M,
                    DMPlus = 0M,
                    DMMinus = 1.00M,
                    Time = 6
                },
                new ADXResult
                {
                    TrueRange = 3.50M,
                    DMPlus = 0M,
                    DMMinus = 1.50M,
                    Time = 7
                },
                new ADXResult
                {
                    TrueRange = 3.50M,
                    DMPlus = 0M,
                    DMMinus = 3.50M,
                    Time = 8
                },
                new ADXResult
                {
                    TrueRange = 4.25M,
                    DMPlus = 0M,
                    DMMinus = 4.00M,
                    Time = 9
                },
                new ADXResult
                {
                    TrueRange = 3.50M,
                    DMPlus = 1.00M,
                    DMMinus = 0M,
                    Time = 10
                },
                new ADXResult
                {
                    TrueRange = 6.50M,
                    DMPlus = 6M,
                    DMMinus = 0M,
                    Time = 11
                },
                new ADXResult
                {
                    TrueRange = 2.25M,
                    DMPlus = 0M,
                    DMMinus = 0M,
                    Time = 12
                },
                new ADXResult
                {
                    TrueRange = 2.00M,
                    DMPlus = 0.25M,
                    DMMinus = 0M,
                    Time = 13
                },
                new ADXResult
                {
                    TrueRange = 4.00M,
                    DMPlus = 2.25M,
                    DMMinus = 0M,
                    Time = 14
                }
            }.ToDictionary(x => x.Time, y => y);
        }
    }
}
