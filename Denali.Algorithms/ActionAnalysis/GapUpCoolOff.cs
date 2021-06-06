using Denali.Algorithms.AggregateAnalysis.EMA;
using Denali.Models.Shared;
using Denali.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Algorithms.ActionAnalysis
{
    public class GapUpCoolOff
    {
        private string _ticker;
        private long _timeCutOff;
        private decimal _openingHigh;

        private readonly TimeUtils _timeUtils;
        private bool _breakoutTriggered = false;
        private bool _reverseTriggered = false;
        private EMA _ema;
        private List<IAggregateData> _stockData;

        public GapUpCoolOff(DateTime date, int hour, int minutes, string ticker)
        {
            _timeUtils = new TimeUtils();
            _ema = new EMA(9);
            _stockData = new List<IAggregateData>();
            _timeCutOff = _timeUtils.GetUnixSecondStamp(
                _timeUtils.GetEasternLocalTime(date, hour, minutes, seconds: 0));
            _ticker = ticker;
        }

        public void SetInitialData(List<IAggregateData> data) => this._stockData = data;

        public void OnBarReceived(IAggregateData bar)
        {
            _stockData.Add(bar);
            _ema.Analyze(_stockData);

            if (bar.Time <= _timeCutOff )
                _openingHigh = Math.Max(_openingHigh, bar.HighPrice); 
            else if (bar.ClosePrice > _openingHigh && !_breakoutTriggered)
            {
                _breakoutTriggered = true;
                Console.WriteLine($"Opening breakout detected for {_ticker} at {_timeUtils.GetETDatetimefromUnixS(bar.Time)}");
            }
            else if (_breakoutTriggered && !_reverseTriggered  && (bar.ClosePrice < _openingHigh))
            {
                _reverseTriggered = true;
                Console.WriteLine($"Reversion detected for {_ticker} at {_timeUtils.GetETDatetimefromUnixS(bar.Time)}");
            }
        }
    }
}
