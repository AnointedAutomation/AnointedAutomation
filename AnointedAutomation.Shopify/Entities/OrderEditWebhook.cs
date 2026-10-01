// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

/// <summary>
/// An object representing a Shopify order edit webhook (orders/edited topic)
/// </summary>
public class OrderEditWebhook
{
    /// <summary>
    /// The OrderEdit object
    /// </summary>
    [JsonProperty("order_edit")]
    public OrderEdit OrderEdit { get; set; }
}