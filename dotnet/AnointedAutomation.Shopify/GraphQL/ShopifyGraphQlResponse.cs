// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// The raw envelope of one Shopify Admin GraphQL answer: status, body, parsed root (data, errors, extensions.cost).

using System.Text.Json;

namespace AnointedAutomation.Shopify.GraphQL
{
    /// <summary>One GraphQL answer. <see cref="Root"/> is a detached clone, safe to keep.</summary>
    public sealed class ShopifyGraphQlResponse
    {
        /// <summary>Create a response from an HTTP status and a JSON body (throws <see cref="JsonException"/> when the
        /// body is not JSON).</summary>
        public ShopifyGraphQlResponse(int statusCode, string body)
        {
            StatusCode = statusCode;
            Body = body;
            using (JsonDocument doc = JsonDocument.Parse(body))
            {
                Root = doc.RootElement.Clone();
            }
        }

        /// <summary>The HTTP status code.</summary>
        public int StatusCode { get; }

        /// <summary>The raw body.</summary>
        public string Body { get; }

        /// <summary>The parsed root object.</summary>
        public JsonElement Root { get; }

        /// <summary>True when the root carries a data object.</summary>
        public bool HasData => TryGetData(out _);

        /// <summary>The data object, when present.</summary>
        public bool TryGetData(out JsonElement data)
        {
            data = default;
            return Root.ValueKind == JsonValueKind.Object
                && Root.TryGetProperty("data", out data) && data.ValueKind == JsonValueKind.Object;
        }

        /// <summary>Short description of the top-level errors, or null when there are none.</summary>
        public string ErrorSummary => ShopifyGraphQlErrors.Describe(Root);

        /// <summary>extensions.cost, or null when absent.</summary>
        public ShopifyQueryCost Cost => ShopifyQueryCost.Read(Root);
    }
}
