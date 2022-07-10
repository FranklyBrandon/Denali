using Alpaca.Markets;
using Denali.Models;

namespace Denali.Services
{
    public class TradeAggregator
    {
        public int Minutes { get; set; }
        public List<ITrade> Trades { get; set; }
        public List<IAggregateBar> Bars { get; set; }
        public int lastMinuteUpdate = 0;

        public TradeAggregator(int minutes, int lastMinuteUpdate)
        {
            Minutes = minutes;
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
