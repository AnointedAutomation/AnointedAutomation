// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// Response models for the publishing, selling plan and fulfillment order operations. Every field is optional because a
// query selects only what it needs.

using System.Collections.Generic;

namespace AnointedAutomation.Shopify.GraphQL.Models
{
    /// <summary>A resourcePublicationsV2 entry: whether the resource is published on a publication.</summary>
    public sealed class ShopifyResourcePublication
    {
        /// <summary>True when published; null when the query did not select it.</summary>
        public bool? IsPublished { get; set; }

        /// <summary>The publication (sales channel).</summary>
        public ShopifyNode Publication { get; set; }
    }

    /// <summary>A resourcePublicationsV2 connection (edges of nodes).</summary>
    public sealed class ShopifyResourcePublicationConnection
    {
        /// <summary>The edges.</summary>
        public List<ShopifyResourcePublicationEdge> Edges { get; set; }
    }

    /// <summary>One edge of <see cref="ShopifyResourcePublicationConnection"/>.</summary>
    public sealed class ShopifyResourcePublicationEdge
    {
        /// <summary>The node.</summary>
        public ShopifyResourcePublication Node { get; set; }
    }

    /// <summary>A collection with its publications.</summary>
    public sealed class ShopifyCollection : ShopifyNode
    {
        /// <summary>Title.</summary>
        public string Title { get; set; }

        /// <summary>Handle.</summary>
        public string Handle { get; set; }

        /// <summary>Publications (first page).</summary>
        public ShopifyResourcePublicationConnection ResourcePublicationsV2 { get; set; }
    }

    /// <summary>A selling plan group with its plan ids and first product.</summary>
    public sealed class ShopifySellingPlanGroup : ShopifyNode
    {
        /// <summary>Plans (first page).</summary>
        public ShopifyConnection<ShopifyNode> SellingPlans { get; set; }

        /// <summary>Products (first page).</summary>
        public ShopifyConnection<ShopifyProduct> Products { get; set; }
    }

    /// <summary>sellingPlanGroupCreate / sellingPlanGroupUpdate payload.</summary>
    public sealed class ShopifySellingPlanGroupPayload : ShopifyUserErrorsPayload
    {
        /// <summary>The group.</summary>
        public ShopifySellingPlanGroup SellingPlanGroup { get; set; }
    }

    /// <summary>sellingPlanGroupDelete payload.</summary>
    public sealed class ShopifySellingPlanGroupDeletePayload : ShopifyUserErrorsPayload
    {
        /// <summary>The deleted group id.</summary>
        public string DeletedSellingPlanGroupId { get; set; }
    }

    /// <summary>A product with only its publications (the product id is read, nothing else).</summary>
    public sealed class ShopifyPublishedResource : ShopifyNode
    {
        /// <summary>Publications (first page).</summary>
        public ShopifyResourcePublicationConnection ResourcePublicationsV2 { get; set; }
    }

    /// <summary>A supported action of a fulfillment order.</summary>
    public sealed class ShopifySupportedAction
    {
        /// <summary>The action name, e.g. REQUEST_CANCELLATION.</summary>
        public string Action { get; set; }
    }

    /// <summary>A fulfillment order line item.</summary>
    public sealed class ShopifyFulfillmentOrderLineItem : ShopifyNode
    {
        /// <summary>Quantity still to fulfill.</summary>
        public int RemainingQuantity { get; set; }

        /// <summary>The order line item.</summary>
        public ShopifyNode LineItem { get; set; }
    }

    /// <summary>A fulfillment order.</summary>
    public sealed class ShopifyFulfillmentOrder : ShopifyNode
    {
        /// <summary>Status, e.g. OPEN, CLOSED.</summary>
        public string Status { get; set; }

        /// <summary>Request status, e.g. UNSUBMITTED.</summary>
        public string RequestStatus { get; set; }

        /// <summary>Actions currently allowed.</summary>
        public List<ShopifySupportedAction> SupportedActions { get; set; }

        /// <summary>Line items (first page).</summary>
        public ShopifyConnection<ShopifyFulfillmentOrderLineItem> LineItems { get; set; }
    }

    /// <summary>An order with only its fulfillment orders (read by the fulfillment order query).</summary>
    public sealed class ShopifyOrderFulfillmentOrders : ShopifyNode
    {
        /// <summary>Fulfillment orders (first page).</summary>
        public ShopifyConnection<ShopifyFulfillmentOrder> FulfillmentOrders { get; set; }
    }
}
