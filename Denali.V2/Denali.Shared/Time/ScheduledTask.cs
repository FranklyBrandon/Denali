namespace Denali.Shared.Time
{
    public class ScheduledTask
    {
        private Timer _timer;
        public ScheduledTask(DateTime utcAlertTime, Action<DateTime> alertAction)
        {
            DateTime current = DateTime.UtcNow;
            TimeSpan timeToGo = utcAlertTime.TimeOfDay - current.TimeOfDay;

            if (timeToGo < TimeSpan.Zero)
                return; // Time has already passed

            _timer = new Timer(x =>
            {
                alertAction.Invoke(utcAlertTime);
            }, null, timeToGo, Timeout.InfiniteTimeSpan);         
        }
    }
}
