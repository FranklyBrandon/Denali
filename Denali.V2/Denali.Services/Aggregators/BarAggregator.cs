using Alpaca.Markets;
using Denali.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Services.Aggregators
{
    public class BarAggregator : BaseAggregator
    {
        public event Action<IAggregateBar> OnBar;
        private readonly List<IBar> _intervalBars;

        public BarAggregator()
        {
            _intervalBars = new List<IBar>();
        }

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
            var minute = (int)Round(timeUtc.Minute + 1);
            _lastUpdateMinute = minute == 60 ? 0 : minute;
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
    }
}
