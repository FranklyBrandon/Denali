using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Shared.Utility
{
    public class TimeUtils
    {
        public TimeZoneInfo EasternStandardTime { get; }
        public TimeUtils()
        {
            EasternStandardTime = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        }

        public long GetNYSEOpenUnixMS(DateTime date)
        {
            var localDateTime = date.Add(new TimeSpan(9, 30, 0));
            var zonedDateTime = new DateTimeWithZone(localDateTime, EasternStandardTime);

            return ((DateTimeOffset)zonedDateTime.UniversalTime).ToUnixTimeMilliseconds();
        }

        public DateTime GetNYSEOpenDateTime(DateTime date)
        {
            var localDateTime = date.Add(new TimeSpan(9, 30, 0));
            var zonedDateTime = new DateTimeWithZone(localDateTime, EasternStandardTime);

            return zonedDateTime.UniversalTime;
        }

        public long GetNYSECloseUnixMS(DateTime date)
        {
            var localDateTime = date.Add(new TimeSpan(16, 0, 0));
            var zonedDateTime = new DateTimeWithZone(localDateTime, EasternStandardTime);

            return ((DateTimeOffset)zonedDateTime.UniversalTime).ToUnixTimeMilliseconds();
        }

        public DateTime GetETDatetimefromUnixMS(long timestamp)
        {
            var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, EasternStandardTime);
        }
    }
}
