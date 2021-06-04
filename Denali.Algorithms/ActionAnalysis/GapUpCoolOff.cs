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
        private long _timeCutOff;
        private decimal _openingHigh;

        private readonly TimeUtils _timeUtils;
        private List<IAggregateData> _stockData;

        public GapUpCoolOff(int hour, int minutes)
        {
            _timeUtils = new TimeUtils();
            _stockData = new List<IAggregateData>();
            _timeCutOff = _timeUtils.GetUnixSecondStamp(
                _timeUtils.GetEasternLocalTime(DateTime.UtcNow, hour, minutes, seconds: 0));
        }

        public void SetInitialData(List<IAggregateData> data) => this._stockData = data;

        public void OnBarReceived(IAggregateData bar)
        {
            _stockData.Add(bar);

            if (bar.Time <= _timeCutOff )
                _openingHigh = Math.Max(_openingHigh, bar.HighPrice); 
            else if (bar.ClosePrice > _openingHigh)
                Console.WriteLine($"Opening breakout detected for {bar.Symbol} at {_timeUtils.GetETDatetimefromUnixS(bar.Time)}");
        }
    }
}
