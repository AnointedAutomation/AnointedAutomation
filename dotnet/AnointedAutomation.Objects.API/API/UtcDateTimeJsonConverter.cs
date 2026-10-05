// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// BACK-COMPAT SHIM. Moved to AnointedAutomation.Serialization.SystemTextJson.UtcDateTimeJsonConverter. That type is
// sealed, so this one cannot derive from it; it delegates instead. Not [Obsolete] (consumers build with
// TreatWarningsAsErrors). New code should use the Serialization type.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AnointedAutomation.Objects.API
{
    /// <summary>
    /// Delegates to <see cref="AnointedAutomation.Serialization.SystemTextJson.UtcDateTimeJsonConverter"/>: writes every
    /// DateTime as UTC ISO-8601 ending in 'Z' (Unspecified treated as UTC, Local converted). Reading is unchanged.
    /// </summary>
    public sealed class UtcDateTimeJsonConverter : JsonConverter<DateTime>
    {
        private static readonly AnointedAutomation.Serialization.SystemTextJson.UtcDateTimeJsonConverter Inner =
            new AnointedAutomation.Serialization.SystemTextJson.UtcDateTimeJsonConverter();

        /// <inheritdoc />
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            Inner.Read(ref reader, typeToConvert, options);

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
            Inner.Write(writer, value, options);

        /// <summary>Normalizes a DateTime to Kind Utc using the platform rule (Unspecified means UTC).</summary>
        public static DateTime ToUtc(DateTime value) =>
            AnointedAutomation.Serialization.SystemTextJson.UtcDateTimeJsonConverter.ToUtc(value);
    }
}
