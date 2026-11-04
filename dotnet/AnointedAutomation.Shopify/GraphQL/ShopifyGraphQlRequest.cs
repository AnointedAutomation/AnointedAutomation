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
            IsMutation = MayBeMutation(query);
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

        /// <summary>
        /// True when any executable operation in <paramref name="query"/> is a mutation, or when the operation type
        /// cannot be determined (fail closed: such a request is never resent after an unknown outcome). Comments,
        /// string literals (including block strings) and fragment definitions are skipped; a shorthand
        /// <c>{ ... }</c> document is a query.
        /// </summary>
        public static bool MayBeMutation(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return true;
            }
            int depth = 0;
            bool sawOperation = false;
            bool inHeader = false; // between a definition keyword and its selection set: names, variables, directives
            int i = 0;
            int n = query.Length;
            while (i < n)
            {
                char c = query[i];
                if (c == '#')
                {
                    while (i < n && query[i] != '\n' && query[i] != '\r')
                    {
                        i++;
                    }
                    continue;
                }
                if (c == '"')
                {
                    if (i + 2 < n && query[i + 1] == '"' && query[i + 2] == '"')
                    {
                        int end = query.IndexOf("\"\"\"", i + 3, StringComparison.Ordinal);
                        if (end < 0)
                        {
                            return true;
                        }
                        i = end + 3;
                        continue;
                    }
                    i++;
                    while (i < n && query[i] != '"')
                    {
                        i += query[i] == '\\' ? 2 : 1;
                    }
                    if (i >= n)
                    {
                        return true;
                    }
                    i++;
                    continue;
                }
                if (c == '{')
                {
                    if (depth == 0)
                    {
                        sawOperation |= !inHeader; // a bare top-level selection set is a shorthand query
                        inHeader = false;
                    }
                    depth++;
                    i++;
                    continue;
                }
                if (c == '}')
                {
                    depth--;
                    if (depth < 0)
                    {
                        return true;
                    }
                    i++;
                    continue;
                }
                if (depth == 0 && (char.IsLetter(c) || c == '_'))
                {
                    int start = i;
                    while (i < n && (char.IsLetterOrDigit(query[i]) || query[i] == '_'))
                    {
                        i++;
                    }
                    if (inHeader)
                    {
                        continue;
                    }
                    string word = query.Substring(start, i - start);
                    if (string.Equals(word, "mutation", StringComparison.Ordinal))
                    {
                        return true;
                    }
                    if (string.Equals(word, "fragment", StringComparison.Ordinal))
                    {
                        inHeader = true;
                        continue;
                    }
                    if (!string.Equals(word, "query", StringComparison.Ordinal) && !string.Equals(word, "subscription", StringComparison.Ordinal))
                    {
                        return true; // unknown top-level definition: fail closed
                    }
                    sawOperation = true;
                    inHeader = true;
                    continue;
                }
                if (depth == 0 && c == '(')
                {
                    int close = SkipParens(query, i);
                    if (close < 0)
                    {
                        return true;
                    }
                    i = close + 1;
                    continue;
                }
                i++;
            }
            return depth != 0 || inHeader || !sawOperation;
        }

        private static int SkipParens(string query, int open)
        {
            int nest = 0;
            for (int i = open; i < query.Length; i++)
            {
                char c = query[i];
                if (c == '"')
                {
                    i++;
                    while (i < query.Length && query[i] != '"')
                    {
                        i += query[i] == '\\' ? 2 : 1;
                    }
                    continue;
                }
                if (c == '#')
                {
                    while (i < query.Length && query[i] != '\n')
                    {
                        i++;
                    }
                    continue;
                }
                if (c == '(')
                {
                    nest++;
                }
                else if (c == ')' && --nest == 0)
                {
                    return i;
                }
            }
            return -1;
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
