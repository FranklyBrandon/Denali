using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Denali.Models.Data.Trading
{
    public class Signal
    {
        public SignalType Type { get; }
        public long StartTime { get; }
        public long EndTime { get; }
        public double StopLoss { get; }
        public double Exit { get; }
        public Action Action { get; }

        public Signal(SignalType type, Action action, long startTime, long endTime, double stopLoss, double exit)
        {
            this.Type = type;
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.StopLoss = stopLoss;
            this.Exit = exit;
        }

    }

    public enum SignalType
    {
        BullishEngulfing
    }

    public enum Action
    {
        Long,
        Short
    }
}
