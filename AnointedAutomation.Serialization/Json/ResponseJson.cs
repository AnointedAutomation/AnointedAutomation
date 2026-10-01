// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Moved from the Anointed API (Services/Common/ResponseJson.cs).
// Safe parsing of a third-party HTTP response body.
//
// WHY THIS EXISTS. A response body is NOT guaranteed to be JSON, no matter what the API's docs say.
// A gateway under load answers with "Service Unavailable", a proxy answers with an HTML error page,
// and a throttled call can come back empty. Every one of those makes a bare JsonDocument.Parse throw
// a JsonException whose message ("'S' is an invalid start of a value") tells the reader nothing about
// which call failed or why.
//
// This cost real money on 2026-08-26/27: the shopify API queue was starving callers, Shopify bodies
// came back non-JSON, and SupplierStockoutZeroService parsed them unguarded. The exception escaped
// per-order processing, so the supplier-cancel sweep logged 5,224 identical errors and ABANDONED the
// cancellation work for those orders. The parse failing is not the bug; the parse failing loudly in
// the middle of a per-item loop is.

using System;
using System.Text.Json;

namespace AnointedAutomation.Serialization.Json
{
    /// <summary>Parse helpers for bodies that are only USUALLY JSON.</summary>
    public static class ResponseJson
    {
        /// <summary>How much of a non-JSON body is worth quoting in a log line.</summary>
        public const int SnippetLength = 200;

        /// <summary>
        /// Parse a response body, or return null when it is missing, blank, or not JSON at all.
        /// The caller owns the returned document.
        /// </summary>
        public static JsonDocument TryParse(string body)
        {
            if (string.IsNullOrWhiteSpace(body))
            {
                return null;
            }
            try
            {
                return JsonDocument.Parse(body);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        /// <summary>
        /// The leading part of a body, single-lined, for a log line that has to say WHAT came back.
        /// "(empty response)" when there was no body at all: an empty body and a body full of HTML
        /// are different failures and must not read the same.
        /// </summary>
        public static string Snippet(string body)
        {
            if (string.IsNullOrWhiteSpace(body))
            {
                return "(empty response)";
            }
            string flat = body.Replace('\n', ' ').Replace('\r', ' ').Trim();
            return flat.Length <= SnippetLength ? flat : flat.Substring(0, SnippetLength) + "...";
        }

        /// <summary>
        /// True when the body is not JSON, i.e. an upstream error page/plain-text failure rather than
        /// a payload worth inspecting. Convenience for the common "log it and move on" branch.
        /// </summary>
        public static bool IsNotJson(string body)
        {
            using JsonDocument parsed = TryParse(body);
            return parsed is null;
        }
    }
}
