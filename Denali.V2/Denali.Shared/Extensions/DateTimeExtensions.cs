using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Shared.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime Truncate(this DateTime dateTime, TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.Zero) throw new ArgumentException("Must use a non-zero timespan");
            if (dateTime == DateTime.MinValue || dateTime == DateTime.MaxValue) throw new ArgumentException("Do not use min or max datetime");

            return dateTime.AddTicks(-(dateTime.Ticks % timeSpan.Ticks));

            // dateTime.Truncate(TimeSpan.FromMilliseconds(1)); // Truncate to whole ms
            // dateTime.Truncate(TimeSpan.FromSeconds(1)); // Truncate to whole second
            // dateTime.Truncate(TimeSpan.FromMinutes(1)); // Truncate to whole minute
        }
    }
}
