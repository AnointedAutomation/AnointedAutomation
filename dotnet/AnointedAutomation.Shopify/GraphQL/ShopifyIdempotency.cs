// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// The Admin GraphQL @idempotent directive (accepted from 2026-01, required on refund and inventory mutations from
// 2026-04). The key rides a variable so the query text stays constant and a resend carries the same key, which is
// what lets the client retry such a mutation after an unknown outcome without applying it twice.

using System;
using System.Security.Cryptography;
using System.Text;

namespace AnointedAutomation.Shopify.GraphQL
{
    /// <summary>The @idempotent directive pieces and key builders for Shopify Admin mutations.</summary>
    public static class ShopifyIdempotency
    {
        /// <summary>Variable declaration to add to the mutation's variable list.</summary>
        public const string KeyDeclaration = "$idempotencyKey: String!";

        /// <summary>The directive placed right after the mutation field's arguments.</summary>
        public const string Directive = "@idempotent(key: $idempotencyKey)";

        /// <summary>The variables property carrying the key.</summary>
        public const string KeyVariable = "idempotencyKey";

        /// <summary>A fresh random key: one per logical operation.</summary>
        public static string NewKey() => Guid.NewGuid().ToString("D");

        /// <summary>A deterministic key from an operation name and its exact parameters: the same request always maps
        /// to the same key, and different parameters never share one.</summary>
        public static string KeyFor(string operation, string parameters)
        {
            if (string.IsNullOrWhiteSpace(operation))
            {
                throw new ArgumentException("An operation name is required.", nameof(operation));
            }
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(operation + "|" + parameters));
            byte[] guidBytes = new byte[16];
            Array.Copy(hash, guidBytes, 16);
            return new Guid(guidBytes).ToString("D");
        }

        /// <summary>True when the document uses the @idempotent directive.</summary>
        public static bool UsesDirective(string query) =>
            query != null && query.Contains("@idempotent", StringComparison.Ordinal);
    }
}
