// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// BACK-COMPAT SHIM. The convention moved to AnointedAutomation.Serialization.SystemTextJson.JsonCasingConvention
// (package AnointedAutomation.Serialization). This type keeps existing consumers compiling unchanged and simply
// delegates. It is deliberately NOT marked [Obsolete]: consumers build with TreatWarningsAsErrors, so an
// obsolete warning would break them on a minor version bump. New code should use the Serialization type.

using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace AnointedAutomation.Objects.API
{
    /// <summary>
    /// Delegates to <see cref="AnointedAutomation.Serialization.SystemTextJson.JsonCasingConvention"/>.
    /// Prefer <c>AnointedAutomation.Serialization.SystemTextJson.JsonCasingConvention</c> (or
    /// <c>AnointedJson.ConfigureApi</c>) in new code.
    /// </summary>
    public static class JsonCasingConvention
    {
        /// <summary>The shared options instance (same instance as the Serialization package's).</summary>
        public static JsonSerializerOptions Options =>
            AnointedAutomation.Serialization.SystemTextJson.JsonCasingConvention.Options;

        /// <summary>Applies the hybrid casing convention to <paramref name="options"/>.</summary>
        public static void Configure(JsonSerializerOptions options) =>
            AnointedAutomation.Serialization.SystemTextJson.JsonCasingConvention.Configure(options);

        /// <summary>The casing modifier, exposed for callers composing their own resolver.</summary>
        public static void ApplyConvention(JsonTypeInfo typeInfo) =>
            AnointedAutomation.Serialization.SystemTextJson.JsonCasingConvention.ApplyConvention(typeInfo);
    }
}
