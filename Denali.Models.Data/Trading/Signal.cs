using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Denali.Models.Data.Trading
{
    public class Signal
    {
        public SignalType Type { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public PositionType PositionType { get; set; }

        public Signal()
        {

        }
        public Signal(SignalType type, PositionType positionType, long startTime, long endTime)
        {
            this.Type = type;
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.PositionType = PositionType;
        }

    }

    public enum SignalType
    {
        BullishEngulfing
    }

    public enum MarketSide
    {
        Buy,
        Sell
    }

    public enum PositionType
    {
        Long,
        Short
    }
}
