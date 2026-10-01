// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// Shared, READ-ONLY System.Text.Json option sets. Each preset reproduces EXACTLY the settings of the hand-rolled
// static options it replaces (noted per member). Reuse the instance; to customize, copy it first:
//   JsonSerializerOptions mine = new JsonSerializerOptions(JsonPresets.CamelCase);

using System.Text.Json;
using System.Text.Json.Serialization;

namespace AnointedAutomation.Serialization.SystemTextJson
{
    /// <summary>Shared read-only <see cref="JsonSerializerOptions"/> presets.</summary>
    public static class JsonPresets
    {
        /// <summary>snake_case_lower + case-insensitive read. Shopify REST/webhook payloads.</summary>
        public static JsonSerializerOptions SnakeCase { get; } = Freeze(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true,
        });

        /// <summary><see cref="SnakeCase"/> plus numbers readable from JSON strings.</summary>
        public static JsonSerializerOptions SnakeCaseLenient { get; } = Freeze(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        });

        /// <summary>snake_case_lower, case-sensitive, nulls always written (<see cref="JsonIgnoreCondition.Never"/>). Storefront ratings/stats feeds.</summary>
        public static JsonSerializerOptions SnakeCaseWriteNulls { get; } = Freeze(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        });

        /// <summary>camelCase, case-sensitive read.</summary>
        public static JsonSerializerOptions CamelCase { get; } = Freeze(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });

        /// <summary>camelCase + case-insensitive read. Payment processor secrets blobs.</summary>
        public static JsonSerializerOptions CamelCaseInsensitive { get; } = Freeze(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
        });

        /// <summary>No naming policy (C# names verbatim) + case-insensitive read.</summary>
        public static JsonSerializerOptions CaseInsensitive { get; } = Freeze(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });

        /// <summary><see cref="CaseInsensitive"/> plus numbers readable from JSON strings.</summary>
        public static JsonSerializerOptions CaseInsensitiveLenient { get; } = Freeze(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        });

        /// <summary>C# names verbatim, nulls omitted on write.</summary>
        public static JsonSerializerOptions IgnoreNulls { get; } = Freeze(new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        });

        /// <summary>C# names verbatim, nulls omitted on write, reference cycles written as null.</summary>
        public static JsonSerializerOptions IgnoreNullsAndCycles { get; } = Freeze(new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        });

        /// <summary>The Anointed hybrid wire casing; the same instance as <see cref="JsonCasingConvention.Options"/>.</summary>
        public static JsonSerializerOptions Api => JsonCasingConvention.Options;

        private static JsonSerializerOptions Freeze(JsonSerializerOptions options)
        {
            options.MakeReadOnly(populateMissingResolver: true);
            return options;
        }
    }
}
