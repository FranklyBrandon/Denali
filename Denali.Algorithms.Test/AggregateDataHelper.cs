using Denali.Algorithms.Test.Models;
using Denali.Models.Polygon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Linq;
using Denali.Models.Shared;
using Denali.Algorithms.AggregateAnalysis.ADX;

namespace Denali.Algorithms.Test
{
    public static class AggregateDataHelper
    {
        public const string AAPL_STOCK_DATA = "StockDataFile1.txt";

        public static AggregateResponse GetStockDataFile(string dataFile)
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), dataFile);
            string text = File.ReadAllText(path);
            return JsonSerializer.Deserialize<AggregateResponse>(text);
        }

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
                new Bar
                {
                    HighPrice = 52.35M,
                    LowPrice = 51.50M,
                    Time = 4
                },
                //Date 5
                new Bar
                {
                    HighPrice = 52.10M,
                    LowPrice = 51.00M,
                    Time = 5
                },
                //Date 6
                new Bar
                {
                    HighPrice = 51.80M,
                    LowPrice = 50.50M,
                    Time = 6
                },
                //Date 7
                new Bar
                {
                    HighPrice = 52.10M,
                    LowPrice = 51.25M,
                    Time = 7
                },
                //Date 8
                new Bar
                {
                    HighPrice = 52.50M,
                    LowPrice = 51.70M,
                    Time = 8
                },
                //Date 9
                new Bar
                {
                    HighPrice = 52.80M,
                    LowPrice = 51.85M,
                    Time = 9
                },
                //Date 10
                new Bar
                {
                    HighPrice = 52.50M,
                    LowPrice = 51.50M,
                    Time = 10
                },
                
                //Date 11
                new Bar
                {
                    HighPrice = 53.50M,
                    LowPrice = 52.30M,
                    Time = 11
                },
                //Date 12
                new Bar
                {
                    HighPrice = 53.50M,
                    LowPrice = 52.50M,
                    Time = 12
                },         
                //Date 13
                new Bar
                {
                    HighPrice = 53.80M,
                    LowPrice = 53.00M,
                    Time = 13
                },
                //Date 14
                new Bar
                {
                    HighPrice = 54.20M,
                    LowPrice = 53.50M,
                    Time = 14
                },
                //Date 15
                new Bar
                {
                    HighPrice = 53.40M,
                    LowPrice = 52.50M,
                    Time = 15
                },
                //Date 16
                new Bar
                {
                    HighPrice = 53.50M,
                    LowPrice = 52.10M,
                    Time = 16
                },
                //Date 17
                new Bar
                {
                    HighPrice = 54.40M,
                    LowPrice = 53.00M,
                    Time = 17
                },
                //Date 18
                new Bar
                {
                    HighPrice = 55.20M,
                    LowPrice = 54.00M,
                    Time = 18
                },
                //Date 19
                new Bar
                {
                    HighPrice = 55.70M,
                    LowPrice = 55.00M,
                    Time = 19
                },
                //Date 20
                new Bar
                {
                    HighPrice = 57.00M,
                    LowPrice = 56.00M,
                    Time = 20
                },
                //Date 21
                new Bar
                {
                    HighPrice = 57.50M,
                    LowPrice = 56.50M,
                    Time = 21
                },
                //Date 22
                new Bar
                {
                    HighPrice = 58.00M,
                    LowPrice = 57.00M,
                    Time = 22
                },
                //Date 23
                new Bar
                {
                    HighPrice = 57.70M,
                    LowPrice = 56.50M,
                    Time = 23
                },
                //Date 24
                new Bar
                {
                    HighPrice = 58.00M,
                    LowPrice = 57.30M,
                    Time = 24
                },
                //Date 25
                new Bar
                {
                    HighPrice = 57.50M,
                    LowPrice = 56.70M,
                    Time = 25
                },
                //Date 26
                new Bar
                {
                    HighPrice = 57.00M,
                    LowPrice = 56.30M,
                    Time = 26
                },
                //Date 27
                new Bar
                {
                    HighPrice = 56.70M,
                    LowPrice = 56.20M,
                    Time = 27
                },
                //Date 28
                new Bar
                {
                    HighPrice = 57.50M,
                    LowPrice = 56.00M,
                    Time = 28
                },
                //Date 29
                new Bar
                {
                    HighPrice = 56.70M,
                    LowPrice = 55.50M,
                    Time = 29
                },
                //Date 30
                new Bar
                {
                    HighPrice = 56.00M,
                    LowPrice = 55.00M,
                    Time = 30
                },
                //Date 31
                new Bar
                {
                    HighPrice = 56.20M,
                    LowPrice = 54.90M,
                    Time = 31
                },
                //Date 32
                new Bar
                {
                    HighPrice = 54.80M,
                    LowPrice = 54.00M,
                    Time = 32
                },
                //Date 33
                new Bar
                {
                    HighPrice = 55.50M,
                    LowPrice = 54.50M,
                    Time = 33
                },
                //Date 34
                new Bar
                {
                    HighPrice = 54.70M,
                    LowPrice = 53.80M,
                    Time = 34
                },
                //Date 35
                new Bar
                {
                    HighPrice = 54.00M,
                    LowPrice = 53.00M,
                    Time = 35
                },       
                //Date 36
                new Bar
                {
                    HighPrice = 52.50M,
                    LowPrice = 51.50M,
                    Time = 36
                },
                //Date 37
                new Bar
                {
                    HighPrice = 51.00M,
                    LowPrice = 50.00M,
                    Time = 37
                },          
                //Date 38
                new Bar
                {
                    HighPrice = 51.50M,
                    LowPrice = 50.50M,
                    Time = 38
                },
                //Date 39
                new Bar
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
                new Bar
                {
                    HighPrice = 274M,
                    LowPrice = 272M,
                    ClosePrice = 272.75M,
                    Time = 1
                },
                new Bar
                {
                    HighPrice = 273.25M,
                    LowPrice = 270.25M,
                    ClosePrice = 270.75M,
                    Time = 2
                },
                new Bar
                {
                    HighPrice = 272M,
                    LowPrice = 269.75M,
                    ClosePrice = 270M,
                    Time = 3
                },
                new Bar
                {
                    HighPrice = 270.75M,
                    LowPrice = 268M,
                    ClosePrice = 269.25M,
                    Time = 4
                },
                new Bar
                {
                    HighPrice = 270M,
                    LowPrice = 269M,
                    ClosePrice = 269.75M,
                    Time = 5
                },
                new Bar
                {
                    HighPrice = 270.50M,
                    LowPrice = 268M,
                    ClosePrice = 270M,
                    Time = 6
                },
                new Bar
                {
                    HighPrice = 268.50M,
                    LowPrice = 266.50M,
                    ClosePrice = 266.50M,
                    Time = 7
                },
                new Bar
                {
                    HighPrice = 265.50M,
                    LowPrice = 263M,
                    ClosePrice = 263.25M,
                    Time = 8
                },
                new Bar
                {
                    HighPrice = 262.50M,
                    LowPrice = 259M,
                    ClosePrice = 260.25M,
                    Time = 9
                },
                new Bar
                {
                    HighPrice = 263.50M,
                    LowPrice = 260M,
                    ClosePrice = 263M,
                    Time = 10
                },
                new Bar
                {
                    HighPrice = 269.50M,
                    LowPrice = 263M,
                    ClosePrice = 266.50M,
                    Time = 11
                },
                new Bar
                {
                    HighPrice = 267.25M,
                    LowPrice = 265M,
                    ClosePrice = 267M,
                    Time = 12
                },
                new Bar
                {
                    HighPrice = 267.50M,
                    LowPrice = 265.50M,
                    ClosePrice = 265.75M,
                    Time = 13
                },
                new Bar
                {
                    HighPrice = 269.75M,
                    LowPrice = 266M,
                    ClosePrice = 268.50M,
                    Time = 14
                }
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
