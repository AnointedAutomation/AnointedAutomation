// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// Lenient, never-throwing readers over System.Text.Json JsonElement. Moved from the Anointed API
// (Services/Common/JsonElementExtensions.cs) and widened into a SUPERSET so every hand-rolled private reader in the
// API maps onto exactly one method here with identical semantics. Where two readers looked alike but differed (trim,
// empty-string handling, raw text vs ToString, culture), they are separate, explicitly named methods.
//
// Rules shared by EVERY property reader (prop-level methods):
//   * A receiver that is not a JSON object (including default/Undefined) is treated exactly like a missing property:
//     the miss value is returned and nothing throws. That makes every reader chainable through GetElementOrDefault.
//   * Number parsing of strings is culture-INVARIANT.
//   * A JSON null never becomes the text "null".

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;

namespace AnointedAutomation.Serialization.Json
{
    /// <summary>Null-safe, non-throwing accessors over <see cref="JsonElement"/>.</summary>
    public static class JsonElementExtensions
    {
        // ------------------------------------------------------------------ core

        /// <summary>
        /// Gets property <paramref name="prop"/> when the receiver is an object and carries it (any kind, including
        /// JSON null). False with <paramref name="value"/> = default otherwise. Never throws.
        /// </summary>
        public static bool TryGetPropertySafe(this JsonElement element, string prop, out JsonElement value)
        {
            if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty(prop, out value))
            {
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Like <see cref="TryGetPropertySafe"/> but matches the property name ignoring case (ordinal); the FIRST matching
        /// property in document order wins. For payloads whose casing is not fixed (e.g. hybrid camel/Pascal wire).
        /// </summary>
        public static bool TryGetPropertyIgnoreCase(this JsonElement element, string prop, out JsonElement value)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (JsonProperty property in element.EnumerateObject())
                {
                    if (property.Name.Equals(prop, StringComparison.OrdinalIgnoreCase))
                    {
                        value = property.Value;
                        return true;
                    }
                }
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Gets property <paramref name="prop"/> when present AND not JSON null. False with default otherwise.
        /// </summary>
        public static bool TryGetNonNull(this JsonElement element, string prop, out JsonElement value)
        {
            if (element.ValueKind == JsonValueKind.Object
                && element.TryGetProperty(prop, out value)
                && value.ValueKind != JsonValueKind.Null)
            {
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>True when the receiver is an object and carries <paramref name="prop"/> (a present JSON null counts).</summary>
        public static bool HasProperty(this JsonElement element, string prop) =>
            element.ValueKind == JsonValueKind.Object
            && element.TryGetProperty(prop, out JsonElement v)
            && v.ValueKind != JsonValueKind.Undefined;

        /// <summary>
        /// The child at <paramref name="prop"/> (any kind), or default (Undefined) when the receiver is not an object
        /// or the property is absent. Chainable: every reader here tolerates Undefined.
        /// </summary>
        public static JsonElement GetElementOrDefault(this JsonElement element, string prop) =>
            element.ValueKind == JsonValueKind.Object && element.TryGetProperty(prop, out JsonElement v)
                ? v
                : default;

        /// <summary>Walks <paramref name="path"/> with <see cref="GetElementOrDefault"/>; default on any miss.</summary>
        public static JsonElement GetPathOrDefault(this JsonElement element, params string[] path)
        {
            JsonElement current = element;
            if (path == null)
            {
                return current;
            }

            foreach (string segment in path)
            {
                current = current.GetElementOrDefault(segment);
            }

            return current;
        }

        /// <summary>Gets <paramref name="prop"/> when it is a JSON array; otherwise false with default.</summary>
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

        /// <summary>The JSON-string members of array <paramref name="prop"/>; non-strings skipped; empty list on miss.</summary>
        public static List<string> GetStringArray(this JsonElement element, string prop)
        {
            List<string> result = new List<string>();
            if (element.TryGetArray(prop, out JsonElement arr))
            {
                foreach (JsonElement e in arr.EnumerateArray())
                {
                    if (e.ValueKind == JsonValueKind.String)
                    {
                        result.Add(e.GetString());
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Each node of GraphQL connection <paramref name="field"/> on <paramref name="parent"/>: <c>nodes[]</c> preferred,
        /// else <c>edges[].node</c>. Nothing when absent or neither shape.
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

        /// <summary>Each node of a connection element itself (<c>nodes[]</c> preferred, else <c>edges[].node</c>).</summary>
        public static IEnumerable<JsonElement> EnumerateConnectionNodes(this JsonElement connection)
        {
            if (connection.ValueKind != JsonValueKind.Object)
            {
                yield break;
            }

            if (connection.TryGetProperty("nodes", out JsonElement nodes) && nodes.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement node in nodes.EnumerateArray())
                {
                    yield return node;
                }

                yield break;
            }

            if (connection.TryGetProperty("edges", out JsonElement edges) && edges.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement edge in edges.EnumerateArray())
                {
                    if (edge.ValueKind == JsonValueKind.Object && edge.TryGetProperty("node", out JsonElement node))
                    {
                        yield return node;
                    }
                }
            }
        }

        // ------------------------------------------------------------------ strings (property level)

        /// <summary>The value when <paramref name="prop"/> is a JSON string (untrimmed, "" kept); otherwise null.</summary>
        public static string GetStringOrNull(this JsonElement element, string prop) =>
            element.ValueKind == JsonValueKind.Object
                && element.TryGetProperty(prop, out JsonElement v)
                && v.ValueKind == JsonValueKind.String
                ? v.GetString()
                : null;

        /// <summary><see cref="GetStringOrNull"/> with "" collapsed to null. Whitespace-only is KEPT (no trim).</summary>
        public static string GetNonEmptyStringOrNull(this JsonElement element, string prop)
        {
            string s = element.GetStringOrNull(prop);
            return string.IsNullOrEmpty(s) ? null : s;
        }

        /// <summary><see cref="GetStringOrNull"/> with null/empty/whitespace-only collapsed to null. The value is NOT trimmed.</summary>
        public static string GetNonBlankStringOrNull(this JsonElement element, string prop)
        {
            string s = element.GetStringOrNull(prop);
            return string.IsNullOrWhiteSpace(s) ? null : s;
        }

        /// <summary><see cref="GetStringOrNull"/> trimmed, with an empty result collapsed to null.</summary>
        public static string GetTrimmedStringOrNull(this JsonElement element, string prop)
        {
            string s = element.GetStringOrNull(prop);
            if (s == null)
            {
                return null;
            }

            s = s.Trim();
            return s.Length == 0 ? null : s;
        }

        /// <summary><see cref="GetStringOrNull"/> or <see cref="string.Empty"/>.</summary>
        public static string GetStringOrEmpty(this JsonElement element, string prop) =>
            element.GetStringOrNull(prop) ?? string.Empty;

        /// <summary>
        /// String value, or a JSON number's raw text verbatim ("12", "1.50", "1e3"). Bool/null/object/array give null.
        /// </summary>
        public static string GetStringOrNumberStringOrNull(this JsonElement element, string prop)
        {
            if (!element.TryGetPropertySafe(prop, out JsonElement v))
            {
                return null;
            }

            if (v.ValueKind == JsonValueKind.String)
            {
                return v.GetString();
            }

            return v.ValueKind == JsonValueKind.Number ? v.GetRawText() : null;
        }

        /// <summary><see cref="GetStringOrNumberStringOrNull"/> or <see cref="string.Empty"/>.</summary>
        public static string GetStringOrNumberStringOrEmpty(this JsonElement element, string prop) =>
            element.GetStringOrNumberStringOrNull(prop) ?? string.Empty;

        /// <summary>
        /// Like <see cref="GetStringOrNumberStringOrNull"/> but a blank (null/empty/whitespace) string gives null. Untrimmed.
        /// </summary>
        public static string GetNonBlankStringOrNumberStringOrNull(this JsonElement element, string prop)
        {
            if (!element.TryGetPropertySafe(prop, out JsonElement v))
            {
                return null;
            }

            if (v.ValueKind == JsonValueKind.Number)
            {
                return v.GetRawText();
            }

            if (v.ValueKind == JsonValueKind.String)
            {
                string s = v.GetString();
                return string.IsNullOrWhiteSpace(s) ? null : s;
            }

            return null;
        }

        /// <summary>
        /// String value; JSON null or missing gives null; ANY other kind gives its raw JSON text
        /// (numbers verbatim, <c>true</c>/<c>false</c> lower-case, objects and arrays as JSON).
        /// </summary>
        public static string GetStringOrRawTextOrNull(this JsonElement element, string prop)
        {
            if (!element.TryGetPropertySafe(prop, out JsonElement v))
            {
                return null;
            }

            return v.AsStringOrRawTextOrNull();
        }

        /// <summary>
        /// Scalar text: a string (with "" collapsed to null; whitespace kept), a number or boolean as raw JSON text
        /// ("12", "true"). Null, object, array and missing give null.
        /// </summary>
        public static string GetScalarTextOrNull(this JsonElement element, string prop)
        {
            if (!element.TryGetPropertySafe(prop, out JsonElement v))
            {
                return null;
            }

            switch (v.ValueKind)
            {
                case JsonValueKind.String:
                    string s = v.GetString();
                    return string.IsNullOrEmpty(s) ? null : s;
                case JsonValueKind.Number:
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return v.GetRawText();
                default:
                    return null;
            }
        }

        /// <summary>
        /// .NET <see cref="JsonElement.ToString"/> rendering of a PRESENT property: a string's value, raw text for
        /// numbers/objects/arrays, "True"/"False" (capitalized) for booleans, "" for JSON null. Missing gives null.
        /// Kept for id fields that legacy code rendered this way.
        /// </summary>
        public static string GetStringOrToString(this JsonElement element, string prop)
        {
            if (!element.TryGetPropertySafe(prop, out JsonElement v))
            {
                return null;
            }

            return v.ValueKind == JsonValueKind.String ? v.GetString() : v.ToString();
        }

        /// <summary><see cref="GetStringOrToString"/> with missing giving <see cref="string.Empty"/>.</summary>
        public static string GetToStringOrEmpty(this JsonElement element, string prop) =>
            element.GetStringOrToString(prop) ?? string.Empty;

        /// <summary>
        /// The first of <paramref name="props"/> whose <see cref="GetStringOrNull"/> value is not blank, returned
        /// UNtrimmed; null when none.
        /// </summary>
        public static string GetFirstNonBlankStringOrNull(this JsonElement element, params string[] props)
        {
            if (props == null)
            {
                return null;
            }

            foreach (string prop in props)
            {
                string s = element.GetStringOrNull(prop);
                if (!string.IsNullOrWhiteSpace(s))
                {
                    return s;
                }
            }

            return null;
        }

        // ------------------------------------------------------------------ strings (value level)

        /// <summary>A string's value; null/Undefined give null; any other kind gives its raw JSON text.</summary>
        public static string AsStringOrRawTextOrNull(this JsonElement value)
        {
            switch (value.ValueKind)
            {
                case JsonValueKind.String:
                    return value.GetString();
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    return null;
                default:
                    return value.GetRawText();
            }
        }

        /// <summary>A string's value (null-safe to ""), else <see cref="JsonElement.ToString"/> (JSON null and Undefined give "", booleans "True"/"False").</summary>
        public static string AsStringOrToString(this JsonElement value) =>
            value.ValueKind == JsonValueKind.String ? value.GetString() ?? string.Empty : value.ToString();

        /// <summary>Raw JSON text (a string comes back QUOTED), or null for Undefined or JSON null.</summary>
        public static string RawTextOrNull(this JsonElement value) =>
            value.ValueKind != JsonValueKind.Undefined && value.ValueKind != JsonValueKind.Null
                ? value.GetRawText()
                : null;

        /// <summary>Raw JSON text (a string comes back QUOTED), or null when no value, Undefined or JSON null.</summary>
        public static string RawTextOrNull(this JsonElement? value) =>
            value.HasValue ? value.Value.RawTextOrNull() : null;

        // ------------------------------------------------------------------ booleans

        /// <summary>JSON true/false; anything else (missing, "true", 1, null) gives <paramref name="def"/>.</summary>
        public static bool GetBoolOrDefault(this JsonElement element, string prop, bool def = false)
        {
            if (!element.TryGetPropertySafe(prop, out JsonElement v))
            {
                return def;
            }

            switch (v.ValueKind)
            {
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                default:
                    return def;
            }
        }

        /// <summary>JSON true/false; anything else gives null.</summary>
        public static bool? GetBoolOrNull(this JsonElement element, string prop)
        {
            if (!element.TryGetPropertySafe(prop, out JsonElement v))
            {
                return null;
            }

            switch (v.ValueKind)
            {
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                default:
                    return null;
            }
        }

        /// <summary>
        /// JSON true/false, or a string accepted by <see cref="bool.TryParse(string, out bool)"/> ("true"/"false", any case,
        /// surrounding whitespace allowed; NOT "1"/"yes"). Anything else gives null.
        /// </summary>
        public static bool? GetBoolCoercedOrNull(this JsonElement element, string prop)
        {
            if (!element.TryGetPropertySafe(prop, out JsonElement v))
            {
                return null;
            }

            switch (v.ValueKind)
            {
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.String:
                    return bool.TryParse(v.GetString(), out bool parsed) ? parsed : (bool?)null;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Flag reader: JSON true/false; a string that is exactly "1" (ordinal) or "true" (any case, NO trim); a number
        /// that is an integer and non-zero (1.5 gives false). Everything else, including "yes", " true", "0", gives false.
        /// </summary>
        public static bool GetBoolLenient(this JsonElement element, string prop)
        {
            if (!element.TryGetPropertySafe(prop, out JsonElement v))
            {
                return false;
            }

            switch (v.ValueKind)
            {
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.String:
                    string s = v.GetString();
                    return string.Equals(s, "1", StringComparison.Ordinal)
                        || string.Equals(s, "true", StringComparison.OrdinalIgnoreCase);
                case JsonValueKind.Number:
                    return v.TryGetInt64(out long n) && n != 0;
                default:
                    return false;
            }
        }

        // ------------------------------------------------------------------ integers

        /// <summary>A JSON number representable as <see cref="int"/>; otherwise null (1.5, out of range, strings).</summary>
        public static int? GetIntOrNull(this JsonElement element, string prop) =>
            element.TryGetPropertySafe(prop, out JsonElement v)
                && v.ValueKind == JsonValueKind.Number
                && v.TryGetInt32(out int n)
                ? n
                : (int?)null;

        /// <summary><see cref="GetIntOrNull"/> or <paramref name="def"/>.</summary>
        public static int GetIntOrDefault(this JsonElement element, string prop, int def) =>
            element.GetIntOrNull(prop) ?? def;

        /// <summary>
        /// A JSON number, or a string parsed with <see cref="NumberStyles.Integer"/> invariant, that fits <see cref="int"/>;
        /// otherwise null.
        /// </summary>
        public static int? GetIntCoercedOrNull(this JsonElement element, string prop)
        {
            if (!element.TryGetPropertySafe(prop, out JsonElement v))
            {
                return null;
            }

            if (v.ValueKind == JsonValueKind.Number && v.TryGetInt32(out int n))
            {
                return n;
            }

            if (v.ValueKind == JsonValueKind.String
                && int.TryParse(v.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int s))
            {
                return s;
            }

            return null;
        }

        /// <summary>
        /// A JSON number that is an exact integer in <see cref="long"/> range (no precision loss above 2^53); otherwise
        /// null. Strings give null.
        /// </summary>
        public static long? GetInt64OrNull(this JsonElement element, string prop) =>
            element.TryGetPropertySafe(prop, out JsonElement v)
                && v.ValueKind == JsonValueKind.Number
                && v.TryGetInt64(out long n)
                ? n
                : (long?)null;

        /// <summary>
        /// A JSON integer number, or a string parsed with <see cref="NumberStyles.Integer"/> under the invariant culture
        /// (surrounding whitespace and a leading sign allowed; no thousands, decimal point or exponent). Exact for every
        /// <see cref="long"/>. Otherwise null (1.5, "4.0", "1,000", booleans, overflow).
        /// </summary>
        public static long? GetInt64CoercedOrNull(this JsonElement element, string prop)
        {
            if (!element.TryGetPropertySafe(prop, out JsonElement v))
            {
                return null;
            }

            if (v.ValueKind == JsonValueKind.Number && v.TryGetInt64(out long n))
            {
                return n;
            }

            if (v.ValueKind == JsonValueKind.String
                && long.TryParse(v.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out long s))
            {
                return s;
            }

            return null;
        }

        /// <summary>
        /// JavaScript-compatible id reader (mirrors a Node route's <c>parseInt(v, 10)</c>): an integer number exactly; a
        /// fractional number TRUNCATED toward zero (returns null when outside <see cref="long"/> range); a string via
        /// <see cref="ParseIntLeading"/> ("12abc" gives 12). Anything else gives null.
        /// </summary>
        public static long? GetInt64JsParseIntOrNull(this JsonElement element, string prop)
        {
            if (!element.TryGetPropertySafe(prop, out JsonElement v))
            {
                return null;
            }

            if (v.ValueKind == JsonValueKind.Number)
            {
                if (v.TryGetInt64(out long n))
                {
                    return n;
                }

                if (v.TryGetDouble(out double d))
                {
                    return TruncateToInt64OrNull(d);
                }

                return null;
            }

            return v.ValueKind == JsonValueKind.String ? ParseIntLeading(v.GetString()) : null;
        }

        /// <summary>
        /// JavaScript <c>parseInt(s, 10)</c> for longs: skip leading whitespace, optional '+'/'-', then the run of ASCII
        /// digits; trailing junk ignored ("12abc" gives 12). Null when there are no digits or the value overflows.
        /// </summary>
        public static long? ParseIntLeading(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            int i = 0;
            while (i < text.Length && char.IsWhiteSpace(text[i]))
            {
                i++;
            }

            int start = i;
            if (i < text.Length && (text[i] == '+' || text[i] == '-'))
            {
                i++;
            }

            int digitsStart = i;
            while (i < text.Length && text[i] >= '0' && text[i] <= '9')
            {
                i++;
            }

            if (i == digitsStart)
            {
                return null;
            }

            return long.TryParse(text.Substring(start, i - start), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out long parsed)
                ? parsed
                : (long?)null;
        }

        // ------------------------------------------------------------------ floating point

        /// <summary>
        /// Value-level lenient double: a JSON number, or a string parsed with <see cref="NumberStyles.Float"/> invariant.
        /// False with <paramref name="result"/> = 0 otherwise.
        /// </summary>
        public static bool TryGetDoubleLenient(this JsonElement value, out double result)
        {
            if (value.ValueKind == JsonValueKind.Number && value.TryGetDouble(out result))
            {
                return true;
            }

            if (value.ValueKind == JsonValueKind.String
                && double.TryParse(value.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out result))
            {
                return true;
            }

            result = 0;
            return false;
        }

        /// <summary>
        /// A JSON number, or a numeric string parsed with <paramref name="styles"/> invariant (default
        /// <see cref="NumberStyles.Float"/>: whitespace, sign, decimal point, exponent; no thousands). Otherwise null.
        /// Note: the strings "NaN"/"Infinity" parse under Float.
        /// </summary>
        public static double? GetDoubleCoercedOrNull(this JsonElement element, string prop, NumberStyles styles = NumberStyles.Float)
        {
            if (!element.TryGetPropertySafe(prop, out JsonElement v))
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

        /// <summary><see cref="GetDoubleCoercedOrNull"/> or <paramref name="fallback"/> (JS <c>parseFloat(x) || 0</c> style when 0).</summary>
        public static double GetDoubleCoercedOr(this JsonElement element, string prop, double fallback = 0, NumberStyles styles = NumberStyles.Float) =>
            element.GetDoubleCoercedOrNull(prop, styles) ?? fallback;

        /// <summary>A JSON number as double (strings give null); otherwise null.</summary>
        public static double? GetDoubleOrNull(this JsonElement element, string prop) =>
            element.TryGetPropertySafe(prop, out JsonElement v)
                && v.ValueKind == JsonValueKind.Number
                && v.TryGetDouble(out double d)
                ? d
                : (double?)null;

        /// <summary><see cref="GetDoubleOrNull"/> or <paramref name="fallback"/>.</summary>
        public static double GetDoubleOr(this JsonElement element, string prop, double fallback) =>
            element.GetDoubleOrNull(prop) ?? fallback;

        // ------------------------------------------------------------------ decimal

        /// <summary>A JSON number representable as <see cref="decimal"/>; otherwise null. Strings give null.</summary>
        public static decimal? GetDecimalOrNull(this JsonElement element, string prop) =>
            element.TryGetPropertySafe(prop, out JsonElement v)
                && v.ValueKind == JsonValueKind.Number
                && v.TryGetDecimal(out decimal d)
                ? d
                : (decimal?)null;

        /// <summary>
        /// A JSON number representable as decimal, or a string parsed with <paramref name="styles"/> invariant
        /// (default <see cref="NumberStyles.Number"/>: whitespace, sign, decimal point, thousands; pass
        /// <see cref="NumberStyles.Any"/> to also allow currency symbols, parentheses and exponents). Otherwise null.
        /// </summary>
        public static decimal? GetDecimalCoercedOrNull(this JsonElement element, string prop, NumberStyles styles = NumberStyles.Number)
        {
            if (!element.TryGetPropertySafe(prop, out JsonElement v))
            {
                return null;
            }

            if (v.ValueKind == JsonValueKind.Number && v.TryGetDecimal(out decimal d))
            {
                return d;
            }

            if (v.ValueKind == JsonValueKind.String
                && decimal.TryParse(v.GetString(), styles, CultureInfo.InvariantCulture, out decimal sd))
            {
                return sd;
            }

            return null;
        }

        // ------------------------------------------------------------------ dates

        /// <summary>
        /// A JSON string parsed as a UTC date: invariant culture, <see cref="DateTimeStyles.AdjustToUniversal"/> |
        /// <see cref="DateTimeStyles.AssumeUniversal"/> (an offset is converted; no zone means UTC). Kind is Utc. Otherwise null.
        /// </summary>
        public static DateTime? GetUtcDateTimeOrNull(this JsonElement element, string prop)
        {
            string raw = element.GetStringOrNull(prop);
            return raw != null
                && DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out DateTime d)
                ? d
                : (DateTime?)null;
        }

        /// <summary>
        /// Printful-style created-at resolution: <paramref name="unixSecondsProp"/> as an integer JSON number of Unix
        /// SECONDS wins (UTC); else <paramref name="isoProp"/> as a string parsed invariant with
        /// <see cref="DateTimeStyles.AdjustToUniversal"/> only (a zone-less value is read as server-local, then adjusted);
        /// else null. Callers that defaulted to "now" use <c>?? DateTime.UtcNow</c>.
        /// </summary>
        public static DateTime? GetUnixSecondsOrIsoUtcOrNull(this JsonElement element, string unixSecondsProp, string isoProp)
        {
            if (element.TryGetPropertySafe(unixSecondsProp, out JsonElement created)
                && created.ValueKind == JsonValueKind.Number
                && created.TryGetInt64(out long secs)
                && secs >= -62135596800L
                && secs <= 253402300799L)
            {
                return DateTimeOffset.FromUnixTimeSeconds(secs).UtcDateTime;
            }

            string iso = element.GetStringOrNull(isoProp);
            if (iso != null && DateTime.TryParse(iso, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out DateTime dt))
            {
                return dt;
            }

            return null;
        }

        private static long? TruncateToInt64OrNull(double d)
        {
            if (double.IsNaN(d) || double.IsInfinity(d))
            {
                return null;
            }

            double t = Math.Truncate(d);
            if (t < -9223372036854775808.0 || t >= 9223372036854775808.0)
            {
                return null;
            }

            return (long)t;
        }
    }
}
