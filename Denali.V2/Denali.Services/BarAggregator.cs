using Alpaca.Markets;
using Denali.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Services
{
    public class BarAggregator
    {
        public event Action<IAggregateBar> OnBar;

        private readonly int _minuteInterval = 5;
        private readonly List<IBar> _intervalBars;
        private int _lastUpdateMinute = 0;

        public BarAggregator()
        {
            _intervalBars = new List<IBar>();
        }

        public void SetLastUpdateMinute(int minute) => _lastUpdateMinute = minute;

        public void OnMinuteBar(IBar bar)
        {
            _intervalBars.Add(bar);

            // If this is the last minute of the aggregate interval
            if (Round(bar.TimeUtc.Minute + 1) != _lastUpdateMinute)
            {
                Aggregate(bar.TimeUtc);
                _intervalBars.Clear();
            }
        }

        public void Aggregate(DateTime timeUtc)
        {
            _lastUpdateMinute = timeUtc.Minute + 1;
            var aggregatBar = new AggregateBar
            {
                Open = _intervalBars.First().Open,
                Close = _intervalBars.Last().Close,
                High = _intervalBars.Max(x => x.High),
                Low = _intervalBars.Min(x => x.Low),
                TimeUtc = timeUtc
            };

            OnBar.Invoke(aggregatBar);
        }

        public decimal Round(decimal value)
        {
            return Math.Floor(value / _minuteInterval) * _minuteInterval;
        }
    }
}
