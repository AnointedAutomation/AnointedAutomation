// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// One tolerant enum converter replacing ShopifySharp's NullableEnumConverter (null on unknown, [EnumMember]-aware)
// and Objects' AppleNotificationEventTypeConverter (unknown -> 0/Unknown). A new vendor value must never break parsing.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using global::Newtonsoft.Json;

namespace AnointedAutomation.Serialization.Newtonsoft
{
    /// <summary>
    /// Reads any enum (or nullable enum) leniently and never throws on an unrecognized value.
    /// Read order for a string token: exact (ordinal) <see cref="EnumMemberAttribute"/> value, then the member NAME
    /// ignoring case. An integer token maps when it is a defined value. Anything else (unknown text, a defined-name
    /// miss, null, other token kinds) is "unknown":
    ///   nullable enum (<c>T?</c>) -> null;
    ///   non-nullable enum -> <c>default(T)</c> (by convention 0 = Unknown).
    /// Writes the [EnumMember] value when present, otherwise the member name; a null nullable writes null.
    /// </summary>
    public class TolerantEnumConverter : JsonConverter
    {
        private static readonly ConcurrentDictionary<Type, EnumMap> Maps = new ConcurrentDictionary<Type, EnumMap>();

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            Type underlying = Nullable.GetUnderlyingType(objectType) ?? objectType;
            return underlying.IsEnum;
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Type nullableUnderlying = Nullable.GetUnderlyingType(objectType);
            bool isNullable = nullableUnderlying != null;
            Type enumType = nullableUnderlying ?? objectType;

            object parsed = TryRead(reader, enumType);
            if (parsed != null)
            {
                return parsed;
            }

            return isNullable ? null : Activator.CreateInstance(enumType);
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteValue(ToWire((Enum)value));
        }

        /// <summary>The value parsed from <paramref name="text"/> per the read rules, or null when unknown.</summary>
        public static object Parse(Type enumType, string text)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException(nameof(enumType));
            }

            if (text == null)
            {
                return null;
            }

            EnumMap map = Maps.GetOrAdd(enumType, BuildMap);
            if (map.ByMember.TryGetValue(text, out object byMember))
            {
                return byMember;
            }

            if (map.ByName.TryGetValue(text, out object byName))
            {
                return byName;
            }

            return null;
        }

        /// <summary>Typed <see cref="Parse(Type, string)"/>: the value, or null when unknown.</summary>
        public static T? Parse<T>(string text) where T : struct, Enum
        {
            object parsed = Parse(typeof(T), text);
            return parsed == null ? (T?)null : (T)parsed;
        }

        /// <summary>The wire string for <paramref name="value"/>: its [EnumMember] value when present, otherwise its name.</summary>
        public static string ToWire(Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            EnumMap map = Maps.GetOrAdd(value.GetType(), BuildMap);
            if (map.ToWire.TryGetValue(value, out string wire))
            {
                return wire;
            }

            return value.ToString();
        }

        private static object TryRead(JsonReader reader, Type enumType)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                    return Parse(enumType, (string)reader.Value);
                case JsonToken.Integer:
                    object number = Enum.ToObject(enumType, Convert.ToInt64(reader.Value, System.Globalization.CultureInfo.InvariantCulture));
                    return Enum.IsDefined(enumType, number) ? number : null;
                default:
                    return null;
            }
        }

        private static EnumMap BuildMap(Type enumType)
        {
            EnumMap map = new EnumMap();
            foreach (FieldInfo field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                object value = field.GetValue(null);
                EnumMemberAttribute member = field.GetCustomAttribute<EnumMemberAttribute>();
                if (member != null && member.Value != null)
                {
                    map.ByMember.TryAdd(member.Value, value);
                    map.ToWire.TryAdd(value, member.Value);
                }

                map.ByName.TryAdd(field.Name, value);
            }

            return map;
        }

        private sealed class EnumMap
        {
            public Dictionary<string, object> ByMember { get; } = new Dictionary<string, object>(StringComparer.Ordinal);

            public Dictionary<string, object> ByName { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            public Dictionary<object, string> ToWire { get; } = new Dictionary<object, string>();
        }
    }
}
