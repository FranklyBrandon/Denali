using Alpaca.Markets;
using Denali.Models;
using Microsoft.Extensions.Logging;

namespace Denali.Services
{
    public class TradeAggregator
    {
        public int Minutes { get; set; }
        public List<ITrade> Trades { get; set; }
        public List<IAggregateBar> Bars { get; set; }
        public int lastMinuteUpdate = 0;
        private System.Threading.Timer timer;
        private readonly ILogger<TradeAggregator> _logger;

        public TradeAggregator(ILogger<TradeAggregator> logger)
        {
            _logger = logger;
            Trades = new List<ITrade>();
            Bars = new List<IAggregateBar>();
        }

        public void OnTrade(ITrade trade)
        {
            Trades.Add(trade);
            if (Round(trade.TimestampUtc.Minute) != lastMinuteUpdate)
            {
                lastMinuteUpdate = trade.TimestampUtc.Minute;
            }

        }

        public decimal Round(decimal value)
        {
            return Math.Floor(value / Minutes) * Minutes;
        }

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

        /*
        private System.Threading.Timer timer;
        private void SetUpTimer(TimeSpan alertTime)
        {
            DateTime current = DateTime.Now;
            TimeSpan timeToGo = alertTime - current.TimeOfDay;
            if (timeToGo < TimeSpan.Zero)
            {
                return;//time already passed
            }
            this.timer = new System.Threading.Timer(x =>
            {
                this.SomeMethodRunsAt1600();
            }, null, timeToGo, Timeout.InfiniteTimeSpan);
        }

        private void SomeMethodRunsAt1600()
        {
            //this runs at 16:00:00
        }
        */
    }
}
