using Alpaca.Markets;
using Denali.Models;
using Microsoft.Extensions.Logging;

namespace Denali.Services.Aggregators
{
    public class TradeAggregator : BaseAggregator
    {
        public IAggregateBar ProvisionalBar { get; private set; }
        public event Action<IAggregateBar> OnBarOpen;

        //private System.Threading.Timer timer;
        private readonly ILogger<TradeAggregator> _logger;

        public TradeAggregator(ILogger<TradeAggregator> logger)
        {
            _logger = logger;
            ProvisionalBar = new AggregateBar();
        }

        public void OnTrade(ITrade trade)
        {
            if (Round(trade.TimestampUtc.Minute) != _lastUpdateMinute)
            {
                SetLastUpdateMinute((int) Round(trade.TimestampUtc.Minute));
                ProvisionalBar = new AggregateBar
                {
                    Open = trade.Price,
                    High = trade.Price,
                    Low = trade.Price,
                    Close = trade.Price
                };
                OnBarOpen.Invoke(ProvisionalBar);
            }
            else
            {
                ProvisionalBar.Close = trade.Price;
                ProvisionalBar.High = Math.Max(ProvisionalBar.High, trade.Price);
                ProvisionalBar.Low = Math.Min(ProvisionalBar.Low, trade.Price);
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
