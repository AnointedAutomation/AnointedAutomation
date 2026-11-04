// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using System;

namespace AnointedAutomation.Shopify.GraphQL
{
    /// <summary>Whether a failed call could have changed the store.</summary>
    public enum ShopifyCallOutcome
    {
        /// <summary>Not set.</summary>
        Unknown = 0,

        /// <summary>Shopify answered and the call did not apply (4xx, top-level errors, userErrors, retries exhausted on
        /// a safe call).</summary>
        Failed = 1,

        /// <summary>A mutation without an idempotency key met a 5xx or a network failure: it may or may not have applied.
        /// It was NOT resent. Reconcile by reading the store before acting again.</summary>
        Indeterminate = 2,
    }

    /// <summary>A Shopify GraphQL call that failed. Carries no body and no token.</summary>
    public sealed class ShopifyGraphQlException : Exception
    {
        /// <summary>Create the exception.</summary>
        public ShopifyGraphQlException(string operation, int statusCode, ShopifyCallOutcome outcome, string reason, Exception inner)
            : base("Shopify GraphQL " + operation + " failed (" + outcome + ", HTTP " + statusCode + "): " + reason, inner)
        {
            Operation = operation;
            StatusCode = statusCode;
            Outcome = outcome;
            Reason = reason;
        }

        /// <summary>The operation name.</summary>
        public string Operation { get; }

        /// <summary>The last HTTP status, 0 when no answer arrived.</summary>
        public int StatusCode { get; }

        /// <summary>Failed, or Indeterminate for an unkeyed mutation that may have applied.</summary>
        public ShopifyCallOutcome Outcome { get; }

        /// <summary>Short reason (error messages and codes only).</summary>
        public string Reason { get; }
    }
}
