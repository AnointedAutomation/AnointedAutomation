// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// Lenient, never-throwing BsonDocument / BsonValue readers. Superset of the Anointed API's Services/Common/BsonMap.cs
// plus every private BSON reader catalogued in the API, each distinct semantic as its own named method.
// Deliberate fixes versus the legacy copies (documented per member):
//   * BsonNull is never rendered as the text "BsonNull".
//   * Narrowing (Int64 -> int, double -> long/int) is range-checked: overflow yields the fallback/null, never a wrapped value.
//   * Integral values are never routed through double (no precision loss above 2^53).
//   * String number parsing is culture-invariant.
// Lives in its own namespace (AnointedAutomation.Repository.Mongo.Bson) so importing the root namespace does not bring
// these extension names into scope next to a consumer's own helpers.

using System;
using System.Globalization;
using MongoDB.Bson;

namespace AnointedAutomation.Repository.Mongo.Bson
{
    /// <summary>Null-safe accessors for <see cref="BsonDocument"/> / <see cref="BsonValue"/>.</summary>
    public static class BsonValueExtensions
    {
        // ================================================================== presence / navigation

        /// <summary>True when <paramref name="doc"/> is non-null and the element exists and is not BsonNull.</summary>
        public static bool Has(this BsonDocument doc, string name) =>
            doc != null
            && doc.TryGetValue(name, out BsonValue value)
            && value != null
            && !value.IsBsonNull;

        /// <summary>The element when present and not BsonNull; otherwise null. Null doc gives null.</summary>
        public static BsonValue GetValueOrNull(this BsonDocument doc, string name) =>
            doc.Has(name) ? doc[name] : null;

        /// <summary>
        /// Dotted-path lookup ("a.b.c"): a name without '.' is a direct lookup; otherwise each segment must be an
        /// embedded document. Any miss gives null. A BsonNull leaf is returned as BsonNull. Null doc gives null.
        /// </summary>
        public static BsonValue GetPath(this BsonDocument doc, string path)
        {
            if (doc == null || path == null)
            {
                return null;
            }

            if (path.IndexOf('.') < 0)
            {
                return doc.TryGetValue(path, out BsonValue direct) ? direct : null;
            }

            BsonValue current = doc;
            foreach (string part in path.Split('.'))
            {
                if (current is BsonDocument d && d.TryGetValue(part, out BsonValue next))
                {
                    current = next;
                }
                else
                {
                    return null;
                }
            }

            return current;
        }

        /// <summary>The first of <paramref name="keys"/> present and not BsonNull; null when none.</summary>
        public static BsonValue PickFirstNonNull(this BsonDocument doc, params string[] keys)
        {
            if (doc == null || keys == null)
            {
                return null;
            }

            foreach (string k in keys)
            {
                if (doc.TryGetValue(k, out BsonValue v) && v != null && !v.IsBsonNull)
                {
                    return v;
                }
            }

            return null;
        }

        /// <summary>
        /// The value of <paramref name="primaryName"/> when the key EXISTS (even BsonNull), else of
        /// <paramref name="fallbackName"/> when it exists, else null. (Pascal/camel key fallback.)
        /// </summary>
        public static BsonValue GetFirstPresent(this BsonDocument doc, string primaryName, string fallbackName)
        {
            if (doc == null)
            {
                return null;
            }

            if (doc.TryGetValue(primaryName, out BsonValue p))
            {
                return p;
            }

            return doc.TryGetValue(fallbackName, out BsonValue c) ? c : null;
        }

        // ================================================================== strings (document)

        /// <summary>The element when it is a BSON string; otherwise <paramref name="fallback"/>.</summary>
        public static string GetStringOr(this BsonDocument doc, string name, string fallback = "")
        {
            BsonValue value = doc.GetValueOrNull(name);
            return value != null && value.IsString ? value.AsString : fallback;
        }

        /// <summary>The element when it is a BSON string (untrimmed, "" kept); otherwise null.</summary>
        public static string GetStringOrNull(this BsonDocument doc, string name) =>
            doc.GetStringOr(name, null);

        /// <summary><see cref="GetStringOrNull"/> with "" collapsed to null. Whitespace kept, no trim.</summary>
        public static string GetNonEmptyStringOrNull(this BsonDocument doc, string name)
        {
            string s = doc.GetStringOrNull(name);
            return string.IsNullOrEmpty(s) ? null : s;
        }

        /// <summary>A BSON string trimmed, with a blank result collapsed to null; otherwise null.</summary>
        public static string GetTrimmedStringOrNull(this BsonDocument doc, string name) =>
            TrimmedOrNull(doc.GetValueOrNull(name));

        /// <summary>
        /// Two-key variant of <see cref="GetTrimmedStringOrNull(BsonDocument, string)"/>: the FIRST PRESENT key wins even
        /// when its value is BsonNull or non-string (the fallback key is consulted only when the primary is absent).
        /// </summary>
        public static string GetTrimmedStringOrNull(this BsonDocument doc, string primaryName, string fallbackName) =>
            TrimmedOrNull(doc.GetFirstPresent(primaryName, fallbackName));

        /// <summary>
        /// A numeric element rendered as its integral invariant string: Int32/Int64 exactly (no double round-trip, so ids
        /// above 2^53 survive), Double/Decimal128 truncated toward zero (null when non-finite or out of long range).
        /// Missing, BsonNull, strings and other types give null.
        /// </summary>
        public static string GetNumberAsStringOrNull(this BsonDocument doc, string name)
        {
            long? n = doc.GetValueOrNull(name).AsInt64TruncOrNull(includeDecimal128: true);
            return n.HasValue ? n.Value.ToString(CultureInfo.InvariantCulture) : null;
        }

        /// <summary>
        /// The first of the dotted <paramref name="paths"/> (see <see cref="GetPath"/>) whose value is present and not
        /// BsonNull: a string's value, otherwise the value's <see cref="BsonValue.ToString"/>. "" when none.
        /// </summary>
        public static string GetStringFromPathsOrEmpty(this BsonDocument doc, params string[] paths)
        {
            if (paths == null)
            {
                return string.Empty;
            }

            foreach (string path in paths)
            {
                BsonValue v = doc.GetPath(path);
                if (v == null || v.IsBsonNull)
                {
                    continue;
                }

                return (v.IsString ? v.AsString : v.ToString()) ?? string.Empty;
            }

            return string.Empty;
        }

        // ================================================================== numbers (document)

        /// <summary>
        /// Numeric element (Int32, Int64, Double truncated, Decimal128 truncated) as int; <paramref name="fallback"/> when
        /// missing, non-numeric, non-finite, or OUT OF int RANGE (legacy code wrapped silently; this does not).
        /// </summary>
        public static int GetIntOr(this BsonDocument doc, string name, int fallback = 0) =>
            doc.GetValueOrNull(name).AsInt32TruncOrNull(includeDecimal128: true) ?? fallback;

        /// <summary>
        /// Numeric element (Int32, Int64, Double truncated, Decimal128 truncated) as long; <paramref name="fallback"/> when
        /// missing, non-numeric, non-finite or out of long range.
        /// </summary>
        public static long GetLongOr(this BsonDocument doc, string name, long fallback = 0) =>
            doc.GetValueOrNull(name).AsInt64TruncOrNull(includeDecimal128: true) ?? fallback;

        /// <summary>Int32 or Int64 ONLY (Double, Decimal128, strings give the fallback) as long.</summary>
        public static long GetIntegralInt64Or(this BsonDocument doc, string name, long fallback = 0)
        {
            BsonValue v = doc.GetValueOrNull(name);
            if (v == null)
            {
                return fallback;
            }

            if (v.IsInt64)
            {
                return v.AsInt64;
            }

            return v.IsInt32 ? v.AsInt32 : fallback;
        }

        /// <summary>Int32, or Int64 within int range, ONLY; anything else (including Int64 overflow) gives the fallback.</summary>
        public static int GetIntegralInt32Or(this BsonDocument doc, string name, int fallback = 0)
        {
            BsonValue v = doc.GetValueOrNull(name);
            if (v == null)
            {
                return fallback;
            }

            if (v.IsInt32)
            {
                return v.AsInt32;
            }

            if (v.IsInt64 && v.AsInt64 >= int.MinValue && v.AsInt64 <= int.MaxValue)
            {
                return (int)v.AsInt64;
            }

            return fallback;
        }

        /// <summary>Int64, Int32 or Double (truncated, range-checked) as long; strings/Decimal128/other give the fallback.</summary>
        public static long GetInt64TruncOr(this BsonDocument doc, string name, long fallback = 0) =>
            doc.GetValueOrNull(name).AsInt64TruncOrNull(includeDecimal128: false) ?? fallback;

        /// <summary>
        /// The first of the dotted <paramref name="paths"/> holding a usable number: a numeric value (Double/Decimal128
        /// truncated, range-checked) or a string parsed with <see cref="NumberStyles.Integer"/> invariant. A BsonNull, an
        /// unparseable string or an out-of-range number falls through to the next path. 0 when none.
        /// </summary>
        public static long GetInt64FromPathsOr(this BsonDocument doc, long fallback, params string[] paths)
        {
            if (paths == null)
            {
                return fallback;
            }

            foreach (string path in paths)
            {
                BsonValue v = doc.GetPath(path);
                if (v == null || v.IsBsonNull)
                {
                    continue;
                }

                long? n = v.AsInt64TruncOrNull(includeDecimal128: true);
                if (n.HasValue)
                {
                    return n.Value;
                }

                if (v.IsString && long.TryParse(v.AsString, NumberStyles.Integer, CultureInfo.InvariantCulture, out long parsed))
                {
                    return parsed;
                }
            }

            return fallback;
        }

        /// <summary>Numeric element (any BSON numeric type) as double; <paramref name="fallback"/> otherwise.</summary>
        public static double GetDoubleOr(this BsonDocument doc, string name, double fallback = 0)
        {
            BsonValue v = doc.GetValueOrNull(name);
            return v != null && v.IsNumeric ? v.ToDouble() : fallback;
        }

        /// <summary>
        /// Two-key (<see cref="GetFirstPresent"/>) double: numeric as-is, or a string TRIMMED and parsed with
        /// <see cref="NumberStyles.Any"/> invariant; <paramref name="fallback"/> when missing, BsonNull or unparseable.
        /// </summary>
        public static double GetDoubleCoercedOr(this BsonDocument doc, string primaryName, string fallbackName, double fallback = 0)
        {
            BsonValue value = doc.GetFirstPresent(primaryName, fallbackName);
            if (value == null || value.IsBsonNull)
            {
                return fallback;
            }

            if (value.IsNumeric)
            {
                return value.ToDouble();
            }

            return value.IsString
                && double.TryParse(value.AsString.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double parsed)
                ? parsed
                : fallback;
        }

        /// <summary>
        /// Numeric as double, or a string parsed with <paramref name="styles"/> under <paramref name="provider"/> (null means
        /// invariant); null when missing, BsonNull or unparseable.
        /// </summary>
        public static double? GetDoubleCoercedOrNull(this BsonDocument doc, string name, NumberStyles styles = NumberStyles.Float, IFormatProvider provider = null)
        {
            BsonValue value = doc.GetValueOrNull(name);
            if (value == null)
            {
                return null;
            }

            if (value.IsNumeric)
            {
                return value.ToDouble();
            }

            return value.IsString
                && double.TryParse(value.AsString, styles, provider ?? CultureInfo.InvariantCulture, out double parsed)
                ? parsed
                : (double?)null;
        }

        /// <summary>
        /// Like <see cref="GetDoubleCoercedOrNull"/> (Float, invariant) but a NUMERIC value that is NaN or +/-Infinity gives
        /// null (commission math must never propagate them). Note: a STRING "NaN"/"Infinity" still parses, as before.
        /// </summary>
        public static double? GetFiniteDoubleCoercedOrNull(this BsonDocument doc, string name)
        {
            BsonValue value = doc.GetValueOrNull(name);
            if (value == null)
            {
                return null;
            }

            if (value.IsNumeric)
            {
                double n = value.ToDouble();
                return double.IsNaN(n) || double.IsInfinity(n) ? (double?)null : n;
            }

            return value.IsString
                && double.TryParse(value.AsString, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsed)
                ? parsed
                : (double?)null;
        }

        // ================================================================== bool / date (document)

        /// <summary>A BSON boolean; otherwise <paramref name="fallback"/>.</summary>
        public static bool GetBoolOr(this BsonDocument doc, string name, bool fallback = false) =>
            doc.GetValueOrNull(name).AsBooleanOr(fallback);

        /// <summary>A BSON boolean, or a string accepted by <see cref="bool.TryParse(string, out bool)"/>; otherwise null.</summary>
        public static bool? GetBoolCoercedOrNull(this BsonDocument doc, string name)
        {
            BsonValue v = doc.GetValueOrNull(name);
            if (v == null)
            {
                return null;
            }

            if (v.IsBoolean)
            {
                return v.AsBoolean;
            }

            return v.IsString && bool.TryParse(v.AsString, out bool parsed) ? parsed : (bool?)null;
        }

        /// <summary>A BSON date as UTC; otherwise <paramref name="fallback"/>.</summary>
        public static DateTime GetDateTimeOr(this BsonDocument doc, string name, DateTime fallback) =>
            doc.GetValueOrNull(name).AsNullableUtcDateTime() ?? fallback;

        /// <summary>A BSON date as UTC; otherwise null.</summary>
        public static DateTime? GetNullableDateTime(this BsonDocument doc, string name) =>
            doc.GetValueOrNull(name).AsNullableUtcDateTime();

        // ================================================================== value level

        /// <summary>
        /// Text of a value: null for C# null or BsonNull; a string's value; any other value's
        /// <see cref="BsonValue.ToString"/>. (Legacy BsonMap returned "BsonNull" for BsonNull; fixed.)
        /// </summary>
        public static string AsStringOrNull(this BsonValue value)
        {
            if (value == null || value.IsBsonNull)
            {
                return null;
            }

            return value.IsString ? value.AsString : value.ToString();
        }

        /// <summary>A string's value; anything else (including C# null) gives null.</summary>
        public static string AsStringStrictOrNull(this BsonValue value) =>
            value != null && value.IsString ? value.AsString : null;

        /// <summary>
        /// Long from Int64, Int32, Double (truncated, range-checked) or a string parsed with <see cref="NumberStyles.Integer"/>
        /// invariant; <paramref name="fallback"/> otherwise (C# null, BsonNull, Decimal128, non-finite, overflow).
        /// </summary>
        public static long AsInt64Or(this BsonValue value, long fallback = 0)
        {
            long? n = value.AsInt64TruncOrNull(includeDecimal128: false);
            if (n.HasValue)
            {
                return n.Value;
            }

            return value != null && value.IsString
                && long.TryParse(value.AsString, NumberStyles.Integer, CultureInfo.InvariantCulture, out long parsed)
                ? parsed
                : fallback;
        }

        /// <summary>Long from Int64, Int32 or Double (truncated, range-checked); <paramref name="fallback"/> otherwise (strings too).</summary>
        public static long AsInt64TruncOr(this BsonValue value, long fallback = 0) =>
            value.AsInt64TruncOrNull(includeDecimal128: false) ?? fallback;

        /// <summary>
        /// Int64 or Int32 exactly, or a string parsed with <see cref="NumberStyles.Integer"/> invariant; <paramref name="fallback"/>
        /// for everything else INCLUDING Double (a legacy id stored as long-or-string).
        /// </summary>
        public static long AsIntegralOrNumericStringInt64Or(this BsonValue value, long fallback = 0)
        {
            if (value == null)
            {
                return fallback;
            }

            if (value.IsInt64)
            {
                return value.AsInt64;
            }

            if (value.IsInt32)
            {
                return value.AsInt32;
            }

            return value.IsString && long.TryParse(value.AsString, NumberStyles.Integer, CultureInfo.InvariantCulture, out long parsed)
                ? parsed
                : fallback;
        }

        /// <summary>
        /// Int from Int32, Int64 (range-checked) or Double (truncated, range-checked); null otherwise. (Legacy copies
        /// wrapped on overflow; fixed.)
        /// </summary>
        public static int? AsNullableInt32Trunc(this BsonValue value) =>
            value.AsInt32TruncOrNull(includeDecimal128: false);

        /// <summary>
        /// A long id from a BSON scalar: Int64, Int32, Double (truncated, range-checked), or a string whose text after the
        /// last '/' (whole string when none, so "gid://shopify/X/5" and "5" both work) parses with
        /// <see cref="NumberStyles.Integer"/> invariant. Null for C# null, BsonNull and anything else.
        /// </summary>
        public static long? AsInt64OrGidTailOrNull(this BsonValue value)
        {
            if (value == null || value.IsBsonNull)
            {
                return null;
            }

            long? n = value.AsInt64TruncOrNull(includeDecimal128: false);
            if (n.HasValue)
            {
                return n;
            }

            return value.IsString ? ParseGidTail(value.AsString) : null;
        }

        /// <summary>
        /// Like <see cref="AsInt64OrGidTailOrNull"/> but integral-only: Int64, Int32 or a gid-tail string; a Double gives null.
        /// </summary>
        public static long? AsIntegralOrGidTailInt64OrNull(this BsonValue value)
        {
            if (value == null || value.IsBsonNull)
            {
                return null;
            }

            if (value.IsString)
            {
                return ParseGidTail(value.AsString);
            }

            if (value.IsInt64)
            {
                return value.AsInt64;
            }

            return value.IsInt32 ? value.AsInt32 : (long?)null;
        }

        /// <summary>
        /// A quantity: Int32; Int64 within int range; a Double within 0.0001 of an integer (rounded, range-checked); a string
        /// parsed with <see cref="NumberStyles.Integer"/> invariant into int. Null otherwise (1.5, overflow, BsonNull).
        /// </summary>
        public static int? AsQuantityInt32OrNull(this BsonValue value)
        {
            if (value == null || value.IsBsonNull)
            {
                return null;
            }

            if (value.IsInt32)
            {
                return value.AsInt32;
            }

            if (value.IsInt64)
            {
                long v = value.AsInt64;
                return v >= int.MinValue && v <= int.MaxValue ? (int)v : (int?)null;
            }

            if (value.IsDouble)
            {
                double d = value.AsDouble;
                if (double.IsNaN(d) || double.IsInfinity(d))
                {
                    return null;
                }

                double r = Math.Round(d);
                return Math.Abs(d - r) < 0.0001 && r >= int.MinValue && r <= int.MaxValue ? (int)r : (int?)null;
            }

            return value.IsString && int.TryParse(value.AsString, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed)
                ? parsed
                : (int?)null;
        }

        /// <summary>A BSON boolean; otherwise <paramref name="fallback"/>.</summary>
        public static bool AsBooleanOr(this BsonValue value, bool fallback = false) =>
            value != null && value.IsBoolean ? value.AsBoolean : fallback;

        /// <summary>A BSON date as UTC; otherwise null.</summary>
        public static DateTime? AsNullableUtcDateTime(this BsonValue value) =>
            value != null && value.IsValidDateTime ? value.ToUniversalTime() : (DateTime?)null;

        // ================================================================== helpers

        private static string TrimmedOrNull(BsonValue value)
        {
            if (value == null || value.IsBsonNull || !value.IsString)
            {
                return null;
            }

            string s = value.AsString.Trim();
            return s.Length == 0 ? null : s;
        }

        private static long? ParseGidTail(string s)
        {
            if (s == null)
            {
                return null;
            }

            int slash = s.LastIndexOf('/');
            string tail = slash >= 0 ? s.Substring(slash + 1) : s;
            return long.TryParse(tail, NumberStyles.Integer, CultureInfo.InvariantCulture, out long id) ? id : (long?)null;
        }

        private static long? AsInt64TruncOrNull(this BsonValue value, bool includeDecimal128)
        {
            if (value == null)
            {
                return null;
            }

            if (value.IsInt64)
            {
                return value.AsInt64;
            }

            if (value.IsInt32)
            {
                return value.AsInt32;
            }

            if (value.IsDouble)
            {
                return TruncToInt64(value.AsDouble);
            }

            if (includeDecimal128 && value.IsDecimal128)
            {
                Decimal128 d = value.AsDecimal128;
                if (Decimal128.IsNaN(d) || Decimal128.IsInfinity(d))
                {
                    return null;
                }

                decimal m;
                try
                {
                    m = Decimal128.ToDecimal(d);
                }
                catch (OverflowException)
                {
                    return null;
                }

                decimal t = decimal.Truncate(m);
                return t >= long.MinValue && t <= long.MaxValue ? (long)t : (long?)null;
            }

            return null;
        }

        private static int? AsInt32TruncOrNull(this BsonValue value, bool includeDecimal128)
        {
            long? n = value.AsInt64TruncOrNull(includeDecimal128);
            return n.HasValue && n.Value >= int.MinValue && n.Value <= int.MaxValue ? (int)n.Value : (int?)null;
        }

        private static long? TruncToInt64(double d)
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
