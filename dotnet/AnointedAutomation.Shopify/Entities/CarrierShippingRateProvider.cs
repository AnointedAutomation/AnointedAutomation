// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class CarrierShippingRateProvider : ShopifyObject
{
    /// <summary>
    /// A Carrier Service (also known as a Carrier Calculated Service or Shipping Service) provides real-time shipping rates to Shopify. Some common carrier services include: Canada Post, FedEx, UPS, and USPS. Note that the term "carrier" is often used interchangeably with the terms "shipping company" and "rate provider."
    /// </summary>
    [JsonProperty("carrier_service_id")]
    public long? CarrierServiceId { get; set; }

    /// <summary>
    /// Rate adjustments - Flat fee
    /// </summary>
    [JsonProperty("flat_modifier")]
    public decimal? FlatModifier { get; set; }

    /// <summary>
    /// Rate adjustments - percentage
    /// </summary>
    [JsonProperty("percent_modifier")]
    public int? PercentModifier { get; set; }

    /// <summary>
    /// Shipping zone id
    /// </summary>
    [JsonProperty("shipping_zone_id")]
    public long? ShippingZoneId { get; set; }
}