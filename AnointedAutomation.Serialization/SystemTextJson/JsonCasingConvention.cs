// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// Central JSON naming convention for AnointedAutomation APIs. The Mongo layer
// (AnointedAutomation.Repository.Mongo.HybridElementNameConvention) applies the SAME rule via NamingRules.

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using AnointedAutomation.Serialization.Naming;

namespace AnointedAutomation.Serialization.SystemTextJson
{
    /// <summary>
    /// Central JSON naming convention:
    ///   class  -> reference-type members PascalCase, value-type members camelCase
    ///   struct -> every member camelCase
    ///   enum   -> camelCase string
    /// An explicit [JsonPropertyName] always wins. Anonymous types are skipped so their literal member names stay verbatim.
    /// For a whole API pipeline prefer <see cref="AnointedJson.ConfigureApi"/>.
    /// </summary>
    public static class JsonCasingConvention
    {
        /// <summary>
        /// A ready-to-use options instance carrying the convention, for code paths that serialize OUTSIDE the
        /// MVC / minimal-API pipeline. Reuse this single instance (System.Text.Json caches per-options metadata).
        /// </summary>
        public static JsonSerializerOptions Options { get; } = CreateOptions();

        private static JsonSerializerOptions CreateOptions()
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                DictionaryKeyPolicy = null,
            };
            Configure(options);
            return options;
        }

        /// <summary>
        /// Adds the camelCase enum string converter, the UTC DateTime converter and the casing modifier to
        /// <paramref name="options"/>. Reuses an existing <see cref="DefaultJsonTypeInfoResolver"/> when present.
        /// </summary>
        public static void Configure(JsonSerializerOptions options)
        {
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            options.Converters.Add(new UtcDateTimeJsonConverter());

            DefaultJsonTypeInfoResolver resolver =
                options.TypeInfoResolver as DefaultJsonTypeInfoResolver ?? new DefaultJsonTypeInfoResolver();
            resolver.Modifiers.Add(ApplyConvention);
            options.TypeInfoResolver = resolver;
        }

        /// <summary>The type-info modifier that applies the hybrid rule. Public so callers composing their own resolver can reuse it.</summary>
        public static void ApplyConvention(JsonTypeInfo typeInfo)
        {
            if (typeInfo.Kind != JsonTypeInfoKind.Object)
            {
                return;
            }

            if (IsAnonymousType(typeInfo.Type))
            {
                return;
            }

            foreach (JsonPropertyInfo property in typeInfo.Properties)
            {
                if (HasExplicitName(property))
                {
                    continue;
                }

                property.Name = NamingRules.ToHybrid(property.Name, typeInfo.Type, property.PropertyType, CamelStyle.SystemTextJson);
            }
        }

        private static bool HasExplicitName(JsonPropertyInfo property)
        {
            ICustomAttributeProvider provider = property.AttributeProvider;
            return provider != null
                && provider.GetCustomAttributes(typeof(JsonPropertyNameAttribute), inherit: true).Length > 0;
        }

        private static bool IsAnonymousType(Type type) =>
            type.IsGenericType
            && type.Name.Contains("AnonymousType", StringComparison.Ordinal)
            && type.GetCustomAttribute<CompilerGeneratedAttribute>() != null;
    }
}
