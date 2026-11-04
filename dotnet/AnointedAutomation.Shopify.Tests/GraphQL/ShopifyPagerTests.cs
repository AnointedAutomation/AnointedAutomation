// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.Shopify.GraphQL;
using Xunit;

namespace AnointedAutomation.Shopify.Tests.GraphQL
{
    public class ShopifyPagerTests
    {
        private const string PageQuery = "query Orders($first: Int!, $after: String) { orders(first: $first, after: $after) { nodes { id } pageInfo { hasNextPage endCursor } } }";

        private static JsonElement Parse(string json)
        {
            using JsonDocument d = JsonDocument.Parse(json);
            return d.RootElement.Clone();
        }

        private static async Task<List<string>> Collect(ShopifyGraphQlClient c, int pageSize, IReadOnlyDictionary<string, object> vars = null)
        {
            List<string> ids = new List<string>();
            await foreach (JsonElement n in ShopifyPager.EnumerateNodesAsync(c, PageQuery, vars, d => d.GetProperty("orders"), pageSize, CancellationToken.None))
            {
                ids.Add(n.GetProperty("id").GetString());
            }
            return ids;
        }

        [Fact]
        public async Task MultiPageWalk_PassesCursorAndSize()
        {
            Harness h = new Harness();
            h.Handler.Json(HttpStatusCode.OK, "{\"data\":{\"orders\":{\"nodes\":[{\"id\":\"1\"},{\"id\":\"2\"}],\"pageInfo\":{\"hasNextPage\":true,\"endCursor\":\"c1\"}}}}")
                .Json(HttpStatusCode.OK, "{\"data\":{\"orders\":{\"edges\":[{\"node\":{\"id\":\"3\"}},{}],\"pageInfo\":{\"hasNextPage\":false,\"endCursor\":\"c2\"}}}}");
            List<string> ids = await Collect(h.Client(), 2, new Dictionary<string, object> { ["q"] = "x" });
            Assert.Equal(new[] { "1", "2", "3" }, ids);
            using JsonDocument first = JsonDocument.Parse(h.Handler.Requests[0].Body);
            Assert.Equal(2, first.RootElement.GetProperty("variables").GetProperty("first").GetInt32());
            Assert.Equal(JsonValueKind.Null, first.RootElement.GetProperty("variables").GetProperty("after").ValueKind);
            Assert.Equal("x", first.RootElement.GetProperty("variables").GetProperty("q").GetString());
            using JsonDocument second = JsonDocument.Parse(h.Handler.Requests[1].Body);
            Assert.Equal("c1", second.RootElement.GetProperty("variables").GetProperty("after").GetString());
        }

        [Fact]
        public async Task EmptyPage_EndsWalk()
        {
            Harness h = new Harness();
            h.Handler.Json(HttpStatusCode.OK, "{\"data\":{\"orders\":{\"nodes\":[],\"pageInfo\":{\"hasNextPage\":false,\"endCursor\":null}}}}");
            Assert.Empty(await Collect(h.Client(), 250));
            Assert.Single(h.Handler.Requests);
        }

        [Fact]
        public async Task MissingData_OrPageInfo_Throws()
        {
            Harness h = new Harness();
            h.Handler.Json(HttpStatusCode.OK, "{\"data\":{\"orders\":{\"nodes\":[{\"id\":\"1\"}]}}}");
            await Assert.ThrowsAsync<InvalidOperationException>(() => Collect(h.Client(), 10));
            Harness h2 = new Harness();
            h2.Handler.Json(HttpStatusCode.OK, "{\"data\":null}");
            await Assert.ThrowsAsync<InvalidOperationException>(() => Collect(h2.Client(), 10));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(251)]
        public async Task InvalidPageSize_Throws(int size)
        {
            Harness h = new Harness();
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => Collect(h.Client(), size));
            Assert.Empty(h.Handler.Requests);
        }

        [Fact]
        public async Task NullArguments_Throw()
        {
            Harness h = new Harness();
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await foreach (JsonElement _ in ShopifyPager.EnumerateNodesAsync(null, PageQuery, null, d => d, 1, CancellationToken.None)) { }
            });
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await foreach (JsonElement _ in ShopifyPager.EnumerateNodesAsync(h.Client(), PageQuery, null, null, 1, CancellationToken.None)) { }
            });
        }

        [Fact]
        public void ReadPageInfo_Variants()
        {
            Assert.Equal(new ShopifyPageInfo(true, "c"), ShopifyPager.ReadPageInfo(Parse("{\"pageInfo\":{\"hasNextPage\":true,\"endCursor\":\"c\"}}")));
            Assert.Equal(new ShopifyPageInfo(false, null), ShopifyPager.ReadPageInfo(Parse("{\"pageInfo\":{\"hasNextPage\":false}}")));
            Assert.Throws<InvalidOperationException>(() => ShopifyPager.ReadPageInfo(Parse("{\"pageInfo\":{\"hasNextPage\":true}}")));
            Assert.Throws<InvalidOperationException>(() => ShopifyPager.ReadPageInfo(Parse("{\"pageInfo\":{\"hasNextPage\":\"yes\"}}")));
            Assert.Throws<InvalidOperationException>(() => ShopifyPager.ReadPageInfo(Parse("[]")));
            Assert.False(ShopifyPageInfo.Last.HasMore);
            Assert.Empty(ShopifyPager.ReadNodes(Parse("[]")));
            Assert.Empty(ShopifyPager.ReadNodes(Parse("{}")));
            ShopifyPager.ValidatePageSize(1);
            ShopifyPager.ValidatePageSize(ShopifyPager.MaxPageSize);
        }

        [Fact]
        public async Task WalkAsync_FollowsCursorAndStopsAtMax()
        {
            List<string> cursors = new List<string>();
            int pages = await ShopifyPager.WalkAsync(c => { cursors.Add(c); return Task.FromResult(new ShopifyPageInfo(true, "n" + cursors.Count)); }, 3);
            Assert.Equal(3, pages);
            Assert.Equal(new[] { null, "n1", "n2" }, cursors);
            Assert.Equal(1, await ShopifyPager.WalkAsync(_ => Task.FromResult(ShopifyPageInfo.Last), 5));
            await Assert.ThrowsAsync<ArgumentNullException>(() => ShopifyPager.WalkAsync(null, 1));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => ShopifyPager.WalkAsync(_ => Task.FromResult(ShopifyPageInfo.Last), 0));
        }
    }
}
