// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class PriceBasedShippingRate : ShopifyObject
{
    /// <summary>
    /// The name of the price based shipping rate, specified by the user.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Minimum order price
    /// </summary>
    [JsonProperty("min_order_subtotal")]
    public decimal? MinOrderSubtotal { get; set; }

    /// <summary>
    /// Rate amount
    /// </summary>
    [JsonProperty("price")]
    public decimal? Price { get; set; }

    /// <summary>
    /// Maximum order price
    /// </summary>
    [JsonProperty("max_order_subtotal")]
    public decimal? MaxOrderSubtotal { get; set; }

    /// <summary>
    /// Shipping zone id
    /// </summary>
    [JsonProperty("shipping_zone_id")]
    public long? ShippingZoneId { get; set; }
}