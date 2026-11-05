// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// Typed builders for the repeated non-money publishing, selling plan and fulfillment order operations: publishablePublish,
// publishableUnpublish, resourcePublicationsV2 reads for products and collections, sellingPlanGroupCreate / Update /
// Delete, and the order fulfillment orders read. Each builder pins its exact query text (tests assert it byte for byte).
// Out of scope on purpose: orderCancel, refundCreate, orderCapture, orderEdit*, draftOrderCreate, transactions (money paths).
// Schema target: 2026-10. publishablePublish / publishableUnpublish accept ONLY Publication gids.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AnointedAutomation.Shopify.GraphQL.Models;

namespace AnointedAutomation.Shopify.Operations
{
    /// <summary>One PublicationInput: the Publication gid.</summary>
    public sealed record ShopifyPublicationInput(
        [property: JsonPropertyName("publicationId")] string PublicationId);

    /// <summary>The publishing, selling plan and fulfillment order builders and their pinned query texts.</summary>
    public static class ShopifyPublishingOperations
    {
        /// <summary>The Publication gid prefix.</summary>
        public const string PublicationGidPrefix = "gid://shopify/Publication/";

        /// <summary>publishablePublish on a product or collection gid.</summary>
        public const string PublishMutation =
            "mutation($id: ID!, $input: [PublicationInput!]!) { publishablePublish(id: $id, input: $input) { userErrors { field message } } }";

        /// <summary>publishableUnpublish on a product or collection gid.</summary>
        public const string UnpublishMutation =
            "mutation($id: ID!, $input: [PublicationInput!]!) { publishableUnpublish(id: $id, input: $input) { userErrors { field message } } }";

        /// <summary>The publications a product is on.</summary>
        public const string ProductPublicationsQuery =
            "query($id: ID!) { product(id: $id) { resourcePublicationsV2(first: 25) { edges { node { publication { id } } } } } }";

        /// <summary>The publications a collection is on, with whether each is published.</summary>
        public const string CollectionPublicationsQuery =
            "query($id: ID!) { collection(id: $id) { resourcePublicationsV2(first: 60) { edges { node { isPublished publication { id } } } } } }";

        /// <summary>sellingPlanGroupCreate (first 10 plans and the first product come back).</summary>
        public const string SellingPlanGroupCreateMutation =
            "mutation createSellingPlanGroup($input: SellingPlanGroupInput!, $resources: SellingPlanGroupResourceInput) { " +
            "sellingPlanGroupCreate(input: $input, resources: $resources) { sellingPlanGroup { id sellingPlans(first: 10) { nodes { id } } " +
            "products(first: 1) { nodes { title } } } userErrors { field message } } }";

        /// <summary>sellingPlanGroupUpdate.</summary>
        public const string SellingPlanGroupUpdateMutation =
            "mutation updateSellingPlanGroup($id: ID!, $input: SellingPlanGroupInput!) { " +
            "sellingPlanGroupUpdate(id: $id, input: $input) { sellingPlanGroup { id sellingPlans(first: 10) { nodes { id } } " +
            "products(first: 1) { nodes { title } } } userErrors { field message } } }";

        /// <summary>sellingPlanGroupDelete.</summary>
        public const string SellingPlanGroupDeleteMutation =
            "mutation deleteSellingPlanGroup($id: ID!) { sellingPlanGroupDelete(id: $id) { deletedSellingPlanGroupId userErrors { field message } } }";

        /// <summary>An order's fulfillment orders (first 25) with supported actions and open line items.</summary>
        public const string OrderFulfillmentOrdersQuery =
            "query($id: ID!){ order(id: $id){ fulfillmentOrders(first: 25){ nodes{ id supportedActions{ action } " +
            "lineItems(first: 50){ nodes{ remainingQuantity lineItem{ id } } } } } } }";

        /// <summary>Publish a product or collection to Publication gids.</summary>
        public static ShopifyOperation<ShopifyUserErrorsPayload> Publish(string id, IEnumerable<string> publicationIds) =>
            Publication(PublishMutation, "publishablePublish", id, publicationIds);

        /// <summary>Unpublish a product or collection from Publication gids.</summary>
        public static ShopifyOperation<ShopifyUserErrorsPayload> Unpublish(string id, IEnumerable<string> publicationIds) =>
            Publication(UnpublishMutation, "publishableUnpublish", id, publicationIds);

        /// <summary>Read the publications of a product (null when not found).</summary>
        public static ShopifyOperation<ShopifyPublishedResource> ProductPublications(string id)
        {
            ShopifyCatalogOperations.Require(id, nameof(id));
            return ShopifyCatalogOperations.Op<ShopifyPublishedResource>(ProductPublicationsQuery, "product",
                new Dictionary<string, object> { ["id"] = id });
        }

        /// <summary>Read a collection with its publications (null when not found).</summary>
        public static ShopifyOperation<ShopifyCollection> CollectionPublications(string id)
        {
            ShopifyCatalogOperations.Require(id, nameof(id));
            return ShopifyCatalogOperations.Op<ShopifyCollection>(CollectionPublicationsQuery, "collection",
                new Dictionary<string, object> { ["id"] = id });
        }

        /// <summary>Create a selling plan group. <paramref name="input"/> is any JSON-serializable SellingPlanGroupInput
        /// (a dictionary or your own record with camelCase names); <paramref name="resources"/> may be null.</summary>
        public static ShopifyOperation<ShopifySellingPlanGroupPayload> SellingPlanGroupCreate(object input, object resources)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            Dictionary<string, object> variables = new Dictionary<string, object>(StringComparer.Ordinal) { ["input"] = input };
            if (resources != null)
            {
                variables["resources"] = resources;
            }
            return ShopifyCatalogOperations.Op<ShopifySellingPlanGroupPayload>(SellingPlanGroupCreateMutation, "sellingPlanGroupCreate", variables);
        }

        /// <summary>Update a selling plan group.</summary>
        public static ShopifyOperation<ShopifySellingPlanGroupPayload> SellingPlanGroupUpdate(string id, object input)
        {
            ShopifyCatalogOperations.Require(id, nameof(id));
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            return ShopifyCatalogOperations.Op<ShopifySellingPlanGroupPayload>(SellingPlanGroupUpdateMutation, "sellingPlanGroupUpdate",
                new Dictionary<string, object> { ["id"] = id, ["input"] = input });
        }

        /// <summary>Delete a selling plan group.</summary>
        public static ShopifyOperation<ShopifySellingPlanGroupDeletePayload> SellingPlanGroupDelete(string id)
        {
            ShopifyCatalogOperations.Require(id, nameof(id));
            return ShopifyCatalogOperations.Op<ShopifySellingPlanGroupDeletePayload>(SellingPlanGroupDeleteMutation, "sellingPlanGroupDelete",
                new Dictionary<string, object> { ["id"] = id });
        }

        /// <summary>Read the fulfillment orders of an order (null when not found).</summary>
        public static ShopifyOperation<ShopifyOrderFulfillmentOrders> OrderFulfillmentOrders(string orderId)
        {
            ShopifyCatalogOperations.Require(orderId, nameof(orderId));
            return ShopifyCatalogOperations.Op<ShopifyOrderFulfillmentOrders>(OrderFulfillmentOrdersQuery, "order",
                new Dictionary<string, object> { ["id"] = orderId });
        }

        private static ShopifyOperation<ShopifyUserErrorsPayload> Publication(string mutation, string field, string id, IEnumerable<string> publicationIds)
        {
            ShopifyCatalogOperations.Require(id, nameof(id));
            List<string> list = publicationIds == null ? null : publicationIds.ToList();
            if (list == null || list.Count == 0
                || list.Any(p => string.IsNullOrWhiteSpace(p) || !p.StartsWith(PublicationGidPrefix, StringComparison.Ordinal)))
            {
                throw new ArgumentException("At least one Publication gid (" + PublicationGidPrefix + "n) is required.", nameof(publicationIds));
            }
            List<ShopifyPublicationInput> input = list.Select(p => new ShopifyPublicationInput(p)).ToList();
            return ShopifyCatalogOperations.Op<ShopifyUserErrorsPayload>(mutation, field,
                new Dictionary<string, object> { ["id"] = id, ["input"] = input });
        }
    }
}
