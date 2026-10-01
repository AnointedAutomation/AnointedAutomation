// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Copyright 2026 Anointed Automation, LLC. All Rights Reserved.
// Coded by Alexander Fields https://www.alexanderfields.me

using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;

namespace AnointedAutomation.Optimization
{
    /// <summary>
    /// Null-safe accessors over <see cref="JsonElement"/> that consolidate the dozens of
    /// per-service private helpers of the shape
    /// <c>TryGetProperty(prop, out el) &amp;&amp; el.ValueKind == ... ? el.Get...() : null</c>.
    /// Every getter first verifies the receiver is a JSON object (so calling on a non-object
    /// element never throws) and that the requested property exists with the expected value
    /// kind; otherwise it returns null / the supplied default. These match the dominant
    /// existing idiom exactly. Helpers that additionally coerce across kinds (parse numbers
    /// out of strings, treat "1"/"true" as booleans, fall back to GetRawText, etc.) have
    /// different semantics and are intentionally left in place.
    /// </summary>
    public static class JsonElementExtensions
    {
        /// <summary>Returns the string value of <paramref name="prop"/>, or null when the
        /// receiver is not an object, the property is missing, or it is not a JSON string.</summary>
        public static string? GetStringOrNull(this JsonElement element, string prop) =>
            element.ValueKind == JsonValueKind.Object
                && element.TryGetProperty(prop, out JsonElement v)
                && v.ValueKind == JsonValueKind.String
                ? v.GetString()
                : null;

        /// <summary>
        /// Like <see cref="GetStringOrNull"/> but also collapses a present-but-empty string to
        /// null. Callers that treat "" and "missing" as the same absent value need this variant;
        /// callers that must observe an empty string as an empty string must NOT use it. The two
        /// semantics existed side by side as private copies, so the difference is spelled out in
        /// the method names rather than left to the reader.
        /// </summary>
        public static string? GetNonEmptyStringOrNull(this JsonElement element, string prop)
        {
            string? s = element.GetStringOrNull(prop);
            return string.IsNullOrEmpty(s) ? null : s;
        }

        /// <summary>
        /// Like <see cref="GetStringOrNull"/> but yields <see cref="string.Empty"/> instead of
        /// null for a missing, non-object, or non-string property. Used where the value flows
        /// straight into a non-nullable field.
        /// </summary>
        public static string GetStringOrEmpty(this JsonElement element, string prop) =>
            element.GetStringOrNull(prop) ?? string.Empty;

        /// <summary>
        /// Like <see cref="GetStringOrNull"/> but ALSO coerces a JSON number to its invariant
        /// string form. Needed where an upstream API may send a field as either a quoted string
        /// or a bare number and the value is used as an identifier/key (e.g. a Zendrop
        /// order_number that feeds COGS externalId matching), so dropping the number would break
        /// the match. Booleans and other kinds still yield null.
        /// </summary>
        public static string? GetStringOrNumberStringOrNull(this JsonElement element, string prop)
        {
            if (element.ValueKind != JsonValueKind.Object
                || !element.TryGetProperty(prop, out JsonElement v))
            {
                return null;
            }

            if (v.ValueKind == JsonValueKind.String)
            {
                return v.GetString();
            }

            if (v.ValueKind == JsonValueKind.Number)
            {
                return v.GetRawText();
            }

            return null;
        }

        /// <summary>True when the receiver is an object and carries <paramref name="prop"/>.</summary>
        public static bool HasProperty(this JsonElement element, string prop) =>
            element.ValueKind == JsonValueKind.Object
            && element.TryGetProperty(prop, out JsonElement v)
            && v.ValueKind != JsonValueKind.Undefined;

        /// <summary>
        /// Returns the child element at <paramref name="prop"/>, or <c>default(JsonElement)</c>
        /// (ValueKind Undefined) when the receiver is not an object or the property is absent.
        /// The Undefined result is deliberately chainable: every getter here tolerates it.
        /// </summary>
        public static JsonElement GetElementOrDefault(this JsonElement element, string prop) =>
            element.ValueKind == JsonValueKind.Object && element.TryGetProperty(prop, out JsonElement v)
                ? v
                : default;

        /// <summary>Gets <paramref name="prop"/> when it is a JSON array; otherwise false with
        /// <paramref name="array"/> set to default.</summary>
        public static bool TryGetArray(this JsonElement element, string prop, out JsonElement array)
        {
            if (element.ValueKind == JsonValueKind.Object
                && element.TryGetProperty(prop, out JsonElement v)
                && v.ValueKind == JsonValueKind.Array)
            {
                array = v;
                return true;
            }
            array = default;
            return false;
        }

        /// <summary>
        /// Reads <paramref name="prop"/> as an array and returns only its JSON-string members.
        /// Non-string members are skipped rather than stringified, matching every private copy
        /// this replaced. Returns an empty list when the property is missing or not an array.
        /// </summary>
        public static List<string> GetStringArray(this JsonElement element, string prop)
        {
            List<string> result = new List<string>();
            if (element.TryGetArray(prop, out JsonElement arr))
            {
                foreach (JsonElement e in arr.EnumerateArray())
                {
                    if (e.ValueKind == JsonValueKind.String)
                    {
                        result.Add(e.GetString()!);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Reads <paramref name="prop"/> as a double, accepting a JSON number OR a numeric JSON
        /// string. Distinct from <see cref="GetDoubleOrNull"/>, which is strict about the kind.
        /// <paramref name="styles"/> is explicit because the copies this replaced disagreed:
        /// some allowed only plain floats, some allowed thousands separators and parentheses.
        /// </summary>
        public static double? GetDoubleCoercedOrNull(
            this JsonElement element, string prop, NumberStyles styles = NumberStyles.Float)
        {
            if (element.ValueKind != JsonValueKind.Object || !element.TryGetProperty(prop, out JsonElement v))
            {
                return null;
            }
            if (v.ValueKind == JsonValueKind.Number && v.TryGetDouble(out double d))
            {
                return d;
            }
            if (v.ValueKind == JsonValueKind.String
                && double.TryParse(v.GetString(), styles, CultureInfo.InvariantCulture, out double sd))
            {
                return sd;
            }
            return null;
        }

        /// <summary>Returns the double value of <paramref name="prop"/>, or null when the
        /// receiver is not an object, the property is missing, or it is not a JSON number.</summary>
        public static double? GetDoubleOrNull(this JsonElement element, string prop) =>
            element.ValueKind == JsonValueKind.Object
                && element.TryGetProperty(prop, out JsonElement v)
                && v.ValueKind == JsonValueKind.Number
                && v.TryGetDouble(out double d)
                ? d
                : (double?)null;

        /// <summary>Returns the int value of <paramref name="prop"/>, or null when the
        /// receiver is not an object, the property is missing, or it is not a JSON number
        /// representable as an <see cref="int"/>.</summary>
        public static int? GetIntOrNull(this JsonElement element, string prop) =>
            element.ValueKind == JsonValueKind.Object
                && element.TryGetProperty(prop, out JsonElement v)
                && v.ValueKind == JsonValueKind.Number
                && v.TryGetInt32(out int n)
                ? n
                : (int?)null;

        /// <summary>Returns the int value of <paramref name="prop"/>, or <paramref name="def"/> when the
        /// receiver is not an object, the property is missing, or it is not a JSON number
        /// representable as an <see cref="int"/>.</summary>
        public static int GetIntOrDefault(this JsonElement element, string prop, int def) =>
            element.GetIntOrNull(prop) ?? def;

        /// <summary>Returns the boolean value of <paramref name="prop"/> when it is a JSON
        /// true/false; otherwise returns <paramref name="def"/> (missing or wrong kind).</summary>
        public static bool GetBoolOrDefault(this JsonElement element, string prop, bool def = false)
        {
            if (element.ValueKind != JsonValueKind.Object
                || !element.TryGetProperty(prop, out JsonElement v))
            {
                return def;
            }
            return v.ValueKind switch
            {
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => def,
            };
        }

        /// <summary>
        /// Yields each node of a GraphQL connection <paramref name="field"/> on
        /// <paramref name="parent"/>, transparently handling both the classic
        /// <c>edges[].node</c> shape and the modern <c>nodes[]</c> shape. Yields nothing
        /// when the field is absent or is neither shape.
        /// </summary>
        public static IEnumerable<JsonElement> EnumerateConnection(this JsonElement parent, string field)
        {
            if (parent.ValueKind != JsonValueKind.Object
                || !parent.TryGetProperty(field, out JsonElement connection)
                || connection.ValueKind != JsonValueKind.Object)
            {
                yield break;
            }

            foreach (JsonElement node in EnumerateConnectionNodes(connection))
            {
                yield return node;
            }
        }

        /// <summary>
        /// Yields each node of a GraphQL connection element itself (already unwrapped from
        /// its parent field), handling both <c>edges[].node</c> and <c>nodes[]</c>.
        /// </summary>
        public static IEnumerable<JsonElement> EnumerateConnectionNodes(this JsonElement connection)
        {
            if (connection.ValueKind != JsonValueKind.Object)
            {
                yield break;
            }

            if (connection.TryGetProperty("nodes", out JsonElement nodes)
                && nodes.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement node in nodes.EnumerateArray())
                {
                    yield return node;
                }
                yield break;
            }

            if (connection.TryGetProperty("edges", out JsonElement edges)
                && edges.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement edge in edges.EnumerateArray())
                {
                    if (edge.ValueKind == JsonValueKind.Object
                        && edge.TryGetProperty("node", out JsonElement node))
                    {
                        yield return node;
                    }
                }
            }
        }
    }
}
