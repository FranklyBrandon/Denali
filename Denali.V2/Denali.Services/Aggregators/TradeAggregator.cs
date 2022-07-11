using Alpaca.Markets;
using Denali.Models;
using Microsoft.Extensions.Logging;

namespace Denali.Services.Aggregators
{
    public class TradeAggregator : BaseAggregator
    {
        public IAggregateBar CurrentBar { get; private set; }

        //private System.Threading.Timer timer;
        private readonly ILogger<TradeAggregator> _logger;

        public TradeAggregator(ILogger<TradeAggregator> logger)
        {
            _logger = logger;
            CurrentBar = new AggregateBar();
        }

        public void OnTrade(ITrade trade)
        {
            if (Round(trade.TimestampUtc.Minute) != _lastUpdateMinute)
            {
                SetLastUpdateMinute(trade.TimestampUtc.Minute);
                CurrentBar = new AggregateBar
                {
                    Open = trade.Price,
                    High = trade.Price,
                    Low = trade.Price,
                    Close = trade.Price
                };
            }
            else
            {
                CurrentBar.Close = trade.Price;
                CurrentBar.High = Math.Max(CurrentBar.High, trade.Price);
                CurrentBar.Low = Math.Min(CurrentBar.Low, trade.Price);
            }
        }


        /*
        public void StartTimer()
        {
            ScheduleTimer(new DateTime(2022, 7, 10, 18, 46, 0));
        }

        private void ScheduleTimer(DateTime alertTime)
        {
            DateTime current = DateTime.Now;
            TimeSpan timeToGo = alertTime.TimeOfDay - current.TimeOfDay;
            if (timeToGo < TimeSpan.Zero)
            {
                return;//time already passed
            }
            this.timer = new System.Threading.Timer(x =>
            {
                this.Bar(alertTime);
            }, null, timeToGo, Timeout.InfiniteTimeSpan);
        }

        private void Bar(DateTime alertTime)
        {
            _logger.LogInformation("Time evented");
            ScheduleTimer(alertTime.AddMinutes(1));
        }
        */
    }
}
