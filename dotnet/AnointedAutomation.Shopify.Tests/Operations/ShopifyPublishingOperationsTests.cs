// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using System;
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
    public class ShopifyPublishingOperationsTests
    {
        private const string Pub1 = "gid://shopify/Publication/1";
        private const string Pub2 = "gid://shopify/Publication/2";

        // The exact literals the API ships today; a builder must not drift from them.
        [Theory]
        [InlineData(ShopifyPublishingOperations.PublishMutation,
            "mutation($id: ID!, $input: [PublicationInput!]!) { publishablePublish(id: $id, input: $input) { userErrors { field message } } }")]
        [InlineData(ShopifyPublishingOperations.UnpublishMutation,
            "mutation($id: ID!, $input: [PublicationInput!]!) { publishableUnpublish(id: $id, input: $input) { userErrors { field message } } }")]
        [InlineData(ShopifyPublishingOperations.ProductPublicationsQuery,
            "query($id: ID!) { product(id: $id) { resourcePublicationsV2(first: 25) { edges { node { publication { id } } } } } }")]
        [InlineData(ShopifyPublishingOperations.CollectionPublicationsQuery,
            "query($id: ID!) { collection(id: $id) { resourcePublicationsV2(first: 60) { edges { node { isPublished publication { id } } } } } }")]
        [InlineData(ShopifyPublishingOperations.SellingPlanGroupCreateMutation,
            "mutation createSellingPlanGroup($input: SellingPlanGroupInput!, $resources: SellingPlanGroupResourceInput) { sellingPlanGroupCreate(input: $input, resources: $resources) { sellingPlanGroup { id sellingPlans(first: 10) { nodes { id } } products(first: 1) { nodes { title } } } userErrors { field message } } }")]
        [InlineData(ShopifyPublishingOperations.SellingPlanGroupUpdateMutation,
            "mutation updateSellingPlanGroup($id: ID!, $input: SellingPlanGroupInput!) { sellingPlanGroupUpdate(id: $id, input: $input) { sellingPlanGroup { id sellingPlans(first: 10) { nodes { id } } products(first: 1) { nodes { title } } } userErrors { field message } } }")]
        [InlineData(ShopifyPublishingOperations.SellingPlanGroupDeleteMutation,
            "mutation deleteSellingPlanGroup($id: ID!) { sellingPlanGroupDelete(id: $id) { deletedSellingPlanGroupId userErrors { field message } } }")]
        [InlineData(ShopifyPublishingOperations.OrderFulfillmentOrdersQuery,
            "query($id: ID!){ order(id: $id){ fulfillmentOrders(first: 25){ nodes{ id supportedActions{ action } lineItems(first: 50){ nodes{ remainingQuantity lineItem{ id } } } } } } }")]
        public void Text_IsPinned(string actual, string expected)
        {
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Publish_BuildsObjectShapedInputs()
        {
            ShopifyOperation<ShopifyUserErrorsPayload> op = ShopifyPublishingOperations.Publish("gid://shopify/Product/1", new[] { Pub1, Pub2 });
            Assert.Equal("publishablePublish", op.Field);
            Assert.True(op.Request.IsMutation);
            Assert.Equal(ShopifyPublishingOperations.PublishMutation, op.Request.Query);
            using JsonDocument d = JsonDocument.Parse(op.Request.ToJson());
            JsonElement vars = d.RootElement.GetProperty("variables");
            Assert.Equal("gid://shopify/Product/1", vars.GetProperty("id").GetString());
            Assert.Equal(new[] { Pub1, Pub2 }, vars.GetProperty("input").EnumerateArray().Select(e => e.GetProperty("publicationId").GetString()));
        }

        [Fact]
        public void Unpublish_UsesItsOwnMutationAndField()
        {
            ShopifyOperation<ShopifyUserErrorsPayload> op = ShopifyPublishingOperations.Unpublish("gid://shopify/Collection/5", new[] { Pub1 });
            Assert.Equal("publishableUnpublish", op.Field);
            Assert.Equal(ShopifyPublishingOperations.UnpublishMutation, op.Request.Query);
            Assert.True(op.Request.IsMutation);
        }

        [Fact]
        public void PublishAndUnpublish_RejectNonPublicationGids()
        {
            Assert.Throws<ArgumentException>(() => ShopifyPublishingOperations.Publish(" ", new[] { Pub1 }));
            Assert.Throws<ArgumentException>(() => ShopifyPublishingOperations.Publish("gid://shopify/Product/1", null));
            Assert.Throws<ArgumentException>(() => ShopifyPublishingOperations.Publish("gid://shopify/Product/1", new string[0]));
            Assert.Throws<ArgumentException>(() => ShopifyPublishingOperations.Publish("gid://shopify/Product/1", new[] { "12345" }));
            Assert.Throws<ArgumentException>(() => ShopifyPublishingOperations.Unpublish("gid://shopify/Product/1", new[] { Pub1, "gid://shopify/Location/1" }));
            Assert.Throws<ArgumentException>(() => ShopifyPublishingOperations.Unpublish("gid://shopify/Product/1", new[] { " " }));
        }

        [Fact]
        public void ProductPublications_BuildsQuery()
        {
            ShopifyOperation<ShopifyPublishedResource> op = ShopifyPublishingOperations.ProductPublications("gid://shopify/Product/1");
            Assert.Equal("product", op.Field);
            Assert.False(op.Request.IsMutation);
            Assert.Equal(ShopifyPublishingOperations.ProductPublicationsQuery, op.Request.Query);
            Assert.Throws<ArgumentException>(() => ShopifyPublishingOperations.ProductPublications(""));
        }

        [Fact]
        public void CollectionPublications_BuildsQuery()
        {
            ShopifyOperation<ShopifyCollection> op = ShopifyPublishingOperations.CollectionPublications("gid://shopify/Collection/1");
            Assert.Equal("collection", op.Field);
            Assert.False(op.Request.IsMutation);
            Assert.Throws<ArgumentException>(() => ShopifyPublishingOperations.CollectionPublications(null));
        }

        [Fact]
        public void SellingPlanGroupCreate_SendsResourcesOnlyWhenGiven()
        {
            ShopifyOperation<ShopifySellingPlanGroupPayload> without = ShopifyPublishingOperations.SellingPlanGroupCreate(new { name = "Pre-order" }, null);
            Assert.Equal("sellingPlanGroupCreate", without.Field);
            Assert.True(without.Request.IsMutation);
            using (JsonDocument d = JsonDocument.Parse(without.Request.ToJson()))
            {
                JsonElement vars = d.RootElement.GetProperty("variables");
                Assert.Equal("Pre-order", vars.GetProperty("input").GetProperty("name").GetString());
                Assert.False(vars.TryGetProperty("resources", out _));
            }
            ShopifyOperation<ShopifySellingPlanGroupPayload> with = ShopifyPublishingOperations.SellingPlanGroupCreate(
                new { name = "Pre-order" }, new { productIds = new[] { "gid://shopify/Product/1" } });
            using (JsonDocument d = JsonDocument.Parse(with.Request.ToJson()))
            {
                Assert.Equal("gid://shopify/Product/1",
                    d.RootElement.GetProperty("variables").GetProperty("resources").GetProperty("productIds")[0].GetString());
            }
            Assert.Throws<ArgumentNullException>(() => ShopifyPublishingOperations.SellingPlanGroupCreate(null, null));
        }

        [Fact]
        public void SellingPlanGroupUpdate_And_Delete_BuildVariables()
        {
            ShopifyOperation<ShopifySellingPlanGroupPayload> upd = ShopifyPublishingOperations.SellingPlanGroupUpdate("gid://shopify/SellingPlanGroup/1", new { name = "N" });
            Assert.Equal("sellingPlanGroupUpdate", upd.Field);
            using (JsonDocument d = JsonDocument.Parse(upd.Request.ToJson()))
            {
                JsonElement vars = d.RootElement.GetProperty("variables");
                Assert.Equal("gid://shopify/SellingPlanGroup/1", vars.GetProperty("id").GetString());
                Assert.Equal("N", vars.GetProperty("input").GetProperty("name").GetString());
            }
            ShopifyOperation<ShopifySellingPlanGroupDeletePayload> del = ShopifyPublishingOperations.SellingPlanGroupDelete("gid://shopify/SellingPlanGroup/1");
            Assert.Equal("sellingPlanGroupDelete", del.Field);
            Assert.True(del.Request.IsMutation);
            Assert.Throws<ArgumentException>(() => ShopifyPublishingOperations.SellingPlanGroupUpdate(" ", new { }));
            Assert.Throws<ArgumentNullException>(() => ShopifyPublishingOperations.SellingPlanGroupUpdate("gid://shopify/SellingPlanGroup/1", null));
            Assert.Throws<ArgumentException>(() => ShopifyPublishingOperations.SellingPlanGroupDelete(""));
        }

        [Fact]
        public void OrderFulfillmentOrders_BuildsQuery()
        {
            ShopifyOperation<ShopifyOrderFulfillmentOrders> op = ShopifyPublishingOperations.OrderFulfillmentOrders("gid://shopify/Order/1");
            Assert.Equal("order", op.Field);
            Assert.False(op.Request.IsMutation);
            Assert.Equal(ShopifyPublishingOperations.OrderFulfillmentOrdersQuery, op.Request.Query);
            Assert.Throws<ArgumentException>(() => ShopifyPublishingOperations.OrderFulfillmentOrders(" "));
        }

        [Fact]
        public async Task Execute_ReadsCollectionPublications()
        {
            Harness h = new Harness();
            h.Handler.Json(HttpStatusCode.OK,
                "{\"data\":{\"collection\":{\"id\":\"gid://shopify/Collection/7\",\"resourcePublicationsV2\":{\"edges\":[{\"node\":{\"isPublished\":true,\"publication\":{\"id\":\"gid://shopify/Publication/1\"}}},{\"node\":{\"isPublished\":false,\"publication\":{\"id\":\"gid://shopify/Publication/2\"}}}]}}}}");
            ShopifyCollection c = await h.Client().ExecuteAsync(ShopifyPublishingOperations.CollectionPublications("gid://shopify/Collection/7"), CancellationToken.None);
            Assert.Equal(7, c.NumericId);
            Assert.Equal(new[] { true, false }, c.ResourcePublicationsV2.Edges.Select(e => e.Node.IsPublished));
            Assert.Equal(2, c.ResourcePublicationsV2.Edges[1].Node.Publication.NumericId);
        }

        [Fact]
        public async Task Execute_ReadsProductPublications_AndNotFoundIsNull()
        {
            Harness h = new Harness();
            h.Handler
                .Json(HttpStatusCode.OK, "{\"data\":{\"product\":{\"resourcePublicationsV2\":{\"edges\":[{\"node\":{\"publication\":{\"id\":\"gid://shopify/Publication/9\"}}}]}}}}")
                .Json(HttpStatusCode.OK, "{\"data\":{\"product\":null}}");
            ShopifyGraphQlClient client = h.Client();
            ShopifyPublishedResource found = await client.ExecuteAsync(ShopifyPublishingOperations.ProductPublications("gid://shopify/Product/1"), CancellationToken.None);
            Assert.Equal(9, Assert.Single(found.ResourcePublicationsV2.Edges).Node.Publication.NumericId);
            ShopifyPublishedResource missing = await client.ExecuteAsync(ShopifyPublishingOperations.ProductPublications("gid://shopify/Product/2"), CancellationToken.None);
            Assert.Null(missing);
        }

        [Fact]
        public async Task Execute_ReadsFulfillmentOrders()
        {
            Harness h = new Harness();
            h.Handler.Json(HttpStatusCode.OK,
                "{\"data\":{\"order\":{\"id\":\"gid://shopify/Order/1\",\"fulfillmentOrders\":{\"nodes\":[{\"id\":\"gid://shopify/FulfillmentOrder/3\",\"supportedActions\":[{\"action\":\"REQUEST_CANCELLATION\"}],\"lineItems\":{\"nodes\":[{\"remainingQuantity\":2,\"lineItem\":{\"id\":\"gid://shopify/LineItem/4\"}}]}}]}}}}");
            ShopifyOrderFulfillmentOrders o = await h.Client().ExecuteAsync(ShopifyPublishingOperations.OrderFulfillmentOrders("gid://shopify/Order/1"), CancellationToken.None);
            ShopifyFulfillmentOrder fo = Assert.Single(o.FulfillmentOrders.Nodes);
            Assert.Equal(3, fo.NumericId);
            Assert.Equal("REQUEST_CANCELLATION", Assert.Single(fo.SupportedActions).Action);
            Assert.Equal(2, Assert.Single(fo.LineItems.Nodes).RemainingQuantity);
            Assert.Equal(4, fo.LineItems.Nodes[0].LineItem.NumericId);
        }

        [Fact]
        public async Task Execute_SellingPlanGroupCreate_ReadsGroupAndPlans()
        {
            Harness h = new Harness();
            h.Handler.Json(HttpStatusCode.OK,
                "{\"data\":{\"sellingPlanGroupCreate\":{\"sellingPlanGroup\":{\"id\":\"gid://shopify/SellingPlanGroup/1\",\"sellingPlans\":{\"nodes\":[{\"id\":\"gid://shopify/SellingPlan/2\"}]},\"products\":{\"nodes\":[{\"title\":\"Tee\"}]}},\"userErrors\":[]}}}");
            ShopifySellingPlanGroupPayload p = await h.Client().ExecuteAsync(
                ShopifyPublishingOperations.SellingPlanGroupCreate(new { name = "Pre-order" }, null), CancellationToken.None);
            Assert.Equal(1, p.SellingPlanGroup.NumericId);
            Assert.Equal(2, Assert.Single(p.SellingPlanGroup.SellingPlans.Nodes).NumericId);
            Assert.Equal("Tee", Assert.Single(p.SellingPlanGroup.Products.Nodes).Title);
        }

        [Fact]
        public async Task Execute_SellingPlanGroupDelete_ReadsDeletedId()
        {
            Harness h = new Harness();
            h.Handler.Json(HttpStatusCode.OK,
                "{\"data\":{\"sellingPlanGroupDelete\":{\"deletedSellingPlanGroupId\":\"gid://shopify/SellingPlanGroup/1\",\"userErrors\":[]}}}");
            ShopifySellingPlanGroupDeletePayload p = await h.Client().ExecuteAsync(
                ShopifyPublishingOperations.SellingPlanGroupDelete("gid://shopify/SellingPlanGroup/1"), CancellationToken.None);
            Assert.Equal("gid://shopify/SellingPlanGroup/1", p.DeletedSellingPlanGroupId);
        }
    }
}
