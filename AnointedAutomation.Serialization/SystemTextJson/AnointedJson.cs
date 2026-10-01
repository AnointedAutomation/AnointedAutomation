// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace AnointedAutomation.Serialization.SystemTextJson
{
    /// <summary>One-call configuration of an API host's serializer options.</summary>
    public static class AnointedJson
    {
        /// <summary>
        /// Applies the Anointed API wire config to <paramref name="options"/> so MVC and Minimal APIs are identical:
        /// no naming/dictionary-key policy (the casing convention decides), numbers readable from JSON strings,
        /// a FRESH <see cref="DefaultJsonTypeInfoResolver"/> (the process-wide default resolver goes immutable after
        /// first use and would reject the modifier in a second in-process host), then <see cref="JsonCasingConvention.Configure"/>.
        /// <code>
        /// builder.Services.AddControllers().AddJsonOptions(o => AnointedJson.ConfigureApi(o.JsonSerializerOptions));
        /// builder.Services.ConfigureHttpJsonOptions(o => AnointedJson.ConfigureApi(o.SerializerOptions));
        /// </code>
        /// </summary>
        public static void ConfigureApi(JsonSerializerOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.PropertyNamingPolicy = null;
            options.DictionaryKeyPolicy = null;
            options.NumberHandling = JsonNumberHandling.AllowReadingFromString;
            options.TypeInfoResolver = new DefaultJsonTypeInfoResolver();
            JsonCasingConvention.Configure(options);
        }
    }
}
