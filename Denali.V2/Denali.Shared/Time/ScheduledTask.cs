namespace Denali.Shared.Time
{
    public class ScheduledTask
    {
        public ScheduledTask(DateTime utcAlertTime, Action<DateTime> alertAction)
        {
            DateTime current = DateTime.UtcNow;
            TimeSpan timeToGo = utcAlertTime.TimeOfDay - current.TimeOfDay;

            if (timeToGo < TimeSpan.Zero)
                return; // Time has already passed

            new Timer(x =>
            {
                alertAction.Invoke(utcAlertTime);
            }, null, timeToGo, Timeout.InfiniteTimeSpan);         
        }
    }
}
