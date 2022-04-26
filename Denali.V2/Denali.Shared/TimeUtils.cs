using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Shared
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
    }
}
