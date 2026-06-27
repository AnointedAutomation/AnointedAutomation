// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//
// Central JSON naming convention for AnointedAutomation APIs. Moved out of each API's Program.cs into the
// shared open-source layer so every API enforces the SAME wire casing as the Mongo layer
// (AnointedAutomation.Repository.Mongo.HybridElementNameConvention) while C# code keeps idiomatic
// PascalCase property names.

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace AnointedAutomation.Objects.API
{
    /// <summary>
    /// Central JSON naming convention enforced on the global serializer (MVC controllers + minimal APIs):
    ///   class  -> reference-type members PascalCase, value-type members camelCase
    ///   struct -> every member camelCase
    ///   enum   -> camelCase string
    /// An explicit [JsonPropertyName] always wins (e.g. Shopify snake_case DTOs, RFC ProblemDetails), so
    /// types that deliberately set their own names are never touched. Anonymous types (hand-rolled
    /// 'new { ... }' payloads, e.g. snake_case cart/build and Node-parity webhook acks) are skipped so
    /// their literal member names stay verbatim. Types serialized through their own local
    /// JsonSerializerOptions (Shopify snake_case, CDN ratings feed, vision metafield) are unaffected.
    ///
    /// Wire it into an API with:
    ///   builder.Services.AddControllers().AddJsonOptions(o => JsonCasingConvention.Configure(o.JsonSerializerOptions));
    ///   builder.Services.ConfigureHttpJsonOptions(o => JsonCasingConvention.Configure(o.SerializerOptions));
    /// </summary>
    public static class JsonCasingConvention
    {
        /// <summary>
        /// A ready-to-use options instance carrying the convention, for code paths that serialize OUTSIDE the
        /// MVC / minimal-API pipeline (pre-serialized JsonElement payloads, file downloads, SSE frames) and
        /// therefore must apply the convention themselves. Reuse this single instance (System.Text.Json
        /// caches per-options metadata).
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

        public static void Configure(JsonSerializerOptions options)
        {
            // Enums -> camelCase strings (the string converter still reads integers on input, so request
            // bodies that send enum ints keep working).
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

            DefaultJsonTypeInfoResolver resolver =
                options.TypeInfoResolver as DefaultJsonTypeInfoResolver ?? new DefaultJsonTypeInfoResolver();
            resolver.Modifiers.Add(ApplyConvention);
            options.TypeInfoResolver = resolver;
        }

        private static void ApplyConvention(JsonTypeInfo typeInfo)
        {
            if (typeInfo.Kind != JsonTypeInfoKind.Object)
            {
                return;
            }

            // Anonymous types are hand-rolled wire payloads (snake_case cart/build, Node-parity acks);
            // their literal member names are intentional contracts, so never re-case them.
            if (IsAnonymousType(typeInfo.Type))
            {
                return;
            }

            // Enums never reach Object-kind; only structs land here as value types.
            bool isStruct = typeInfo.Type.IsValueType;
            foreach (JsonPropertyInfo property in typeInfo.Properties)
            {
                if (HasExplicitName(property))
                {
                    continue;
                }

                Type memberType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                // struct: every member camelCase. class: value-type members camelCase, ref-type members PascalCase.
                bool camel = isStruct || memberType.IsValueType;
                property.Name = camel ? ToCamel(property.Name) : ToPascal(property.Name);
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

        private static string ToCamel(string name) =>
            string.IsNullOrEmpty(name) ? name : JsonNamingPolicy.CamelCase.ConvertName(name);

        private static string ToPascal(string name) =>
            string.IsNullOrEmpty(name) || char.IsUpper(name[0])
                ? name
                : char.ToUpperInvariant(name[0]) + name.Substring(1);
    }
}
