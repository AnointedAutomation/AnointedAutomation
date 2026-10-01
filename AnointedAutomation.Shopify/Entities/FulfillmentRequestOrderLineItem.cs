// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

/// <summary>
/// An object representing a Shopify fulfillment request order line items.
/// </summary>
public class FulfillmentRequestOrderLineItem : ShopifyObject
{
    /// <summary>
    /// The id of the fulfillment order line item being fulfilled. This is **not** the same as <see cref="FulfillmentOrderLineItem.LineItemId"/>;
    /// instead, Shopify expects the value of <see cref="FulfillmentOrderLineItem.Id"/>.
    /// </summary>
    [JsonProperty("id")]
    public new long? Id { get; set; }

    /// <summary>
    /// The total number of units to be fulfilled.
    /// </summary>
    [JsonProperty("quantity")]
    public long? Quantity { get; set; }

}