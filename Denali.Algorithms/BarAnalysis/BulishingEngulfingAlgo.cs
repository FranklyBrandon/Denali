using Denali.Models.Alpaca;
using Denali.Models.Polygon;
using Denali.Models.Trading;
using Denali.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Denali.Algorithms.BarAnalysis
{
    public class BulishingEngulfingAlgo : IBarSignalAlgorithm
    {
        private readonly TimeUtils _timeUtils;
        //public StockAction Analyze(KeyValuePair<string,List<Candle>> barData)
        //{
        //    var size = barData.Value.Count - 1;
        //    var previous = barData.Value[size - 1];
        //    var current = barData.Value[size];

        //    //Bullish
        //    if (previous.IsClosed && current.IsOpen)
        //    {
        //        if (IsEngulfing(previous, current, true))
        //        {
        //            //var startTime = _timeUtils.GetNewYorkTimeFromEpoch(previous.Timestamp).LocalTime.ToString("g");
        //            //var endTime = _timeUtils.GetNewYorkTimeFromEpoch(current.Timestamp).LocalTime.ToString("g");
        //            //Console.WriteLine($"Bullish Engulfing start at {startTime} and ending at {endTime}");
        //            return new StockAction
        //            {
        //                StockSymbol = barData.Key,
        //                Side = MarketSide.Buy,
        //                Signal = new Signal
        //                {
        //                    Type = SignalType.BullishEngulfing,
        //                    PositionType = PositionType.Long,
        //                    StartTime = previous.Timestamp,
        //                    EndTime = current.Timestamp
        //                },

        //            };
        //            //return new Signal(SignalType.BullishEngulfing, Models.Data.Trading.MarketAction.Long, previous.Timestamp, current.Timestamp, previous.LowPrice, current.ClosePrice + .20);
        //        }
        //    }
        //    //Bearish
        //    else if (previous.IsOpen && current.IsClosed)
        //    {
        //        if (IsEngulfing(previous, current, false))
        //        {
        //            //var startTime = _timeUtils.GetNewYorkTimeFromEpoch(previous.Timestamp).LocalTime.ToString("g");
        //            //var endTime = _timeUtils.GetNewYorkTimeFromEpoch(current.Timestamp).LocalTime.ToString("g");
        //            //Console.WriteLine($"Bearish Engulfing start at {startTime} and ending at {endTime}");
        //            return default;
        //        }
        //    }

        //    return default;
        //}
        public BulishingEngulfingAlgo()
        {
            _timeUtils = new TimeUtils();
        }
        public void Analyze(IList<Bar> barData)
        {
            var size = barData.Count - 1;
            var previous = barData[size - 1];
            var current = barData[size];

            if (previous.IsClosed && current.IsOpen)
            {
                if (IsEngulfing(previous, current, true))
                {
                    var time = _timeUtils.GetETDatetimefromUnixMS(current.Time);
                    Console.WriteLine($"Bullish Engulfing recognized: {time.ToString()}");
                }
            }
        }

        private bool IsEngulfing(Bar first, Bar last, bool bullish)
        {
            if (bullish && first.OpenPrice < last.ClosePrice && first.ClosePrice > last.OpenPrice)
                return true;

            if (!bullish && first.ClosePrice < last.OpenPrice && first.OpenPrice > last.ClosePrice)
                return true;

            return false;
        }
    }
}
