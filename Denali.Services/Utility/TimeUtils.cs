using Denali.Models;
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
            return TimeZoneInfo.ConvertTimeFromUtc(timeUtc, EasternStandardTime);
        }

        public TimeSpan GetNYSEOpen()
        {
            //var open = new DateTime(date.Year, date.Month, date.Day, 9, 30, 0);
            //return new DateTimeWithZone(open, EasternStandardTime);
            return new TimeSpan(9, 30, 0);
        }

        public TimeSpan GetNYSEClose()
        {
            //var open = new DateTime(date.Year, date.Month, date.Day, 16, 0, 0);
            //return new DateTimeWithZone(open, EasternStandardTime);
            return new TimeSpan(16, 0, 0);
        }

        public DateTimeWithZone GetNewYorkTimeFromEpoch(long seconds)
        {
            return new DateTimeWithZone(DateTimeOffset.FromUnixTimeSeconds(seconds), EasternStandardTime);
        }
    }
}
