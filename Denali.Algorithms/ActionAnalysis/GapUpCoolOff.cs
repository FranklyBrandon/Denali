using Alpaca.Markets;
using Denali.Algorithms.AggregateAnalysis.EMA;
using Denali.Models.Shared;
using Denali.Services.Alpaca;
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
        private AlpacaTradingService _tradingService;

        public GapUpCoolOff(DateTime date, int hour, int minutes, string ticker, AlpacaTradingService tradingService)
        {
            _timeUtils = new TimeUtils();
            _ema = new EMA(9);
            _stockData = new List<IAggregateData>();
            _timeCutOff = _timeUtils.GetUnixSecondStamp(
                _timeUtils.GetEasternLocalTime(date, hour, minutes, seconds: 0));
            _ticker = ticker;
            _tradingService = tradingService;
        }

        public void SetInitialData(List<IAggregateData> data) => this._stockData = data;

        public void OnBarReceived(IAggregateData bar)
        {
            _stockData.Add(bar);
            _ema.Analyze(_stockData);

            if (bar.Time <= _timeCutOff )
                _openingHigh = Math.Max(_openingHigh, bar.HighPrice); 
            else
            {
                var hasOpenPosition = _tradingService.HasOpenPosition(_ticker);

                if (bar.ClosePrice > _openingHigh && !_breakoutTriggered)
                {
                    _breakoutTriggered = true;
                    Console.WriteLine($"Opening breakout detected for {_ticker} at {_timeUtils.GetETDatetimefromUnixS(bar.Time)}");
                    if (!hasOpenPosition)
                        _tradingService.EnterPosition(_ticker, 1, OrderType.Market, TimeInForce.Day);
                }
                else if (_breakoutTriggered && bar.ClosePrice < _ema.MovingAverages.Last())
                {
                    Console.WriteLine($"Reversion detected for {_ticker} at {_timeUtils.GetETDatetimefromUnixS(bar.Time)}");
                    if (hasOpenPosition)
                        _tradingService.ClosePosition(_ticker, OrderType.Market, TimeInForce.Day);
                }
            }
        }
    }
}
