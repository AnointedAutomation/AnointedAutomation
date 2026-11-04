// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// Cursor pagination over Shopify GraphQL connections (pageInfo { hasNextPage endCursor }). The query declares
// $first: Int! and $after: String; the pager sets both and walks until hasNextPage is false.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AnointedAutomation.Shopify.GraphQL
{
    /// <summary>A connection's pageInfo.</summary>
    public readonly record struct ShopifyPageInfo(bool HasNextPage, string EndCursor)
    {
        /// <summary>The last page.</summary>
        public static ShopifyPageInfo Last => new ShopifyPageInfo(false, null);

        /// <summary>True when there is another page AND a cursor to ask for it with.</summary>
        public bool HasMore => HasNextPage && !string.IsNullOrEmpty(EndCursor);
    }

    /// <summary>Cursor pagination helpers.</summary>
    public static class ShopifyPager
    {
        /// <summary>Shopify's maximum page size for a connection (first:).</summary>
        public const int MaxPageSize = 250;

        /// <summary>Variable name for the page size.</summary>
        public const string FirstVariable = "first";

        /// <summary>Variable name for the cursor.</summary>
        public const string AfterVariable = "after";

        /// <summary>Throws unless 1 &lt;= <paramref name="pageSize"/> &lt;= <see cref="MaxPageSize"/>.</summary>
        public static void ValidatePageSize(int pageSize)
        {
            if (pageSize < 1 || pageSize > MaxPageSize)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), pageSize, "Page size must be between 1 and " + MaxPageSize + ".");
            }
        }

        /// <summary>Read pageInfo off a connection. Missing or malformed pageInfo throws <see cref="InvalidOperationException"/>
        /// (a malformed page never silently ends a walk).</summary>
        public static ShopifyPageInfo ReadPageInfo(JsonElement connection)
        {
            if (connection.ValueKind != JsonValueKind.Object
                || !connection.TryGetProperty("pageInfo", out JsonElement pageInfo) || pageInfo.ValueKind != JsonValueKind.Object
                || !pageInfo.TryGetProperty("hasNextPage", out JsonElement hasNext)
                || (hasNext.ValueKind != JsonValueKind.True && hasNext.ValueKind != JsonValueKind.False))
            {
                throw new InvalidOperationException("The connection has no pageInfo { hasNextPage } object; select pageInfo { hasNextPage endCursor }.");
            }
            string cursor = pageInfo.TryGetProperty("endCursor", out JsonElement ec) && ec.ValueKind == JsonValueKind.String
                ? ec.GetString()
                : null;
            bool more = hasNext.ValueKind == JsonValueKind.True;
            if (more && string.IsNullOrEmpty(cursor))
            {
                throw new InvalidOperationException("pageInfo.hasNextPage is true but endCursor is missing; select pageInfo { hasNextPage endCursor }.");
            }
            return new ShopifyPageInfo(more, cursor);
        }

        /// <summary>The items of one connection page: nodes[], else edges[].node.</summary>
        public static IReadOnlyList<JsonElement> ReadNodes(JsonElement connection)
        {
            List<JsonElement> items = new List<JsonElement>();
            if (connection.ValueKind != JsonValueKind.Object)
            {
                return items;
            }
            if (connection.TryGetProperty("nodes", out JsonElement nodes) && nodes.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement n in nodes.EnumerateArray())
                {
                    items.Add(n);
                }
                return items;
            }
            if (connection.TryGetProperty("edges", out JsonElement edges) && edges.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement e in edges.EnumerateArray())
                {
                    if (e.ValueKind == JsonValueKind.Object && e.TryGetProperty("node", out JsonElement node))
                    {
                        items.Add(node);
                    }
                }
            }
            return items;
        }

        /// <summary>
        /// Walk a connection: <paramref name="fetchPage"/> is called with a null cursor, then with each endCursor while
        /// <see cref="ShopifyPageInfo.HasMore"/>, up to <paramref name="maxPages"/>. Returns the number of pages fetched.
        /// </summary>
        public static async Task<int> WalkAsync(Func<string, Task<ShopifyPageInfo>> fetchPage, int maxPages)
        {
            if (fetchPage == null)
            {
                throw new ArgumentNullException(nameof(fetchPage));
            }
            if (maxPages < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxPages), maxPages, "at least one page");
            }
            string cursor = null;
            int pages = 0;
            while (true)
            {
                ShopifyPageInfo page = await fetchPage(cursor).ConfigureAwait(false);
                pages++;
                if (!page.HasMore || pages >= maxPages)
                {
                    return pages;
                }
                cursor = page.EndCursor;
            }
        }

        /// <summary>
        /// Every node of a connection, page by page, through <see cref="ShopifyGraphQlClient.QueryAsync"/>. The query must
        /// declare $first: Int! and $after: String. <paramref name="selectConnection"/> picks the connection out of
        /// data (e.g. data =&gt; data.GetProperty("orders")). Extra <paramref name="variables"/> may be null.
        /// </summary>
        public static async IAsyncEnumerable<JsonElement> EnumerateNodesAsync(ShopifyGraphQlClient client, string query,
            IReadOnlyDictionary<string, object> variables, Func<JsonElement, JsonElement> selectConnection, int pageSize,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (selectConnection == null)
            {
                throw new ArgumentNullException(nameof(selectConnection));
            }
            ValidatePageSize(pageSize);
            string cursor = null;
            while (true)
            {
                Dictionary<string, object> vars = new Dictionary<string, object>(StringComparer.Ordinal);
                if (variables != null)
                {
                    foreach (KeyValuePair<string, object> pair in variables)
                    {
                        vars[pair.Key] = pair.Value;
                    }
                }
                vars[FirstVariable] = pageSize;
                vars[AfterVariable] = cursor;
                ShopifyGraphQlResponse response = await client.QueryAsync(new ShopifyGraphQlRequest(query, vars), cancellationToken).ConfigureAwait(false);
                if (!response.TryGetData(out JsonElement data))
                {
                    throw new InvalidOperationException("Shopify answered the page query without a data object.");
                }
                JsonElement connection = selectConnection(data);
                foreach (JsonElement node in ReadNodes(connection))
                {
                    yield return node;
                }
                ShopifyPageInfo page = ReadPageInfo(connection);
                if (!page.HasMore)
                {
                    yield break;
                }
                cursor = page.EndCursor;
            }
        }
    }
}
