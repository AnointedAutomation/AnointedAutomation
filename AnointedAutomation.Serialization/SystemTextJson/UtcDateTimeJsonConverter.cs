// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// Every DateTime on the Anointed wire is UTC. System.Text.Json only appends the 'Z' designator when a value's
// Kind is Utc, so an Unspecified value goes out WITHOUT it and browsers read it as local time. This converter
// normalizes on write so the designator is always present. Reading is unchanged.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AnointedAutomation.Serialization.SystemTextJson
{
    /// <summary>
    /// Writes every <see cref="DateTime"/> as UTC ISO-8601 ending in 'Z':
    ///   Utc -> as-is, Unspecified -> treated as UTC, Local -> converted to UTC.
    /// Reading keeps System.Text.Json's default parsing.
    /// </summary>
    public sealed class UtcDateTimeJsonConverter : JsonConverter<DateTime>
    {
        /// <inheritdoc />
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            reader.GetDateTime();

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
            writer.WriteStringValue(ToUtc(value));

        /// <summary>Normalizes a DateTime to Kind Utc using the platform rule (Unspecified means UTC).</summary>
        public static DateTime ToUtc(DateTime value) => value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc),
        };
    }
}
