namespace Denali.Shared.Time
{
    public class ScheduledTask
    {
        private Timer _timer;
        private Func<Task> _alertAction;
        public ScheduledTask(DateTime utcAlertTime, Func<Task> alertAction)
        {
            _alertAction = alertAction;
            DateTime current = DateTime.UtcNow;
            TimeSpan timeToGo = utcAlertTime.TimeOfDay - current.TimeOfDay;

            if (timeToGo < TimeSpan.Zero)
                return; // Time has already passed

            _timer = new Timer(async x =>
            {
                await alertAction.Invoke();
            }, null, timeToGo, Timeout.InfiniteTimeSpan);         
        }

        public async Task InvokeManual()
        {
            if (_timer != null)
                _timer.Dispose();
            await _alertAction.Invoke();
        }
    }
}
