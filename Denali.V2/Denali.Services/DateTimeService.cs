using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Services
{
    public interface IDateTimeService
    {
        DateTime UtcNow();
    }
    public class DateTimeService : IDateTimeService
    {
        public DateTime UtcNow() => DateTime.UtcNow;
    }

    public class MockDateTimeService : IDateTimeService
    {
        private DateTime _time;

        public DateTime UtcNow() => _time;

        public void SetDateTime(DateTime time)
        {
            _time = time;
        }
    }
}
