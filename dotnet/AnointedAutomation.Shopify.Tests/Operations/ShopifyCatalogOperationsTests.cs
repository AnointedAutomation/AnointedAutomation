// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.Shopify.GraphQL;
using AnointedAutomation.Shopify.GraphQL.Models;
using AnointedAutomation.Shopify.Operations;
using AnointedAutomation.Shopify.Tests.GraphQL;
using Xunit;

namespace AnointedAutomation.Shopify.Tests.Operations
{
    public class ShopifyCatalogOperationsTests
    {
        // The exact literals consumers already ship today; a builder must not drift from them.
        [Theory]
        [InlineData(ShopifyCatalogOperations.TagsAddMutation,
            "mutation($id: ID!, $tags: [String!]!) { tagsAdd(id: $id, tags: $tags) { userErrors { field message } } }")]
        [InlineData(ShopifyCatalogOperations.TagsRemoveMutation,
            "mutation($id: ID!, $tags: [String!]!) { tagsRemove(id: $id, tags: $tags) { userErrors { field message } } }")]
        [InlineData(ShopifyCatalogOperations.MetafieldsSetMutation,
            "mutation bulkMetafieldsSet($metafields: [MetafieldsSetInput!]!) { metafieldsSet(metafields: $metafields) { metafields { id } userErrors { field message } } }")]
        [InlineData(ShopifyCatalogOperations.ProductUpdateMutation,
            "mutation($input: ProductUpdateInput!){ productUpdate(product:$input){ product{ id title } userErrors{ message } } }")]
        [InlineData(ShopifyCatalogOperations.ProductSetMutation,
            "mutation($input: ProductSetInput!, $sync: Boolean!) { productSet(synchronous: $sync, input: $input) { product { id } userErrors { field message code } } }")]
        [InlineData(ShopifyCatalogOperations.VariantsBulkUpdateMutation,
            "mutation($productId: ID!, $variants: [ProductVariantsBulkInput!]!) { productVariantsBulkUpdate(productId: $productId, variants: $variants) { userErrors { field message } } }")]
        [InlineData(ShopifyCatalogOperations.CustomerUpdateMutation,
            "mutation($input: CustomerInput!) { customerUpdate(input: $input) { customer { id } userErrors { field message } } }")]
        [InlineData(ShopifyCatalogOperations.InventoryActivateMutation,
            "mutation($inventoryItemId: ID!, $locationId: ID!, $idempotencyKey: String!) { inventoryActivate(inventoryItemId: $inventoryItemId, locationId: $locationId) @idempotent(key: $idempotencyKey) { inventoryLevel { id } userErrors { field message } } }")]
        [InlineData(ShopifyCatalogOperations.InventorySetQuantitiesMutation,
            "mutation($input: InventorySetQuantitiesInput!, $idempotencyKey: String!) { inventorySetQuantities(input: $input) @idempotent(key: $idempotencyKey) { userErrors { field message } } }")]
        public void Mutation_Text_IsPinned(string actual, string expected)
        {
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Builders_ProduceExpectedVariablesAndFlags()
        {
            ShopifyOperation<ShopifyUserErrorsPayload> tags = ShopifyCatalogOperations.TagsAdd("gid://shopify/Product/1", new[] { "a", "b" });
            Assert.Equal("tagsAdd", tags.Field);
            Assert.True(tags.Request.IsMutation);
            Assert.False(tags.Request.SafeToResendAfterUnknownOutcome);
            using (JsonDocument d = JsonDocument.Parse(tags.Request.ToJson()))
            {
                Assert.Equal(new[] { "a", "b" }, d.RootElement.GetProperty("variables").GetProperty("tags").EnumerateArray().Select(e => e.GetString()));
            }

            ShopifyOperation<ShopifyUserErrorsPayload> variants = ShopifyCatalogOperations.ProductVariantsBulkUpdate("gid://shopify/Product/1",
                new[] { new ShopifyVariantBulkInput { Id = "gid://shopify/ProductVariant/2", Price = 19.90m } });
            using (JsonDocument d = JsonDocument.Parse(variants.Request.ToJson()))
            {
                JsonElement v = d.RootElement.GetProperty("variables").GetProperty("variants")[0];
                Assert.Equal("19.90", v.GetProperty("price").GetString());
                Assert.False(v.TryGetProperty("compareAtPrice", out _));
                Assert.False(v.TryGetProperty("barcode", out _));
            }

            ShopifyOperation<ShopifyMetafieldsSetPayload> mf = ShopifyCatalogOperations.MetafieldsSet(new[]
            {
                new ShopifyMetafieldsSetInput("gid://shopify/Product/1", "custom", "size", null, "M"),
            });
            using (JsonDocument d = JsonDocument.Parse(mf.Request.ToJson()))
            {
                JsonElement m = d.RootElement.GetProperty("variables").GetProperty("metafields")[0];
                Assert.Equal("gid://shopify/Product/1", m.GetProperty("ownerId").GetString());
                Assert.Equal("custom", m.GetProperty("namespace").GetString());
                Assert.False(m.TryGetProperty("type", out _));
            }

            ShopifyOperation<ShopifyUserErrorsPayload> inv = ShopifyCatalogOperations.InventorySetQuantities(
                new ShopifyInventorySetQuantitiesInput("available", "correction", null,
                    new[] { new ShopifyInventoryQuantityInput("gid://shopify/InventoryItem/1", "gid://shopify/Location/1", 5, null) }),
                "key-1");
            Assert.True(inv.Request.IsIdempotent);
            Assert.True(inv.Request.SafeToResendAfterUnknownOutcome);
            using (JsonDocument d = JsonDocument.Parse(inv.Request.ToJson()))
            {
                JsonElement vars = d.RootElement.GetProperty("variables");
                Assert.Equal("key-1", vars.GetProperty("idempotencyKey").GetString());
                JsonElement q = vars.GetProperty("input").GetProperty("quantities")[0];
                Assert.Equal(JsonValueKind.Null, q.GetProperty("changeFromQuantity").ValueKind);
                Assert.Equal(5, q.GetProperty("quantity").GetInt32());
                Assert.False(vars.GetProperty("input").TryGetProperty("referenceDocumentUri", out _));
            }

            ShopifyOperation<ShopifyInventoryActivatePayload> act = ShopifyCatalogOperations.InventoryActivate("gid://shopify/InventoryItem/1", "gid://shopify/Location/1", "k");
            Assert.True(act.Request.IsIdempotent);

            ShopifyOperation<ShopifyCustomerPayload> cust = ShopifyCatalogOperations.CustomerUpdate(new ShopifyCustomerUpdateInput { Id = "gid://shopify/Customer/1", FirstName = "Sam" });
            using (JsonDocument d = JsonDocument.Parse(cust.Request.ToJson()))
            {
                JsonElement input = d.RootElement.GetProperty("variables").GetProperty("input");
                Assert.Equal("Sam", input.GetProperty("firstName").GetString());
                Assert.False(input.TryGetProperty("lastName", out _));
            }

            ShopifyOperation<ShopifyProductPayload> set = ShopifyCatalogOperations.ProductSet(new Dictionary<string, object> { ["title"] = "T" }, true);
            using (JsonDocument d = JsonDocument.Parse(set.Request.ToJson()))
            {
                Assert.True(d.RootElement.GetProperty("variables").GetProperty("sync").GetBoolean());
            }

            ShopifyOperation<ShopifyProductPayload> upd = ShopifyCatalogOperations.ProductUpdate(new ShopifyProductUpdateInput { Id = "gid://shopify/Product/1", Status = "DRAFT" });
            using (JsonDocument d = JsonDocument.Parse(upd.Request.ToJson()))
            {
                JsonElement input = d.RootElement.GetProperty("variables").GetProperty("input");
                Assert.Equal("DRAFT", input.GetProperty("status").GetString());
                Assert.False(input.TryGetProperty("title", out _));
            }

            Assert.False(ShopifyCatalogOperations.ProductById("gid://shopify/Product/1").Request.IsMutation);
            Assert.Equal("order", ShopifyCatalogOperations.OrderById("gid://shopify/Order/1").Field);
            Assert.Equal("customer", ShopifyCatalogOperations.CustomerById("gid://shopify/Customer/1").Field);
        }

        [Fact]
        public void Builders_RejectBadInput()
        {
            Assert.Throws<ArgumentException>(() => ShopifyCatalogOperations.TagsAdd(" ", new[] { "a" }));
            Assert.Throws<ArgumentException>(() => ShopifyCatalogOperations.TagsRemove("gid://shopify/Product/1", new string[0]));
            Assert.Throws<ArgumentException>(() => ShopifyCatalogOperations.TagsRemove("gid://shopify/Product/1", new[] { " " }));
            Assert.Throws<ArgumentException>(() => ShopifyCatalogOperations.MetafieldsSet(new ShopifyMetafieldsSetInput[0]));
            Assert.Throws<ArgumentException>(() => ShopifyCatalogOperations.MetafieldsSet(Enumerable.Repeat(new ShopifyMetafieldsSetInput("o", "n", "k", null, "v"), 26).ToList()));
            Assert.Throws<ArgumentException>(() => ShopifyCatalogOperations.MetafieldsSet(new[] { new ShopifyMetafieldsSetInput("o", "n", "", null, "v") }));
            Assert.Throws<ArgumentException>(() => ShopifyCatalogOperations.ProductUpdate(new ShopifyProductUpdateInput()));
            Assert.Throws<ArgumentNullException>(() => ShopifyCatalogOperations.ProductSet(null, false));
            Assert.Throws<ArgumentException>(() => ShopifyCatalogOperations.ProductVariantsBulkUpdate("p", new[] { new ShopifyVariantBulkInput() }));
            Assert.Throws<ArgumentException>(() => ShopifyCatalogOperations.CustomerUpdate(null));
            Assert.Throws<ArgumentException>(() => ShopifyCatalogOperations.InventoryActivate("i", "l", " "));
            Assert.Throws<ArgumentException>(() => ShopifyCatalogOperations.InventorySetQuantities(
                new ShopifyInventorySetQuantitiesInput("available", "correction", null, new ShopifyInventoryQuantityInput[0]), "k"));
            Assert.Throws<ArgumentException>(() => ShopifyCatalogOperations.ProductById(""));
        }

        [Fact]
        public async Task Execute_Mutation_ReturnsTypedPayload()
        {
            Harness h = new Harness();
            h.Handler.Json(HttpStatusCode.OK, "{\"data\":{\"productUpdate\":{\"product\":{\"id\":\"gid://shopify/Product/5\",\"title\":\"New\"},\"userErrors\":[]}}}");
            ShopifyProductPayload p = await h.Client().ExecuteAsync(
                ShopifyCatalogOperations.ProductUpdate(new ShopifyProductUpdateInput { Id = "gid://shopify/Product/5", Title = "New" }), CancellationToken.None);
            Assert.Equal("New", p.Product.Title);
            Assert.Equal(5, p.Product.NumericId);
            Assert.Empty(p.UserErrors);
        }

        [Fact]
        public async Task Execute_MutationUserErrors_Throws()
        {
            Harness h = new Harness();
            h.Handler.Json(HttpStatusCode.OK, "{\"data\":{\"tagsAdd\":{\"userErrors\":[{\"field\":[\"id\"],\"message\":\"Not found\"}]}}}");
            ShopifyGraphQlException ex = await Assert.ThrowsAsync<ShopifyGraphQlException>(() =>
                h.Client().ExecuteAsync(ShopifyCatalogOperations.TagsAdd("gid://shopify/Product/1", new[] { "x" }), CancellationToken.None));
            Assert.Contains("Not found", ex.Message, StringComparison.Ordinal);
        }

        [Fact]
        public async Task Execute_Query_ReadsTypedModel_AndNullIsNotFound()
        {
            Harness h = new Harness();
            h.Handler.Json(HttpStatusCode.OK, Fixture("product.json")).Json(HttpStatusCode.OK, "{\"data\":{\"product\":null}}")
                .Json(HttpStatusCode.OK, "{\"data\":{}}");
            ShopifyGraphQlClient client = h.Client();
            ShopifyProduct product = await client.ExecuteAsync(ShopifyCatalogOperations.ProductById("gid://shopify/Product/108828309"), CancellationToken.None);
            Assert.Equal("Example Tee", product.Title);
            Assert.Null(await client.ExecuteAsync(ShopifyCatalogOperations.ProductById("gid://shopify/Product/2"), CancellationToken.None));
            await Assert.ThrowsAsync<InvalidOperationException>(() => client.ExecuteAsync(ShopifyCatalogOperations.ProductById("gid://shopify/Product/3"), CancellationToken.None));
        }

        [Fact]
        public async Task Enumerate_WalksPagesIntoModels()
        {
            Harness h = new Harness();
            h.Handler
                .Json(HttpStatusCode.OK, "{\"data\":{\"customers\":{\"nodes\":[{\"id\":\"gid://shopify/Customer/1\",\"numberOfOrders\":\"3\"}],\"pageInfo\":{\"hasNextPage\":true,\"endCursor\":\"c\"}}}}")
                .Json(HttpStatusCode.OK, "{\"data\":{\"customers\":{\"nodes\":[{\"id\":\"gid://shopify/Customer/2\",\"numberOfOrders\":\"0\"}],\"pageInfo\":{\"hasNextPage\":false,\"endCursor\":null}}}}");
            List<ShopifyCustomer> all = new List<ShopifyCustomer>();
            await foreach (ShopifyCustomer c in ShopifyCatalogOperations.EnumerateCustomersAsync(h.Client(), "tag:vip", 50, CancellationToken.None))
            {
                all.Add(c);
            }
            Assert.Equal(new long?[] { 3, 0 }, all.Select(c => c.NumberOfOrders));
            using JsonDocument first = JsonDocument.Parse(h.Handler.Requests[0].Body);
            Assert.Equal(ShopifyCatalogOperations.CustomersPageQuery, first.RootElement.GetProperty("query").GetString());
            Assert.Equal("tag:vip", first.RootElement.GetProperty("variables").GetProperty("query").GetString());
            Assert.Equal(50, first.RootElement.GetProperty("variables").GetProperty("first").GetInt32());
        }

        [Fact]
        public async Task EnumerateProductsAndOrders_UseTheirConnections()
        {
            Harness h = new Harness();
            h.Handler
                .Json(HttpStatusCode.OK, "{\"data\":{\"products\":{\"nodes\":[{\"id\":\"gid://shopify/Product/1\"}],\"pageInfo\":{\"hasNextPage\":false,\"endCursor\":null}}}}")
                .Json(HttpStatusCode.OK, "{\"data\":{\"orders\":{\"nodes\":[{\"id\":\"gid://shopify/Order/1\",\"name\":\"#1\"}],\"pageInfo\":{\"hasNextPage\":false,\"endCursor\":null}}}}");
            ShopifyGraphQlClient client = h.Client();
            int products = 0;
            await foreach (ShopifyProduct _ in ShopifyCatalogOperations.EnumerateProductsAsync(client, null, 10, CancellationToken.None))
            {
                products++;
            }
            List<string> names = new List<string>();
            await foreach (ShopifyOrder o in ShopifyCatalogOperations.EnumerateOrdersAsync(client, null, 10, CancellationToken.None))
            {
                names.Add(o.Name);
            }
            Assert.Equal(1, products);
            Assert.Equal(new[] { "#1" }, names);
        }

        internal static string Fixture(string name) => File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "graphql", name));
    }

    public class ShopifyGraphQlModelTests
    {
        private static JsonElement Data(string fixture, string field)
        {
            using JsonDocument doc = JsonDocument.Parse(ShopifyCatalogOperationsTests.Fixture(fixture));
            return doc.RootElement.GetProperty("data").GetProperty(field).Clone();
        }

        [Fact]
        public void Product_Fixture_Deserializes()
        {
            ShopifyProduct p = ShopifyGraphQlJson.Read<ShopifyProduct>(Data("product.json", "product"));
            Assert.Equal(108828309, p.NumericId);
            Assert.Equal("ACTIVE", p.Status);
            Assert.Equal(new[] { "cotton", "summer" }, p.Tags);
            Assert.Equal(new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero), p.CreatedAt);
            ShopifyProductVariant v = Assert.Single(p.Variants.Nodes);
            Assert.Equal(19.99m, v.Price);
            Assert.Null(v.CompareAtPrice);
            Assert.Equal(7, v.InventoryQuantity);
            Assert.True(v.InventoryItem.Tracked);
            Assert.False(p.Variants.PageInfo.HasNextPage);
            Assert.Equal("eyJsYXN0X2lkIjo0MzcyOTA3Nn0=", p.Variants.PageInfo.EndCursor);
        }

        [Fact]
        public void Product_FixtureCost_ReadsThroughQueryCost()
        {
            ShopifyGraphQlResponse r = new ShopifyGraphQlResponse(200, ShopifyCatalogOperationsTests.Fixture("product.json"));
            Assert.Equal(12, r.Cost.RequestedQueryCost);
            Assert.Equal(1995, r.Cost.CurrentlyAvailable);
        }

        [Fact]
        public void Order_Fixture_Deserializes()
        {
            ShopifyOrder o = ShopifyGraphQlJson.Read<ShopifyOrder>(Data("order.json", "order"));
            Assert.Equal("#1001", o.Name);
            Assert.Null(o.CancelledAt);
            Assert.Equal(45.50m, o.TotalPriceSet.ShopMoney.Amount);
            Assert.Equal("CAD", o.TotalPriceSet.PresentmentMoney.CurrencyCode);
            Assert.Equal(207119551, o.Customer.NumericId);
            Assert.Equal(2, o.LineItems.Nodes.Count);
            Assert.Equal(2, o.LineItems.Nodes[0].Quantity);
            Assert.Null(o.LineItems.Nodes[1].Variant);
            Assert.Equal(new DateTimeOffset(2026, 3, 1, 15, 0, 0, TimeSpan.Zero), o.CreatedAt.Value.ToUniversalTime());
        }

        [Fact]
        public void Customer_Fixture_Deserializes()
        {
            ShopifyCustomer c = ShopifyGraphQlJson.Read<ShopifyCustomer>(Data("customer.json", "customer"));
            Assert.Equal("Sam", c.FirstName);
            Assert.Equal(12, c.NumberOfOrders);
            Assert.Equal(1234.56m, c.AmountSpent.Amount);
            Assert.Equal(new[] { "wholesale" }, c.Tags);
        }

        [Fact]
        public void Payloads_And_UserErrors_Deserialize()
        {
            using JsonDocument doc = JsonDocument.Parse("{\"metafields\":[{\"id\":\"gid://shopify/Metafield/9\"}],\"userErrors\":[{\"field\":[\"metafields\",\"0\",\"value\"],\"message\":\"Bad\",\"code\":\"INVALID\"}]}");
            ShopifyMetafieldsSetPayload p = ShopifyGraphQlJson.Read<ShopifyMetafieldsSetPayload>(doc.RootElement);
            Assert.Equal(9, p.Metafields[0].NumericId);
            ShopifyUserError e = Assert.Single(p.UserErrors);
            Assert.Equal(new[] { "metafields", "0", "value" }, e.Field);
            Assert.Equal("INVALID", e.Code);
            using JsonDocument nul = JsonDocument.Parse("null");
            Assert.Null(ShopifyGraphQlJson.Read<ShopifyProduct>(nul.RootElement));
            Assert.Null(new ShopifyNode { Id = "not a gid" }.NumericId);
        }
    }
}
