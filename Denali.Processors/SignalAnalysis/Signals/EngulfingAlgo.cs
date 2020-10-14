using Denali.Models.Alpaca;
using Denali.Models.Trading;
using Denali.Services.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Processors.SignalAnalysis.Signals
{
    public class EngulfingAlgo
    {
        private readonly TimeUtils _timeUtils;

        public EngulfingAlgo(TimeUtils timeUtils)
        {
            _timeUtils = timeUtils;
        }

        public Signal Process(List<Candle> candles, int index, bool first, bool last)
        {
            //Ignore first tick data of day
            if (first)
                return null;

            var previous = candles[index -1];
            var current = candles[index];

            //Bullish
            if (previous.IsClosed && current.IsOpen)
            {
                if (IsEngulfing(previous, current, true))
                {
                    var startTime = _timeUtils.GetNewYorkTimeFromEpoch(previous.Timestamp).LocalTime.ToString("g");
                    var endTime = _timeUtils.GetNewYorkTimeFromEpoch(current.Timestamp).LocalTime.ToString("g");
                    Console.WriteLine($"Bullish Engulfing start at {startTime} and ending at {endTime}");
                    //return new Signal(SignalType.BullishEngulfing, Models.Data.Trading.MarketAction.Long, previous.Timestamp, current.Timestamp, previous.LowPrice, current.ClosePrice + .20);
                }
            }
            //Bearish
            else if (previous.IsOpen && current.IsClosed)
            {
                if (IsEngulfing(previous, current, false))
                {
                    var startTime = _timeUtils.GetNewYorkTimeFromEpoch(previous.Timestamp).LocalTime.ToString("g");
                    var endTime = _timeUtils.GetNewYorkTimeFromEpoch(current.Timestamp).LocalTime.ToString("g");
                    //Console.WriteLine($"Bearish Engulfing start at {startTime} and ending at {endTime}");
                    return null;
                }
            }

            return null;
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
