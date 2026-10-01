// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

using System;
using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Apple
{
    /// <summary>
    /// Reads a JSON boolean that the provider may send EITHER as a real boolean or as the strings
    /// "true"/"false". Apple documents <c>email_verified</c>, <c>is_private_email</c> and
    /// <c>nonce_supported</c> this way (historically strings, newer tokens booleans). Writes a real boolean.
    /// </summary>
    public class StringOrBoolConverter : JsonConverter
    {
        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool) || objectType == typeof(bool?);
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Boolean:
                    return (bool)reader.Value;
                case JsonToken.String:
                    return Parse((string)reader.Value);
                case JsonToken.Null:
                    return null;
                default:
                    throw new JsonSerializationException("Expected a boolean or a \"true\"/\"false\" string, got " + reader.TokenType + ".");
            }
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteValue((bool)value);
        }

        /// <summary>
        /// Parses a claim value that may be "true"/"false" (any case). Returns null for null, empty or any
        /// other text, so an unexpected value is recorded as unknown rather than guessed.
        /// </summary>
        public static bool? Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (bool.TryParse(value.Trim(), out bool parsed))
            {
                return parsed;
            }

            return null;
        }
    }
}
