// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AnointedAutomation.Shopify;

public class LineItemsByFulfillmentOrder
{
    /// <summary>
    /// The ID of the fulfillment order associated with this line item.
    /// </summary>
    [JsonProperty("fulfillment_order_id")]
    public long? FulfillmentOrderId { get; set; }

    /// <summary>
    /// The fulfillment order line items to be requested for fulfillment. If left blank, all line items of the fulfillment order are requested for fulfillment.
    /// </summary>
    [JsonProperty("fulfillment_order_line_items")]
    public IEnumerable<FulfillmentRequestOrderLineItem> FulfillmentRequestOrderLineItems { get; set; }
}