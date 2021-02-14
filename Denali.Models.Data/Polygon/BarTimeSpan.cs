using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Denali.Models.Polygon
{
    public enum BarTimeSpan
    {
        [EnumMember(Value = "minute")]
        Minute,
        [EnumMember(Value = "hour")]
        Hour,
        [EnumMember(Value = "day")]
        Day,
        [EnumMember(Value = "week")]
        Week,
        [EnumMember(Value = "month")]
        Month,
        [EnumMember(Value = "quarter")]
        Quarter,
        [EnumMember(Value = "year")]
        Year
    }
}
