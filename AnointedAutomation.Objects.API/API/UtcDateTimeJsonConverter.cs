// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//
// Every DateTime on the Anointed wire is UTC. System.Text.Json only appends the 'Z' designator when a value's
// Kind is Utc, so an Unspecified value (the default for 'new DateTime(...)', DateTime.Parse, and many
// arithmetic results) goes out WITHOUT it and browsers then read it as the viewer's local time. This converter
// normalizes on write so the designator is always present. Reading is unchanged.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AnointedAutomation.Objects.API
{
    /// <summary>
    /// Writes every <see cref="DateTime"/> as UTC ISO-8601 ending in 'Z':
    ///   Utc         -> written as-is
    ///   Unspecified -> treated as UTC (the platform rule: every stored DateTime is UTC)
    ///   Local       -> converted to UTC
    /// Output format matches System.Text.Json's own Utc formatting (fractional seconds trimmed), so values that
    /// already carried 'Z' serialize byte-for-byte the same. Nullable DateTime is covered automatically.
    /// Reading keeps System.Text.Json's default parsing. Property names and casing are never touched.
    /// </summary>
    public sealed class UtcDateTimeJsonConverter : JsonConverter<DateTime>
    {
        /// <inheritdoc />
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            reader.GetDateTime();

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
            writer.WriteStringValue(ToUtc(value));

        /// <summary>
        /// Normalizes a DateTime to Kind Utc using the platform rule (Unspecified means UTC).
        /// </summary>
        public static DateTime ToUtc(DateTime value) => value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc),
        };
    }
}
