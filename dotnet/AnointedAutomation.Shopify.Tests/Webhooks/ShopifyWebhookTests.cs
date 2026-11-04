// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.Shopify.GraphQL;
using AnointedAutomation.Shopify.Tests.GraphQL;
using AnointedAutomation.Shopify.Webhooks;
using Microsoft.Extensions.Logging;
using Xunit;

namespace AnointedAutomation.Shopify.Tests.Webhooks
{
    public class ShopifyWebhookHmacTests
    {
        private const string Secret = "test_secret";
        private static readonly byte[] Body = Encoding.UTF8.GetBytes("{\"id\":1,\"note\":\"café\"}");

        private static string Sign(byte[] body) => Convert.ToBase64String(HMACSHA256.HashData(Encoding.UTF8.GetBytes(Secret), body));

        [Fact]
        public void Valid_Signature_Verifies()
        {
            string header = Sign(Body);
            Assert.Equal(header, ShopifyWebhookHmac.Compute(Body, Secret));
            Assert.True(ShopifyWebhookHmac.Verify(Body, header, Secret));
            Assert.True(ShopifyWebhookHmac.Verify(Body, " " + header + " ", Secret));
            Assert.True(ShopifyWebhookHmac.Verify(Encoding.UTF8.GetString(Body), header, Secret));
        }

        [Fact]
        public void Invalid_Signatures_Fail()
        {
            string header = Sign(Body);
            Assert.False(ShopifyWebhookHmac.Verify(Encoding.UTF8.GetBytes("{\"id\":2}"), header, Secret));
            Assert.False(ShopifyWebhookHmac.Verify(Body, header, "other_secret"));
            Assert.False(ShopifyWebhookHmac.Verify(Body, "not base64 !!", Secret));
            Assert.False(ShopifyWebhookHmac.Verify(Body, Convert.ToBase64String(new byte[] { 1, 2, 3 }), Secret));
            char[] flipped = header.ToCharArray();
            flipped[0] = flipped[0] == 'A' ? 'B' : 'A';
            Assert.False(ShopifyWebhookHmac.Verify(Body, new string(flipped), Secret));
        }

        [Fact]
        public void Missing_Header_Fails_MissingSecretOrBody_Throws()
        {
            Assert.False(ShopifyWebhookHmac.Verify(Body, null, Secret));
            Assert.False(ShopifyWebhookHmac.Verify(Body, "  ", Secret));
            Assert.Throws<ArgumentException>(() => ShopifyWebhookHmac.Verify(Body, Sign(Body), ""));
            Assert.Throws<ArgumentNullException>(() => ShopifyWebhookHmac.Verify((byte[])null, "x", Secret));
            Assert.Throws<ArgumentNullException>(() => ShopifyWebhookHmac.Verify((string)null, "x", Secret));
            Assert.Throws<ArgumentException>(() => ShopifyWebhookHmac.Compute(Body, null));
        }
    }

    public class ShopifyWebhookRegistrarTests
    {
        private const string Prefix = "https://hooks.example.com/shopify/";

        [Fact]
        public void Diff_CreatesMissing_KeepsExisting_DeletesStaleUnderPrefixOnly()
        {
            List<ShopifyWebhookTarget> wanted = new List<ShopifyWebhookTarget>
            {
                new ShopifyWebhookTarget("ORDERS_CREATE", Prefix + "orders"),
                new ShopifyWebhookTarget("PRODUCTS_UPDATE", Prefix + "products"),
                new ShopifyWebhookTarget("PRODUCTS_UPDATE", Prefix + "products"),
            };
            List<ShopifyWebhookSubscription> existing = new List<ShopifyWebhookSubscription>
            {
                new ShopifyWebhookSubscription("1", "ORDERS_CREATE", Prefix + "orders"),
                new ShopifyWebhookSubscription("2", "ORDERS_CREATE", Prefix + "orders"),
                new ShopifyWebhookSubscription("3", "CUSTOMERS_UPDATE", Prefix + "customers"),
                new ShopifyWebhookSubscription("4", "CUSTOMERS_UPDATE", "https://someone-else.example/x"),
                new ShopifyWebhookSubscription("5", "ORDERS_CREATE", null),
            };

            ShopifyWebhookPlan plan = ShopifyWebhookRegistrar.Diff(wanted, existing, Prefix);

            Assert.Equal(new[] { "1" }, plan.Keep.Select(s => s.Id));
            Assert.Equal(new[] { "PRODUCTS_UPDATE" }, plan.Create.Select(t => t.Topic));
            Assert.Equal(new[] { "2", "3" }, plan.Delete.Select(s => s.Id));
        }

        [Fact]
        public void Diff_RequiresPrefixAndValidTargets()
        {
            Assert.Throws<ArgumentException>(() => ShopifyWebhookRegistrar.Diff(new ShopifyWebhookTarget[0], new ShopifyWebhookSubscription[0], " "));
            Assert.Throws<ArgumentException>(() => ShopifyWebhookRegistrar.Diff(new[] { new ShopifyWebhookTarget("", "u") }, new ShopifyWebhookSubscription[0], Prefix));
            Assert.Throws<ArgumentException>(() => ShopifyWebhookRegistrar.Diff(new[] { new ShopifyWebhookTarget("T", "") }, new ShopifyWebhookSubscription[0], Prefix));
            Assert.Throws<ArgumentNullException>(() => ShopifyWebhookRegistrar.Diff(null, new ShopifyWebhookSubscription[0], Prefix));
            Assert.Throws<ArgumentNullException>(() => ShopifyWebhookRegistrar.Diff(new ShopifyWebhookTarget[0], null, Prefix));
        }

        [Fact]
        public async Task Sync_ListsPages_CreatesAndDeletes_ReportsFailures()
        {
            Harness h = new Harness();
            h.Handler
                .Json(HttpStatusCode.OK, "{\"data\":{\"webhookSubscriptions\":{\"nodes\":[{\"id\":\"1\",\"topic\":\"ORDERS_CREATE\",\"uri\":\"" + Prefix + "orders\"}],\"pageInfo\":{\"hasNextPage\":true,\"endCursor\":\"c1\"}}}}")
                .Json(HttpStatusCode.OK, "{\"data\":{\"webhookSubscriptions\":{\"nodes\":[{\"id\":\"9\",\"topic\":\"SHOP_UPDATE\",\"uri\":\"" + Prefix + "shop\"}],\"pageInfo\":{\"hasNextPage\":false,\"endCursor\":null}}}}")
                .Json(HttpStatusCode.OK, "{\"data\":{\"webhookSubscriptionCreate\":{\"webhookSubscription\":{\"id\":\"10\",\"topic\":\"PRODUCTS_UPDATE\",\"uri\":\"u\"},\"userErrors\":[]}}}")
                .Json(HttpStatusCode.OK, "{\"data\":{\"webhookSubscriptionCreate\":{\"webhookSubscription\":null,\"userErrors\":[{\"field\":[\"topic\"],\"message\":\"Access denied for topic\"}]}}}")
                .Json(HttpStatusCode.OK, "{\"data\":{\"webhookSubscriptionDelete\":{\"deletedWebhookSubscriptionId\":\"9\",\"userErrors\":[]}}}");
            ShopifyWebhookRegistrar registrar = new ShopifyWebhookRegistrar(h.Client(), h.Logger);

            IReadOnlyList<ShopifyWebhookOutcome> outcomes = await registrar.SyncAsync(new[]
            {
                new ShopifyWebhookTarget("ORDERS_CREATE", Prefix + "orders"),
                new ShopifyWebhookTarget("PRODUCTS_UPDATE", Prefix + "products"),
                new ShopifyWebhookTarget("ORDERS_PAID", Prefix + "paid"),
            }, Prefix, CancellationToken.None);

            Assert.Equal(new[] { ShopifyWebhookChange.Kept, ShopifyWebhookChange.Created, ShopifyWebhookChange.Failed, ShopifyWebhookChange.Deleted },
                outcomes.Select(o => o.Change));
            Assert.Equal("10", outcomes[1].SubscriptionId);
            Assert.Contains("Access denied", outcomes[2].Error, StringComparison.Ordinal);
            using JsonDocument page2 = JsonDocument.Parse(h.Handler.Requests[1].Body);
            Assert.Equal("c1", page2.RootElement.GetProperty("variables").GetProperty("after").GetString());
            using JsonDocument create = JsonDocument.Parse(h.Handler.Requests[2].Body);
            JsonElement sub = create.RootElement.GetProperty("variables").GetProperty("webhookSubscription");
            Assert.Equal(Prefix + "products", sub.GetProperty("uri").GetString());
            Assert.Equal("JSON", sub.GetProperty("format").GetString());
            Assert.Contains(h.Logger.Entries, e => e.Level == LogLevel.Error && e.Exception is ShopifyGraphQlException);
        }

        [Fact]
        public async Task Sync_CreateOutcomeUnknown_IsFailedAndNotResent()
        {
            Harness h = new Harness();
            h.Handler
                .Json(HttpStatusCode.OK, "{\"data\":{\"webhookSubscriptions\":{\"nodes\":[],\"pageInfo\":{\"hasNextPage\":false,\"endCursor\":null}}}}")
                .Json(HttpStatusCode.BadGateway, "{}");
            ShopifyWebhookRegistrar registrar = new ShopifyWebhookRegistrar(h.Client(), h.Logger);

            IReadOnlyList<ShopifyWebhookOutcome> outcomes = await registrar.SyncAsync(new[] { new ShopifyWebhookTarget("ORDERS_PAID", Prefix + "paid") },
                Prefix, CancellationToken.None);

            Assert.Equal(ShopifyWebhookChange.Failed, Assert.Single(outcomes).Change);
            Assert.Equal(2, h.Handler.Requests.Count);
        }

        [Fact]
        public async Task Sync_CreateWithoutId_IsFailedNotCreated()
        {
            Harness h = new Harness();
            h.Handler
                .Json(HttpStatusCode.OK, "{\"data\":{\"webhookSubscriptions\":{\"nodes\":[],\"pageInfo\":{\"hasNextPage\":false,\"endCursor\":null}}}}")
                .Json(HttpStatusCode.OK, "{\"data\":{\"webhookSubscriptionCreate\":{\"webhookSubscription\":null,\"userErrors\":[]}}}");
            IReadOnlyList<ShopifyWebhookOutcome> outcomes = await new ShopifyWebhookRegistrar(h.Client(), h.Logger)
                .SyncAsync(new[] { new ShopifyWebhookTarget("ORDERS_PAID", Prefix + "paid") }, Prefix, CancellationToken.None);
            ShopifyWebhookOutcome o = Assert.Single(outcomes);
            Assert.Equal(ShopifyWebhookChange.Failed, o.Change);
            Assert.Null(o.SubscriptionId);
            Assert.Contains("no subscription id", o.Error, StringComparison.Ordinal);
        }

        [Fact]
        public async Task Sync_DeleteFails_IsReported()
        {
            Harness h = new Harness();
            h.Handler
                .Json(HttpStatusCode.OK, "{\"data\":{\"webhookSubscriptions\":{\"nodes\":[{\"id\":\"7\",\"topic\":\"SHOP_UPDATE\",\"uri\":\"" + Prefix + "shop\"}],\"pageInfo\":{\"hasNextPage\":false,\"endCursor\":null}}}}")
                .Json(HttpStatusCode.OK, "{\"data\":{\"webhookSubscriptionDelete\":{\"deletedWebhookSubscriptionId\":null,\"userErrors\":[{\"field\":[\"id\"],\"message\":\"Not found\"}]}}}");
            IReadOnlyList<ShopifyWebhookOutcome> outcomes = await new ShopifyWebhookRegistrar(h.Client(), h.Logger)
                .SyncAsync(new ShopifyWebhookTarget[0], Prefix, CancellationToken.None);
            ShopifyWebhookOutcome o = Assert.Single(outcomes);
            Assert.Equal(ShopifyWebhookChange.Failed, o.Change);
            Assert.Equal("7", o.SubscriptionId);
        }

        [Fact]
        public async Task Sync_ListFails_Throws_AndChangesNothing()
        {
            Harness h = new Harness();
            h.Handler.Json(HttpStatusCode.OK, Bodies.AccessDenied);
            await Assert.ThrowsAsync<ShopifyGraphQlException>(() => new ShopifyWebhookRegistrar(h.Client(), h.Logger)
                .SyncAsync(new[] { new ShopifyWebhookTarget("ORDERS_PAID", Prefix + "paid") }, Prefix, CancellationToken.None));
            Assert.Single(h.Handler.Requests);
        }

        [Fact]
        public async Task Delete_RequiresId()
        {
            Harness h = new Harness();
            ShopifyWebhookRegistrar registrar = new ShopifyWebhookRegistrar(h.Client(), h.Logger);
            await Assert.ThrowsAsync<ArgumentException>(() => registrar.DeleteAsync(" ", CancellationToken.None));
            await Assert.ThrowsAsync<ArgumentNullException>(() => registrar.CreateAsync(null, CancellationToken.None));
        }
    }
}
