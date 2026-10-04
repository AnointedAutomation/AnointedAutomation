// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// Shopify global id (gid://shopify/<Resource>/<id>) formatting and parsing. Moved from the Anointed API
// (Services/Shopify/ShopifyGid.cs + Services/Tracking/FulfillmentGid.cs) and widened into the superset of every
// hand-rolled GID parser there. Each parser shape that behaved differently (bare number accepted or not, query
// stripped or not, validation) is its own explicitly named method.

using System;
using System.Globalization;

namespace AnointedAutomation.Shopify
{
    /// <summary>Format and parse Shopify GIDs (gid://shopify/&lt;Resource&gt;/&lt;id&gt;).</summary>
    public static class ShopifyGid
    {
        /// <summary>"gid://shopify/".</summary>
        public const string Scheme = "gid://shopify/";

        // ------------------------------------------------------------------ formatting

        /// <summary>gid://shopify/{resource}/{id}. No trim, no validation.</summary>
        public static string ToGid(string resource, string id) =>
            $"gid://shopify/{resource}/{id}";

        /// <summary>gid://shopify/{resource}/{id} with the id formatted invariant.</summary>
        public static string ToGid(string resource, long id) =>
            $"gid://shopify/{resource}/{id.ToString(CultureInfo.InvariantCulture)}";

        /// <summary><see cref="ToGid(string, long)"/>, or null for a null id.</summary>
        public static string ToGidOrNull(string resource, long? id) =>
            id.HasValue ? ToGid(resource, id.Value) : null;

        /// <summary>
        /// The input unchanged when it already starts with "gid://" (ordinal, no trim); otherwise
        /// <see cref="ToGid(string, string)"/>. Null input throws <see cref="ArgumentNullException"/>.
        /// </summary>
        public static string EnsureGid(string resource, string idOrGid)
        {
            if (idOrGid == null)
            {
                throw new ArgumentNullException(nameof(idOrGid));
            }

            return idOrGid.StartsWith("gid://", StringComparison.Ordinal) ? idOrGid : ToGid(resource, idOrGid);
        }

        /// <summary>
        /// Trimmed variant of <see cref="EnsureGid"/>: null is treated as "", the value is trimmed, then returned as-is
        /// when it starts with "gid://" (compared with <paramref name="prefixComparison"/>), otherwise formatted with
        /// <see cref="ToGid(string, string)"/> (so null/blank gives "gid://shopify/{resource}/").
        /// </summary>
        public static string EnsureGidTrimmed(string resource, string idOrGid, StringComparison prefixComparison = StringComparison.Ordinal)
        {
            string trimmed = (idOrGid ?? string.Empty).Trim();
            return trimmed.StartsWith("gid://", prefixComparison) ? trimmed : ToGid(resource, trimmed);
        }

        /// <summary>
        /// Trims the input; when it is a plain non-negative integer (digits only, invariant) returns
        /// gid://shopify/{resource}/{n} (leading zeros dropped), otherwise the trimmed input unchanged (gids pass
        /// through). Null gives null.
        /// </summary>
        public static string NormalizeNumericToGid(string resource, string idOrGid)
        {
            if (idOrGid == null)
            {
                return null;
            }

            string trimmed = idOrGid.Trim();
            return long.TryParse(trimmed, NumberStyles.None, CultureInfo.InvariantCulture, out long n)
                ? ToGid(resource, n)
                : trimmed;
        }

        // ------------------------------------------------------------------ strict, resource-aware

        /// <summary>
        /// Strictly parses gid://shopify/{Resource}/{id}[?query]: resource is one or more ASCII letters, id is 1..19 ASCII
        /// digits and fits a positive or zero <see cref="long"/>. No trim. False for anything else (a bare number, a
        /// trailing slash, "12abc", extra path segments).
        /// </summary>
        public static bool TryParse(string gid, out string resource, out long id)
        {
            resource = null;
            id = 0;
            if (string.IsNullOrEmpty(gid) || !gid.StartsWith(Scheme, StringComparison.Ordinal))
            {
                return false;
            }

            string rest = gid.Substring(Scheme.Length);
            int query = rest.IndexOf('?');
            if (query >= 0)
            {
                rest = rest.Substring(0, query);
            }

            int slash = rest.IndexOf('/');
            if (slash <= 0 || rest.IndexOf('/', slash + 1) >= 0)
            {
                return false;
            }

            string name = rest.Substring(0, slash);
            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];
                if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')))
                {
                    return false;
                }
            }

            string digits = rest.Substring(slash + 1);
            if (!IsAsciiDigits(digits, 19)
                || !long.TryParse(digits, NumberStyles.None, CultureInfo.InvariantCulture, out long parsed))
            {
                return false;
            }

            resource = name;
            id = parsed;
            return true;
        }

        /// <summary>
        /// The numeric id of a gid of EXACTLY <paramref name="resource"/> (ordinal, so "DraftOrder" never matches "Order"),
        /// per <see cref="TryParse"/>; otherwise null. A bare number gives null.
        /// </summary>
        public static long? ParseResourceIdOrNull(string gid, string resource) =>
            TryParse(gid, out string parsedResource, out long id)
                && string.Equals(parsedResource, resource, StringComparison.Ordinal)
                ? id
                : (long?)null;

        /// <summary><see cref="ParseResourceIdOrNull"/> rendered as an invariant string, or null.</summary>
        public static string ParseResourceIdStringOrNull(string gid, string resource)
        {
            long? id = ParseResourceIdOrNull(gid, resource);
            return id.HasValue ? id.Value.ToString(CultureInfo.InvariantCulture) : null;
        }

        /// <summary>
        /// Normalizes an id or gid of <paramref name="resource"/> to its gid: null/blank gives null; the value is trimmed;
        /// a value starting with gid://shopify/{resource}/ is returned when its tail is a positive number, a bare
        /// positive number gets the prefix, anything else (another resource, zero, negative, non-numeric, query string)
        /// gives null. "Positive number" = 1..19 ASCII digits, value &gt; 0.
        /// </summary>
        public static string NormalizePositive(string resource, string idOrGid)
        {
            if (string.IsNullOrWhiteSpace(idOrGid))
            {
                return null;
            }

            string prefix = Scheme + resource + "/";
            string value = idOrGid.Trim();
            if (value.StartsWith(prefix, StringComparison.Ordinal))
            {
                return IsPositiveNumber(value.Substring(prefix.Length)) ? value : null;
            }

            return IsPositiveNumber(value) ? prefix + value : null;
        }

        /// <summary>The numeric id of <see cref="NormalizePositive"/>, or null.</summary>
        public static long? ToPositiveNumericIdOrNull(string resource, string idOrGid)
        {
            string normalized = NormalizePositive(resource, idOrGid);
            if (normalized == null)
            {
                return null;
            }

            int prefixLength = Scheme.Length + resource.Length + 1;
            return long.Parse(normalized.Substring(prefixLength), CultureInfo.InvariantCulture);
        }

        // ------------------------------------------------------------------ tail parsers (legacy shapes)

        /// <summary>
        /// Text after the last '/' with any '?query' cut first. Null/empty input, no '/', or a trailing '/' give "".
        /// NOT validated ("gid://x/abc" gives "abc"); a bare "123" gives "".
        /// </summary>
        public static string ParseNumericId(string gid)
        {
            if (string.IsNullOrEmpty(gid))
            {
                return string.Empty;
            }

            string value = gid;
            int query = value.IndexOf('?');
            if (query >= 0)
            {
                value = value.Substring(0, query);
            }

            int slash = value.LastIndexOf('/');
            if (slash < 0 || slash == value.Length - 1)
            {
                return string.Empty;
            }

            return value.Substring(slash + 1);
        }

        /// <summary><see cref="ParseNumericId"/> with a blank result collapsed to null.</summary>
        public static string ParseNumericIdOrNull(string gid)
        {
            string s = ParseNumericId(gid);
            return string.IsNullOrWhiteSpace(s) ? null : s;
        }

        /// <summary>
        /// <see cref="ParseNumericId"/> parsed as digits only (<see cref="NumberStyles.None"/>, invariant); null otherwise.
        /// A bare "123" gives null (no slash).
        /// </summary>
        public static long? ParseNumericIdInt64OrNull(string gid) =>
            long.TryParse(ParseNumericId(gid), NumberStyles.None, CultureInfo.InvariantCulture, out long id)
                ? id
                : (long?)null;

        /// <summary>
        /// Lenient tail parse: null/blank gives null; the text after the last '/' (or the WHOLE input when there is no
        /// '/') is parsed with <see cref="NumberStyles.Integer"/> invariant (whitespace, sign allowed). The query string
        /// is NOT stripped ("…/123?x=1" gives null). A bare "123" gives 123.
        /// </summary>
        public static long? ParseTailInt64OrNull(string gidOrId)
        {
            if (string.IsNullOrWhiteSpace(gidOrId))
            {
                return null;
            }

            int slash = gidOrId.LastIndexOf('/');
            string tail = slash >= 0 ? gidOrId.Substring(slash + 1) : gidOrId;
            return long.TryParse(tail, NumberStyles.Integer, CultureInfo.InvariantCulture, out long id) ? id : (long?)null;
        }

        /// <summary><see cref="ParseTailInt64OrNull"/> of the trimmed input as a Try pattern; <paramref name="id"/> is 0 on failure.</summary>
        public static bool TryParseTailInt64(string gidOrId, out long id)
        {
            long? parsed = ParseTailInt64OrNull(gidOrId == null ? null : gidOrId.Trim());
            id = parsed.GetValueOrDefault();
            return parsed.HasValue;
        }

        /// <summary>
        /// Slash-required tail parse: 0 for null/empty, for input WITHOUT a '/' (a bare "123" gives 0), or when the text
        /// after the last '/' does not parse with <see cref="NumberStyles.Integer"/> invariant. No query strip.
        /// </summary>
        public static long ParseSlashTailInt64OrZero(string gid)
        {
            if (string.IsNullOrEmpty(gid))
            {
                return 0;
            }

            int slash = gid.LastIndexOf('/');
            return slash >= 0 && long.TryParse(gid.Substring(slash + 1), NumberStyles.Integer, CultureInfo.InvariantCulture, out long id)
                ? id
                : 0;
        }

        /// <summary>
        /// The text after the last '/' (the whole input when there is no '/'); null for null/empty input or an empty tail
        /// (trailing slash). No query strip, no validation.
        /// </summary>
        public static string TailOrNull(string gid)
        {
            if (string.IsNullOrEmpty(gid))
            {
                return null;
            }

            int slash = gid.LastIndexOf('/');
            string last = slash >= 0 ? gid.Substring(slash + 1) : gid;
            return last.Length > 0 ? last : null;
        }

        /// <summary><see cref="TailOrNull"/> when the tail is all digits (<see cref="char.IsDigit(char)"/>), else null.</summary>
        public static string DigitTailOrNull(string gid)
        {
            string tail = TailOrNull(gid);
            if (tail == null)
            {
                return null;
            }

            for (int i = 0; i < tail.Length; i++)
            {
                if (!char.IsDigit(tail[i]))
                {
                    return null;
                }
            }

            return tail;
        }

        private static bool IsPositiveNumber(string value) =>
            value.Length > 0
            && value.Length <= 19
            && long.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out long parsed)
            && parsed > 0;

        private static bool IsAsciiDigits(string value, int maxLength)
        {
            if (value.Length == 0 || value.Length > maxLength)
            {
                return false;
            }

            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] < '0' || value[i] > '9')
                {
                    return false;
                }
            }

            return true;
        }
    }
}
