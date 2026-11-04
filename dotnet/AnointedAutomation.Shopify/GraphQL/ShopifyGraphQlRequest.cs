// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// One GraphQL document plus its variables. Knows whether it is a mutation and whether that mutation carries an
// @idempotent key, which is what the retry policy needs to decide if a resend is safe.

using System;
using System.Collections.Generic;
using System.Text.Json;

namespace AnointedAutomation.Shopify.GraphQL
{
    /// <summary>A Shopify Admin GraphQL request.</summary>
    public sealed class ShopifyGraphQlRequest
    {
        /// <summary>Create a request. <paramref name="variables"/> may be null (no variables).</summary>
        public ShopifyGraphQlRequest(string query, IReadOnlyDictionary<string, object> variables)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException("A GraphQL query is required.", nameof(query));
            }
            Query = query;
            Variables = variables;
            OperationName = ShopifyGraphQlErrors.OperationName(query);
            IsMutation = query.TrimStart().StartsWith("mutation", StringComparison.Ordinal);
            IsIdempotent = IsMutation && ShopifyIdempotency.UsesDirective(query) && HasKey(variables);
        }

        /// <summary>The GraphQL document.</summary>
        public string Query { get; }

        /// <summary>The variables, or null.</summary>
        public IReadOnlyDictionary<string, object> Variables { get; }

        /// <summary>The operation name ("mutation RefundCreate" = RefundCreate), else the first field.</summary>
        public string OperationName { get; }

        /// <summary>True for a mutation document.</summary>
        public bool IsMutation { get; }

        /// <summary>True for a mutation that uses @idempotent AND passes a non-empty key variable: only such a mutation
        /// is resent after a 5xx or a network failure.</summary>
        public bool IsIdempotent { get; }

        /// <summary>True when a resend after an unknown outcome (5xx, network failure) cannot apply twice.</summary>
        public bool SafeToResendAfterUnknownOutcome => !IsMutation || IsIdempotent;

        /// <summary>The JSON body POSTed to graphql.json.</summary>
        public string ToJson()
        {
            Dictionary<string, object> body = new Dictionary<string, object> { ["query"] = Query };
            if (Variables != null)
            {
                body["variables"] = Variables;
            }
            return JsonSerializer.Serialize(body);
        }

        private static bool HasKey(IReadOnlyDictionary<string, object> variables)
        {
            if (variables == null || !variables.TryGetValue(ShopifyIdempotency.KeyVariable, out object key) || key == null)
            {
                return false;
            }
            return !string.IsNullOrWhiteSpace(key.ToString());
        }
    }
}
