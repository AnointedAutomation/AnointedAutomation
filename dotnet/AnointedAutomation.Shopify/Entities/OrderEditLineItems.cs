// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation Corporation. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AnointedAutomation.Shopify;

/// <summary>
/// An object representing a Shopify line item edit.
/// The Id of this object is the Id of the line item being edited
/// </summary>
public class OrderEditLineItems
{
    /// <summary>
    /// The additions to the line item
    /// </summary>
    [JsonProperty("additions")]
    public IEnumerable<OrderEditLineItemDelta> Additions { get; set; }

    /// <summary>
    /// The removals to the line item
    /// </summary>
    [JsonProperty("removals")]
    public IEnumerable<OrderEditLineItemDelta> Removals { get; set; }
}