using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Services.Aggregators
{
    public abstract class BaseAggregator 
    {
        protected int _minuteInterval;
        protected int _lastUpdateMinute;

        public void SetLastUpdateMinute(int minute) => _lastUpdateMinute = minute;
        public void SetMinuteInterval(int minuteInterval) => _minuteInterval = minuteInterval;

        public decimal Round(decimal value)
        {
            return Math.Floor(value / _minuteInterval) * _minuteInterval;
        }
    }
}
