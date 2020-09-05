using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Services.Utility
{
    public class TimeUtils
    {
        public TimeZoneInfo EasternStandardTime { get; }
        public TimeUtils()
        {
            EasternStandardTime = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        }
        public DateTime GetNYSEDateTime()
        {
            var timeUtc = DateTime.UtcNow;
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);
        }

        public long GetNYSEOpen(DateTime date)
        {
            var open = new DateTime(date.Year, date.Month, date.Day, 9, 30, 0);
            var openTime = new DateTimeWithZone(open, EasternStandardTime);
            return ((DateTimeOffset)openTime.UniversalTime).ToUnixTimeSeconds();
        }

        public long GetNYSEClose(DateTime date)
        {
            var open = new DateTime(date.Year, date.Month, date.Day, 16, 0, 0);
            var openTime = new DateTimeWithZone(open, EasternStandardTime);
            return ((DateTimeOffset)openTime.UniversalTime).ToUnixTimeSeconds();
        }
    }
}
