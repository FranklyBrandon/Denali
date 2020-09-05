using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Denali.Models.Data.FinnHub
{
    public enum CandleResolution
    {
        [EnumMember(Value = "1")]
        One,
        [EnumMember(Value = "5")]
        Five,
        [EnumMember(Value = "15")]
        Fifteen,
        [EnumMember(Value = "30")]
        Thirty,
        [EnumMember(Value = "60")]
        Sixty,
        [EnumMember(Value = "D")]
        Day,
        [EnumMember(Value = "W")]
        Week,
        [EnumMember(Value = "M")]
        Month
    }
}
