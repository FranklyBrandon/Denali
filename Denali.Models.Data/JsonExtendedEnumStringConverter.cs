using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Denali.Models
{
    public class JsonExtendedEnumStringConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return (Nullable.GetUnderlyingType(typeToConvert)?.IsEnum ?? false) || typeToConvert.IsEnum;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return (JsonConverter)Activator.CreateInstance(typeof(JsonExtendedEnumStringConverterInner<>).MakeGenericType(typeToConvert),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: null,
                culture: null
            );
        }

        private class JsonExtendedEnumStringConverterInner<T> : JsonConverter<T>
        {
            public JsonExtendedEnumStringConverterInner() { }

            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var tokenType = reader.TokenType;

                switch (tokenType)
                {
                    case JsonTokenType.Number:
                        return (T)Enum.ToObject(typeof(T), reader.GetInt32());
                    case JsonTokenType.String:
                        return ParseEnumString(reader.GetString(), typeToConvert);
                    case JsonTokenType.Null:
                        if (Nullable.GetUnderlyingType(typeToConvert)!= null) return default;
                        throw new JsonException("Unable to parse the value as it was null and a null was not expected");
                    default:
                        throw new JsonException("Unable to parse the value of the enum as the token type was not a number, a string, or null in the case where type is nullable.");
                }
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                if (value == null)
                    JsonSerializer.Serialize(writer, null, options);
                else
                {
                    var enumType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
                    var enumValueName = Enum.GetName(enumType, value);
                    var enumField = enumType.GetField(enumValueName);

                    if (enumField != null)
                    {
                        var enumMemberAttribute = enumField.GetCustomAttribute<EnumMemberAttribute>();
                        var displayAttribute = enumField.GetCustomAttribute<DisplayAttribute>();

                        if (enumMemberAttribute != null)
                        {
                            JsonSerializer.Serialize(writer, enumMemberAttribute.Value, options);
                        }
                        else if (displayAttribute != null && displayAttribute.Name != null)
                        {
                            JsonSerializer.Serialize(writer, displayAttribute.Name, options);
                        }
                        else
                        {
                            JsonSerializer.Serialize(writer, enumValueName, options);
                        }
                    }
                    else
                    {
                        throw new JsonException($"Unable to find a suitable string to represent the enum of type {enumType.FullName}.");
                    }
                }
            }

            private static T ParseEnumString(string value, Type typeToConvert)
            {
                var enumType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
                var enumValues = Enum.GetValues(enumType);

                if (value != null)
                {
                    return enumValues.Cast<T>().Where(enumValue =>
                    {
                        var enumStringValue = enumValue.ToString();
                        var enumField = enumType.GetField(enumValue.ToString());
                        var memberAttribute = enumField?.GetCustomAttribute<EnumMemberAttribute>();
                        var displayAttribute = enumField?.GetCustomAttribute<DisplayAttribute>();

                        return memberAttribute?.Value == value
                               || displayAttribute?.Name == value
                               || enumStringValue == value;

                    }).FirstOrDefault() ?? throw new JsonException("Value read in did not match any known string values for enum");
                }

                throw new JsonException("Value read was null when null was not expected");
            }
        }
    }
}
