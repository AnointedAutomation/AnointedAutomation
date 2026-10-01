// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Copyright 2026 Anointed Automation, LLC. All Rights Reserved.
// Coded by Alexander Fields https://www.alexanderfields.me
// Standardization primitive (#2 of the 2026-07-30 backend duplication audit): typed, null-safe
// BsonDocument field accessors replacing the ~500 hand-rolled "doc["x"].AsString / .AsInt32" reads
// (each with its own TryGetValue + BsonNull guard) scattered across the repositories. Every getter
// returns the supplied default when the element is missing, BsonNull, or the wrong type, so a mapping
// never throws on a partially-populated document.

using System;
using System.Globalization;
using MongoDB.Bson;

namespace AnointedAutomation.Optimization
{
    /// <summary>
    /// Null-safe extension accessors for <see cref="BsonDocument"/> / <see cref="BsonValue"/> fields.
    /// </summary>
    public static class BsonMap
    {
        /// <summary>True when the named element exists and is neither BsonNull nor absent.</summary>
        public static bool Has(this BsonDocument doc, string name)
        {
            return doc != null
                && doc.TryGetValue(name, out BsonValue value)
                && value != null
                && !value.IsBsonNull;
        }

        /// <summary>
        /// Returns the element as a string when it is present and actually a BSON string; otherwise
        /// <paramref name="fallback"/>. Strict (a present non-string yields the fallback, never a rendered
        /// value), matching the `doc["k"].IsString ? doc["k"].AsString : fallback` guarded ternary.
        /// </summary>
        public static string GetStringOr(this BsonDocument doc, string name, string fallback = "")
        {
            if (!doc.Has(name))
            {
                return fallback;
            }

            BsonValue value = doc[name];
            return value.IsString ? value.AsString : fallback;
        }

        /// <summary>
        /// Returns the element as a string ONLY when it is present and actually a BSON string; otherwise
        /// null. Unlike <see cref="GetStringOr"/> this never renders a non-string value, so it is the exact
        /// replacement for the common `doc.Contains("k") &amp;&amp; doc["k"].IsString ? doc["k"].AsString : null`
        /// guarded ternary.
        /// </summary>
        public static string? GetStringOrNull(this BsonDocument doc, string name)
        {
            if (!doc.Has(name))
            {
                return null;
            }

            BsonValue value = doc[name];
            return value.IsString ? value.AsString : null;
        }

        /// <summary>Returns the element as an int, or <paramref name="fallback"/> when missing/non-numeric.</summary>
        public static int GetIntOr(this BsonDocument doc, string name, int fallback = 0)
        {
            if (!doc.Has(name))
            {
                return fallback;
            }

            BsonValue value = doc[name];
            return value.IsNumeric ? value.ToInt32() : fallback;
        }

        /// <summary>Returns the element as a long, or <paramref name="fallback"/> when missing/non-numeric.</summary>
        public static long GetLongOr(this BsonDocument doc, string name, long fallback = 0)
        {
            if (!doc.Has(name))
            {
                return fallback;
            }

            BsonValue value = doc[name];
            return value.IsNumeric ? value.ToInt64() : fallback;
        }

        /// <summary>Returns the element as a double, or <paramref name="fallback"/> when missing/non-numeric.</summary>
        public static double GetDoubleOr(this BsonDocument doc, string name, double fallback = 0)
        {
            if (!doc.Has(name))
            {
                return fallback;
            }

            BsonValue value = doc[name];
            return value.IsNumeric ? value.ToDouble() : fallback;
        }

        /// <summary>Returns the element as a bool, or <paramref name="fallback"/> when missing/non-boolean.</summary>
        public static bool GetBoolOr(this BsonDocument doc, string name, bool fallback = false)
        {
            if (!doc.Has(name))
            {
                return fallback;
            }

            BsonValue value = doc[name];
            return value.IsBoolean ? value.AsBoolean : fallback;
        }

        /// <summary>
        /// Returns the element as a UTC <see cref="DateTime"/>, or <paramref name="fallback"/> when missing
        /// or not a BSON date. Nullable overload of the same is <see cref="GetNullableDateTime"/>.
        /// </summary>
        public static DateTime GetDateTimeOr(this BsonDocument doc, string name, DateTime fallback)
        {
            if (!doc.Has(name))
            {
                return fallback;
            }

            BsonValue value = doc[name];
            return value.IsValidDateTime ? value.ToUniversalTime() : fallback;
        }

        /// <summary>Returns the element as a UTC <see cref="DateTime"/>, or null when missing/not a date.</summary>
        public static DateTime? GetNullableDateTime(this BsonDocument doc, string name)
        {
            if (!doc.Has(name))
            {
                return null;
            }

            BsonValue value = doc[name];
            return value.IsValidDateTime ? value.ToUniversalTime() : (DateTime?)null;
        }

        // ---------------------------------------------------------------------------------------------
        // Coercing / fallback overloads. These carry semantics the strict accessors above deliberately
        // do NOT (trim-and-collapse-empty, pascal/camel key fallback, string->number coercion). They
        // consolidate the last per-service private copies (PricingConformance, CompetitorMatchTask,
        // PrintfulCommissionLogic, AiTaskDoc) without changing any of their parsed values.
        // ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Returns the element as a trimmed, non-empty string, or null when missing, non-string, or
        /// blank after trimming. Unlike <see cref="GetStringOrNull(BsonDocument,string)"/> this trims
        /// and treats a whitespace-only value as absent.
        /// </summary>
        public static string? GetTrimmedStringOrNull(this BsonDocument doc, string name)
        {
            if (doc == null || !doc.TryGetValue(name, out BsonValue value) || !value.IsString)
            {
                return null;
            }

            string s = value.AsString.Trim();
            return s.Length == 0 ? null : s;
        }

        /// <summary>
        /// Pascal/camel fallback variant of <see cref="GetTrimmedStringOrNull(BsonDocument,string)"/>.
        /// The FIRST present key wins even when its value is null or non-string (the fallback key is only
        /// consulted when the primary key is absent entirely), matching the hybrid-casing cache reads.
        /// </summary>
        public static string? GetTrimmedStringOrNull(this BsonDocument doc, string primaryName, string fallbackName)
        {
            BsonValue? value =
                doc.TryGetValue(primaryName, out BsonValue p) ? p
                : doc.TryGetValue(fallbackName, out BsonValue c) ? c
                : null;
            if (value == null || value.IsBsonNull || !value.IsString)
            {
                return null;
            }

            string s = value.AsString.Trim();
            return s.Length == 0 ? null : s;
        }

        /// <summary>
        /// Renders a numeric element as its integral string form (<c>((long)value.ToDouble())</c>),
        /// or null when the element is missing or not numeric. Matches the legacy-id rendering used to
        /// backfill a missing string id from a numeric one.
        /// </summary>
        public static string? GetNumberAsStringOrNull(this BsonDocument doc, string name) =>
            doc.TryGetValue(name, out BsonValue v) && v != null && v.IsNumeric
                ? ((long)v.ToDouble()).ToString(CultureInfo.InvariantCulture)
                : null;

        /// <summary>
        /// Pascal/camel double reader that coerces a numeric OR numeric-string value, returning
        /// <paramref name="fallback"/> when missing, null, or unparseable. String values are trimmed and
        /// parsed with <see cref="NumberStyles.Any"/> under the invariant culture.
        /// </summary>
        public static double GetDoubleCoercedOr(
            this BsonDocument doc, string primaryName, string fallbackName, double fallback = 0)
        {
            BsonValue? value =
                doc.TryGetValue(primaryName, out BsonValue p) ? p
                : doc.TryGetValue(fallbackName, out BsonValue c) ? c
                : null;
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
        /// Nullable double reader that coerces a numeric OR numeric-string value, returning null when the
        /// element is missing, BsonNull, or unparseable. The parse <paramref name="styles"/> and
        /// <paramref name="provider"/> are explicit because the copies this replaced disagreed: money reads
        /// used invariant <see cref="NumberStyles.Float"/>, while others matched the default
        /// <c>double.TryParse(string)</c> (Float + AllowThousands under the current culture).
        /// </summary>
        public static double? GetDoubleCoercedOrNull(
            this BsonDocument doc, string name, NumberStyles styles = NumberStyles.Float, IFormatProvider? provider = null)
        {
            if (!doc.Has(name))
            {
                return null;
            }

            BsonValue value = doc[name];
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
        /// Nullable bool reader that coerces a boolean OR the strings "true"/"false", returning null when
        /// missing or otherwise unparseable.
        /// </summary>
        public static bool? GetBoolCoercedOrNull(this BsonDocument doc, string name)
        {
            if (doc == null || !doc.TryGetValue(name, out BsonValue v))
            {
                return null;
            }
            if (v.IsBoolean)
            {
                return v.AsBoolean;
            }
            return v.IsString && bool.TryParse(v.AsString, out bool parsed) ? parsed : (bool?)null;
        }

        // ---------------------------------------------------------------------------------------------
        // BsonValue overloads for documents read through a multi-key picker (see AiTaskDoc): the caller
        // resolves the value first, then reads it. Same coercion rules as the BsonDocument readers above.
        // ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Returns a string form of the value: its raw string when it IS a BSON string, otherwise its
        /// rendered <see cref="BsonValue.ToString"/>. Null when the value itself is null.
        /// </summary>
        public static string? AsStringOrNull(this BsonValue? value) =>
            value == null ? null : (value.IsString ? value.AsString : value.ToString());

        /// <summary>
        /// Returns the value as a long, coercing Int64/Int32/Double and numeric strings, or
        /// <paramref name="fallback"/> when null or unparseable.
        /// </summary>
        public static long AsInt64Or(this BsonValue? value, long fallback = 0)
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
            if (value.IsDouble)
            {
                return (long)value.AsDouble;
            }
            return value.IsString && long.TryParse(value.AsString, out long parsed) ? parsed : fallback;
        }

        /// <summary>Returns the value as a bool when it is a BSON boolean; otherwise <paramref name="fallback"/>.</summary>
        public static bool AsBooleanOr(this BsonValue? value, bool fallback = false) =>
            value != null && value.IsBoolean ? value.AsBoolean : fallback;

        /// <summary>Returns the value as a UTC <see cref="DateTime"/>, or null when null / not a date.</summary>
        public static DateTime? AsNullableUtcDateTime(this BsonValue? value) =>
            value != null && value.IsValidDateTime ? value.ToUniversalTime() : (DateTime?)null;
    }
}
