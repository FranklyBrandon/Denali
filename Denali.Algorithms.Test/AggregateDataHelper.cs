using Denali.Algorithms.Test.Models;
using Denali.Models.Polygon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Linq;

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
        public static List<Bar> JWellesWilderSARExampleSheet()
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
                    SAR = 50.00,
                    ExtremePoint = 52.35,
                    AccelerationFactor = 0.02,
                    Time = 4               
                },
                new ParabolicSARResult
                {
                    SAR = 50.05,
                    ExtremePoint = 52.35,
                    AccelerationFactor = 0.02,
                    Time = 5
                },
                new ParabolicSARResult
                {
                    SAR = 50.10,
                    ExtremePoint = 52.35,
                    AccelerationFactor = 0.02,
                    Time = 6
                },
                new ParabolicSARResult
                {
                    SAR = 50.15,
                    ExtremePoint = 52.35,
                    AccelerationFactor = 0.02,
                    Time = 7
                },
                new ParabolicSARResult
                {
                    SAR = 50.19,
                    ExtremePoint = 52.50,
                    AccelerationFactor = 0.04,
                    Time = 8
                },
                new ParabolicSARResult
                {
                    SAR = 50.28,
                    ExtremePoint = 50.28,
                    AccelerationFactor = 0.06,
                    Time = 9
                },
                new ParabolicSARResult
                {
                    SAR = 50.43,
                    ExtremePoint = 52.80,
                    AccelerationFactor = 0.06,
                    Time = 10
                },
                new ParabolicSARResult
                {
                    SAR = 50.57,
                    ExtremePoint = 53.50,
                    AccelerationFactor = 0.08,
                    Time = 11
                },
                new ParabolicSARResult
                {
                    SAR = 50.80,
                    ExtremePoint = 53.50,
                    AccelerationFactor = 0.08,
                    Time = 12
                },
                new ParabolicSARResult
                {
                    SAR = 51.02,
                    ExtremePoint = 53.80,
                    AccelerationFactor = 0.10,
                    Time = 13
                },
                new ParabolicSARResult
                {
                    SAR = 51.30,
                    ExtremePoint = 54.20,
                    AccelerationFactor = 0.12,
                    Time = 14
                },
                new ParabolicSARResult
                {
                    SAR = 51.67,
                    ExtremePoint = 54.20,
                    AccelerationFactor = 0.12,
                    Time = 15
                },
                new ParabolicSARResult
                {
                    SAR = 51.96,
                    ExtremePoint = 54.20,
                    AccelerationFactor = 0.12,
                    Time = 16
                },
                new ParabolicSARResult
                {
                    SAR = 52.10,
                    ExtremePoint = 54.40,
                    AccelerationFactor = 0.14,
                    Time = 17
                },

                new ParabolicSARResult
                {
                    SAR = 52.10,
                    ExtremePoint = 55.20,
                    AccelerationFactor = 0.16,
                    Time = 18
                },

                new ParabolicSARResult
                {
                    SAR = 52.60,
                    ExtremePoint = 55.70,
                    AccelerationFactor = 0.18,
                    Time = 19
                },

                new ParabolicSARResult
                {
                    SAR = 53.16,
                    ExtremePoint = 57.00,
                    AccelerationFactor = 0.20,
                    Time = 20
                },
                new ParabolicSARResult
                {
                    SAR = 53.93,
                    ExtremePoint = 57.50,
                    AccelerationFactor = 0.20,
                    Time = 21
                },
                new ParabolicSARResult
                {
                    SAR = 54.64,
                    ExtremePoint = 58.00,
                    AccelerationFactor = 0.20,
                    Time = 22
                },
                new ParabolicSARResult
                {
                    SAR = 55.31,
                    ExtremePoint = 58.00,
                    AccelerationFactor = 0.20,
                    Time = 23
                },
                new ParabolicSARResult
                {
                    SAR = 55.85,
                    ExtremePoint = 58.00,
                    AccelerationFactor = 0.20,
                    Time = 24
                },
                new ParabolicSARResult
                {
                    SAR = 56.62,
                    ExtremePoint = 58.00,
                    AccelerationFactor = 0.20,
                    Time = 25
                },
                new ParabolicSARResult
                {
                    SAR = 58.00,
                    ExtremePoint = 56.30,
                    AccelerationFactor = 0.02,
                    Time = 26
                },

                new ParabolicSARResult
                {
                    SAR = 57.97,
                    ExtremePoint = 56.20,
                    AccelerationFactor = 0.04,
                    Time = 27
                },
                new ParabolicSARResult
                {
                    SAR = 57.90,
                    ExtremePoint = 56.00,
                    AccelerationFactor = 0.06,
                    Time = 28
                },
                new ParabolicSARResult
                {
                    SAR = 57.79,
                    ExtremePoint = 55.50,
                    AccelerationFactor = 0.08,
                    Time = 29
                },
                new ParabolicSARResult
                {
                    SAR = 57.61,
                    ExtremePoint = 55.00,
                    AccelerationFactor = 0.10,
                    Time = 30
                },
                new ParabolicSARResult
                {
                    SAR = 57.35,
                    ExtremePoint = 54.90,
                    AccelerationFactor = 0.12,
                    Time = 31
                },
                new ParabolicSARResult
                {
                    SAR = 57.06,
                    ExtremePoint = 54.00,
                    AccelerationFactor = 0.14,
                    Time = 32
                },
                new ParabolicSARResult
                {
                    SAR = 56.63,
                    ExtremePoint = 54.00,
                    AccelerationFactor = 0.14,
                    Time = 33
                },
                new ParabolicSARResult
                {
                    SAR = 56.26,
                    ExtremePoint = 53.80,
                    AccelerationFactor = 0.16,
                    Time = 34
                },
                new ParabolicSARResult
                {
                    SAR = 55.87,
                    ExtremePoint = 53.00,
                    AccelerationFactor = 0.18,
                    Time = 35
                },
                new ParabolicSARResult
                {
                    SAR = 55.35,
                    ExtremePoint = 51.50,
                    AccelerationFactor = 0.20,
                    Time = 36
                },
                new ParabolicSARResult
                {
                    SAR = 54.58,
                    ExtremePoint = 50.00,
                    AccelerationFactor = 0.20,
                    Time = 37
                },
                new ParabolicSARResult
                {
                    SAR = 53.66,
                    ExtremePoint = 50.00,
                    AccelerationFactor = 0.20,
                    Time = 38
                },
                new ParabolicSARResult
                {
                    SAR = 52.34,
                    ExtremePoint = 50.00,
                    AccelerationFactor = 0.20,
                    Time = 39
                }
            }.ToDictionary(x => x.Time, y => y);
        }
    }
}
