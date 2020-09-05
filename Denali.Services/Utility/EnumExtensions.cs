using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Linq;

namespace Denali.Services.Utility
{
    public static class EnumExtensions
    {
        public static string GetAttributeValue(this Enum enumValue)
        {
            var attribute = enumValue.GetType()
                                    .GetMember(enumValue.ToString())
                                    .First()
                                    .GetCustomAttribute<EnumMemberAttribute>(false);

            if (attribute != null && attribute.Value != null)
                return attribute.Value;

            return string.Empty;
        }
    }
}
