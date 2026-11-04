// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// Readers over a Shopify Admin GraphQL answer's failure channels: top-level "errors" (THROTTLED, ACCESS_DENIED, a bad
// query; Shopify answers those with HTTP 200 and no data) and a mutation payload's "userErrors". Only messages and
// codes are kept, never the body.

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AnointedAutomation.Shopify.GraphQL
{
    /// <summary>Error readers over a parsed GraphQL root.</summary>
    public static class ShopifyGraphQlErrors
    {
        /// <summary>The error code Shopify returns for the query-cost throttle.</summary>
        public const string ThrottledCode = "THROTTLED";

        /// <summary>Longest description returned.</summary>
        public const int MaxLength = 300;

        private static readonly Regex NamedOperation = new Regex(
            @"\b(?:query|mutation)\s+([A-Za-z_][A-Za-z0-9_]*)", RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));

        private static readonly Regex FirstField = new Regex(
            @"\{\s*([A-Za-z_][A-Za-z0-9_]*)", RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));

        /// <summary>The operation name of a document ("query OrdersPage" = OrdersPage), else its first field.</summary>
        public static string OperationName(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return "(empty query)";
            }
            Match named = NamedOperation.Match(query);
            if (named.Success)
            {
                return named.Groups[1].Value;
            }
            Match field = FirstField.Match(query);
            return field.Success ? field.Groups[1].Value : "(unnamed)";
        }

        /// <summary>The top-level errors as "CODE: message; ..." (capped), or null when there are none.</summary>
        public static string Describe(JsonElement root)
        {
            if (root.ValueKind != JsonValueKind.Object || !root.TryGetProperty("errors", out JsonElement errors))
            {
                return null;
            }
            if (errors.ValueKind == JsonValueKind.String)
            {
                string text = errors.GetString();
                return string.IsNullOrWhiteSpace(text) ? null : Cap(text);
            }
            if (errors.ValueKind != JsonValueKind.Array || errors.GetArrayLength() == 0)
            {
                return null;
            }
            List<string> parts = new List<string>();
            foreach (JsonElement e in errors.EnumerateArray())
            {
                if (e.ValueKind != JsonValueKind.Object)
                {
                    continue;
                }
                string message = e.TryGetProperty("message", out JsonElement m) && m.ValueKind == JsonValueKind.String
                    && !string.IsNullOrEmpty(m.GetString()) ? m.GetString() : "error";
                string code = ErrorCode(e);
                parts.Add(string.IsNullOrEmpty(code) ? message : code + ": " + message);
            }
            return Cap(parts.Count == 0 ? "errors" : string.Join("; ", parts));
        }

        /// <summary>True when a top-level errors[] entry has extensions.code equal to <paramref name="code"/>.</summary>
        public static bool HasErrorCode(JsonElement root, string code)
        {
            if (root.ValueKind != JsonValueKind.Object || !root.TryGetProperty("errors", out JsonElement errors)
                || errors.ValueKind != JsonValueKind.Array)
            {
                return false;
            }
            foreach (JsonElement e in errors.EnumerateArray())
            {
                if (string.Equals(ErrorCode(e), code, StringComparison.Ordinal))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>True when the answer is a THROTTLED error.</summary>
        public static bool IsThrottled(JsonElement root) => HasErrorCode(root, ThrottledCode);

        /// <summary>data.&lt;field&gt;.userErrors[].message, in order (entries without a string message skipped).
        /// Empty when the chain is missing.</summary>
        public static IReadOnlyList<string> ReadUserErrors(JsonElement root, string mutationField)
        {
            List<string> messages = new List<string>();
            if (!TryGetPayload(root, mutationField, out JsonElement payload)
                || !payload.TryGetProperty("userErrors", out JsonElement userErrors)
                || userErrors.ValueKind != JsonValueKind.Array)
            {
                return messages;
            }
            foreach (JsonElement e in userErrors.EnumerateArray())
            {
                if (e.ValueKind == JsonValueKind.Object && e.TryGetProperty("message", out JsonElement m)
                    && m.ValueKind == JsonValueKind.String)
                {
                    messages.Add(m.GetString());
                }
            }
            return messages;
        }

        /// <summary>data.&lt;field&gt; when it is an object.</summary>
        public static bool TryGetPayload(JsonElement root, string mutationField, out JsonElement payload)
        {
            if (string.IsNullOrWhiteSpace(mutationField))
            {
                throw new ArgumentException("A mutation field is required.", nameof(mutationField));
            }
            payload = default;
            return root.ValueKind == JsonValueKind.Object
                && root.TryGetProperty("data", out JsonElement data) && data.ValueKind == JsonValueKind.Object
                && data.TryGetProperty(mutationField, out payload) && payload.ValueKind == JsonValueKind.Object;
        }

        /// <summary>Why the mutation did not run, or null when it ran: top-level errors first, then the first userError,
        /// then a missing data.&lt;field&gt; payload.</summary>
        public static string FirstMutationError(JsonElement root, string mutationField)
        {
            string top = Describe(root);
            if (top != null)
            {
                return top;
            }
            IReadOnlyList<string> userErrors = ReadUserErrors(root, mutationField);
            if (userErrors.Count > 0)
            {
                return Cap(string.Join(", ", userErrors));
            }
            return TryGetPayload(root, mutationField, out _) ? null : "no " + mutationField + " result returned";
        }

        private static string ErrorCode(JsonElement e) =>
            e.ValueKind == JsonValueKind.Object
            && e.TryGetProperty("extensions", out JsonElement ext) && ext.ValueKind == JsonValueKind.Object
            && ext.TryGetProperty("code", out JsonElement c) && c.ValueKind == JsonValueKind.String
                ? c.GetString()
                : null;

        internal static string Cap(string text) => text.Length <= MaxLength ? text : text.Substring(0, MaxLength);
    }
}
