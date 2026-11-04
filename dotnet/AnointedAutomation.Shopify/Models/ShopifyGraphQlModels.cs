// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// Hand-written System.Text.Json response models for the typed operations (ShopifyCatalogOperations). Every field is
// optional because a query selects only what it needs; read them with ShopifyGraphQlJson.Options (case-insensitive,
// numbers from strings, since Shopify sends Money and UnsignedInt64 as strings). Ids stay gid strings; NumericId reuses
// ShopifyGid. The REST POCOs in AnointedAutomation.Shopify are untouched.

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using AnointedAutomation.Shopify.GraphQL;

namespace AnointedAutomation.Shopify.GraphQL.Models
{
    /// <summary>The serializer options for these models.</summary>
    public static class ShopifyGraphQlJson
    {
        /// <summary>Case-insensitive names, numbers allowed as strings.</summary>
        public static readonly JsonSerializerOptions Options = Create();

        /// <summary>Deserialize <paramref name="element"/> into <typeparamref name="T"/>; a JSON null gives default.</summary>
        public static T Read<T>(JsonElement element) =>
            element.ValueKind == JsonValueKind.Null || element.ValueKind == JsonValueKind.Undefined
                ? default
                : element.Deserialize<T>(Options);

        private static JsonSerializerOptions Create()
        {
            JsonSerializerOptions o = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
            };
            o.MakeReadOnly(true);
            return o;
        }
    }

    /// <summary>Anything with a gid.</summary>
    public class ShopifyNode
    {
        /// <summary>The gid, e.g. gid://shopify/Product/1.</summary>
        public string Id { get; set; }

        /// <summary>The numeric tail of <see cref="Id"/>, or null.</summary>
        public long? NumericId => ShopifyGid.ParseNumericIdInt64OrNull(Id);
    }

    /// <summary>A mutation userError.</summary>
    public sealed class ShopifyUserError
    {
        /// <summary>Path to the offending input field.</summary>
        public List<string> Field { get; set; }

        /// <summary>Message.</summary>
        public string Message { get; set; }

        /// <summary>Error code, when the payload type has one.</summary>
        public string Code { get; set; }
    }

    /// <summary>A connection page: nodes plus pageInfo (reuses <see cref="ShopifyPageInfo"/>).</summary>
    public sealed class ShopifyConnection<T>
    {
        /// <summary>The items.</summary>
        public List<T> Nodes { get; set; }

        /// <summary>Paging state.</summary>
        public ShopifyPageInfo PageInfo { get; set; }
    }

    /// <summary>MoneyV2.</summary>
    public sealed class ShopifyMoneyV2
    {
        /// <summary>Amount (decimal, read from Shopify's string).</summary>
        public decimal Amount { get; set; }

        /// <summary>ISO currency code.</summary>
        public string CurrencyCode { get; set; }
    }

    /// <summary>MoneyBag: shop and presentment currency.</summary>
    public sealed class ShopifyMoneyBag
    {
        /// <summary>In the shop's currency.</summary>
        public ShopifyMoneyV2 ShopMoney { get; set; }

        /// <summary>In the buyer's currency.</summary>
        public ShopifyMoneyV2 PresentmentMoney { get; set; }
    }

    /// <summary>Metafield.</summary>
    public sealed class ShopifyMetafield : ShopifyNode
    {
        /// <summary>Namespace.</summary>
        public string Namespace { get; set; }
        /// <summary>Key.</summary>
        public string Key { get; set; }
        /// <summary>Type, e.g. single_line_text_field.</summary>
        public string Type { get; set; }
        /// <summary>Value as stored (JSON text for json types).</summary>
        public string Value { get; set; }
    }

    /// <summary>InventoryItem (as selected under a variant).</summary>
    public sealed class ShopifyInventoryItem : ShopifyNode
    {
        /// <summary>Whether Shopify tracks quantities.</summary>
        public bool? Tracked { get; set; }
    }

    /// <summary>ProductVariant.</summary>
    public sealed class ShopifyProductVariant : ShopifyNode
    {
        /// <summary>Title.</summary>
        public string Title { get; set; }
        /// <summary>SKU.</summary>
        public string Sku { get; set; }
        /// <summary>Price (Money scalar).</summary>
        public decimal? Price { get; set; }
        /// <summary>Compare-at price.</summary>
        public decimal? CompareAtPrice { get; set; }
        /// <summary>Available quantity across locations.</summary>
        public int? InventoryQuantity { get; set; }
        /// <summary>Inventory item.</summary>
        public ShopifyInventoryItem InventoryItem { get; set; }
    }

    /// <summary>Product.</summary>
    public sealed class ShopifyProduct : ShopifyNode
    {
        /// <summary>Title.</summary>
        public string Title { get; set; }
        /// <summary>Handle.</summary>
        public string Handle { get; set; }
        /// <summary>ACTIVE, ARCHIVED, DRAFT, UNLISTED.</summary>
        public string Status { get; set; }
        /// <summary>Vendor.</summary>
        public string Vendor { get; set; }
        /// <summary>Product type.</summary>
        public string ProductType { get; set; }
        /// <summary>Tags.</summary>
        public List<string> Tags { get; set; }
        /// <summary>Description HTML.</summary>
        public string DescriptionHtml { get; set; }
        /// <summary>Created.</summary>
        public DateTimeOffset? CreatedAt { get; set; }
        /// <summary>Updated.</summary>
        public DateTimeOffset? UpdatedAt { get; set; }
        /// <summary>Variants (first page).</summary>
        public ShopifyConnection<ShopifyProductVariant> Variants { get; set; }
        /// <summary>Metafields (first page).</summary>
        public ShopifyConnection<ShopifyMetafield> Metafields { get; set; }
    }

    /// <summary>Customer (read).</summary>
    public sealed class ShopifyCustomer : ShopifyNode
    {
        /// <summary>First name.</summary>
        public string FirstName { get; set; }
        /// <summary>Last name.</summary>
        public string LastName { get; set; }
        /// <summary>Email.</summary>
        public string Email { get; set; }
        /// <summary>Phone.</summary>
        public string Phone { get; set; }
        /// <summary>Note.</summary>
        public string Note { get; set; }
        /// <summary>Tags.</summary>
        public List<string> Tags { get; set; }
        /// <summary>Order count (UnsignedInt64, read from a string).</summary>
        public long? NumberOfOrders { get; set; }
        /// <summary>Lifetime spend.</summary>
        public ShopifyMoneyV2 AmountSpent { get; set; }
        /// <summary>Created.</summary>
        public DateTimeOffset? CreatedAt { get; set; }
        /// <summary>Updated.</summary>
        public DateTimeOffset? UpdatedAt { get; set; }
    }

    /// <summary>Order line item.</summary>
    public sealed class ShopifyLineItem : ShopifyNode
    {
        /// <summary>Title.</summary>
        public string Title { get; set; }
        /// <summary>Quantity.</summary>
        public int? Quantity { get; set; }
        /// <summary>SKU.</summary>
        public string Sku { get; set; }
        /// <summary>Variant reference (null when deleted).</summary>
        public ShopifyNode Variant { get; set; }
    }

    /// <summary>Order (read only; money-moving mutations are out of scope).</summary>
    public sealed class ShopifyOrder : ShopifyNode
    {
        /// <summary>Name, e.g. #1001.</summary>
        public string Name { get; set; }
        /// <summary>Buyer email.</summary>
        public string Email { get; set; }
        /// <summary>Created.</summary>
        public DateTimeOffset? CreatedAt { get; set; }
        /// <summary>Cancelled, or null.</summary>
        public DateTimeOffset? CancelledAt { get; set; }
        /// <summary>Financial status, e.g. PAID.</summary>
        public string DisplayFinancialStatus { get; set; }
        /// <summary>Fulfillment status, e.g. FULFILLED.</summary>
        public string DisplayFulfillmentStatus { get; set; }
        /// <summary>Tags.</summary>
        public List<string> Tags { get; set; }
        /// <summary>Note.</summary>
        public string Note { get; set; }
        /// <summary>Current total.</summary>
        public ShopifyMoneyBag TotalPriceSet { get; set; }
        /// <summary>Customer reference.</summary>
        public ShopifyNode Customer { get; set; }
        /// <summary>Line items (first page).</summary>
        public ShopifyConnection<ShopifyLineItem> LineItems { get; set; }
    }

    /// <summary>A payload that carries only userErrors (tagsAdd, tagsRemove, productVariantsBulkUpdate,
    /// inventorySetQuantities).</summary>
    public class ShopifyUserErrorsPayload
    {
        /// <summary>userErrors (empty when the mutation applied).</summary>
        public List<ShopifyUserError> UserErrors { get; set; }
    }

    /// <summary>metafieldsSet payload.</summary>
    public sealed class ShopifyMetafieldsSetPayload : ShopifyUserErrorsPayload
    {
        /// <summary>The written metafields.</summary>
        public List<ShopifyMetafield> Metafields { get; set; }
    }

    /// <summary>productUpdate / productSet payload.</summary>
    public sealed class ShopifyProductPayload : ShopifyUserErrorsPayload
    {
        /// <summary>The product.</summary>
        public ShopifyProduct Product { get; set; }
    }

    /// <summary>customerUpdate payload.</summary>
    public sealed class ShopifyCustomerPayload : ShopifyUserErrorsPayload
    {
        /// <summary>The customer.</summary>
        public ShopifyCustomer Customer { get; set; }
    }

    /// <summary>inventoryActivate payload.</summary>
    public sealed class ShopifyInventoryActivatePayload : ShopifyUserErrorsPayload
    {
        /// <summary>The activated level.</summary>
        public ShopifyNode InventoryLevel { get; set; }
    }
}
