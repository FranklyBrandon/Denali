namespace Denali.Shared.Time
{
    public class ScheduledTask
    {
        private Timer _timer;
        private Func<DateTime, Task> _alertAction;
        private DateTime _alertTime;
        public ScheduledTask(DateTime utcAlertTime, Func<DateTime, Task> alertAction)
        {
            _alertTime = utcAlertTime;
            _alertAction = alertAction;
            DateTime current = DateTime.UtcNow;
            TimeSpan timeToGo = utcAlertTime.TimeOfDay - current.TimeOfDay;

            if (timeToGo < TimeSpan.Zero)
                return; // Time has already passed

            _timer = new Timer(async x =>
            {
                await alertAction.Invoke(utcAlertTime);
            }, null, timeToGo, Timeout.InfiniteTimeSpan);         
        }

        public void InvokeManual()
        {
            if (_timer != null)
                _timer.Dispose();
            _alertAction.Invoke(_alertTime);
        }
    }
}
