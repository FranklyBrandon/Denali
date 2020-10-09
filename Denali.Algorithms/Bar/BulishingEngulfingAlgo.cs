using Denali.Models.Data.Alpaca;
using Denali.Models.Data.Trading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Denali.Algorithms.Bar
{
    public class BulishingEngulfingAlgo : IBarSignalAlgorithm
    {
        public StockAction Analyze(KeyValuePair<string,List<Candle>> barData)
        {
            var size = barData.Value.Count - 1;
            var previous = barData.Value[size - 1];
            var current = barData.Value[size];

            //Bullish
            if (previous.IsClosed && current.IsOpen)
            {
                if (IsEngulfing(previous, current, true))
                {
                    //var startTime = _timeUtils.GetNewYorkTimeFromEpoch(previous.Timestamp).LocalTime.ToString("g");
                    //var endTime = _timeUtils.GetNewYorkTimeFromEpoch(current.Timestamp).LocalTime.ToString("g");
                    //Console.WriteLine($"Bullish Engulfing start at {startTime} and ending at {endTime}");
                    return new StockAction
                    {
                        StockSymbol = barData.Key,
                        Side = MarketSide.Buy,
                        Signal = new Signal
                        {
                            Type = SignalType.BullishEngulfing,
                            PositionType = PositionType.Long,
                            StartTime = previous.Timestamp,
                            EndTime = current.Timestamp
                        },
                        
                    };
                    //return new Signal(SignalType.BullishEngulfing, Models.Data.Trading.MarketAction.Long, previous.Timestamp, current.Timestamp, previous.LowPrice, current.ClosePrice + .20);
                }
            }
            //Bearish
            else if (previous.IsOpen && current.IsClosed)
            {
                if (IsEngulfing(previous, current, false))
                {
                    //var startTime = _timeUtils.GetNewYorkTimeFromEpoch(previous.Timestamp).LocalTime.ToString("g");
                    //var endTime = _timeUtils.GetNewYorkTimeFromEpoch(current.Timestamp).LocalTime.ToString("g");
                    //Console.WriteLine($"Bearish Engulfing start at {startTime} and ending at {endTime}");
                    return default;
                }
            }

            return default;
        }

        private bool IsEngulfing(Candle first, Candle last, bool bullish)
        {
            if (bullish && first.OpenPrice < last.ClosePrice && first.ClosePrice > last.OpenPrice)
                return true;

            if (!bullish && first.ClosePrice < last.OpenPrice && first.OpenPrice > last.ClosePrice)
                return true;

            return false;
        }
    }
}
