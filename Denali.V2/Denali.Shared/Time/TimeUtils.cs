﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Shared.Time
{
    public static class TimeUtils
    {
        private static TimeZoneInfo _newYorkTimeZone;

        static TimeUtils()
        {
            _newYorkTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        }

        public static DateTime GetNewYorkTime(DateTime universalTime) =>
            TimeZoneInfo.ConvertTime(universalTime, _newYorkTimeZone);

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp);
            return dateTime;
        }
    }
}
