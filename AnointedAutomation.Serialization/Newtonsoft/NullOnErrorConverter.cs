// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. Moved from AnointedAutomation.Shopify.Converters (2026-09-30) and merged with
// ShopifySharp's InvalidDateConverter.

using System;
using global::Newtonsoft.Json;

namespace AnointedAutomation.Serialization.Newtonsoft
{
    /// <summary>
    /// Read-only converter that yields null instead of throwing when a value cannot be deserialized.
    /// Also covers the Shopify "invalid date" bug (values like <c>0000-12-31T18:09:24-05:50</c>, below DateTime.MinValue,
    /// seen on fulfillment, transaction and customer timestamps): any string starting with <c>0000-</c> read into a
    /// DateTime/DateTimeOffset (nullable or not) returns null without attempting a parse.
    /// Apply to nullable or reference-typed members only; a non-nullable value type receives its default when null is returned.
    /// </summary>
    public class NullOnErrorConverter : JsonConverter
    {
        /// <inheritdoc />
        public override bool CanWrite => false;

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (IsInvalidDate(reader, objectType))
            {
                return null;
            }

            try
            {
                return serializer.Deserialize(reader, objectType);
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException($"Unnecessary because {nameof(CanWrite)} is false.");
        }

        private static bool IsInvalidDate(JsonReader reader, Type objectType)
        {
            Type underlying = Nullable.GetUnderlyingType(objectType) ?? objectType;
            if (underlying != typeof(DateTime) && underlying != typeof(DateTimeOffset))
            {
                return false;
            }

            return reader.TokenType == JsonToken.String
                && reader.Value is string text
                && text.StartsWith("0000-", StringComparison.Ordinal);
        }
    }
}
