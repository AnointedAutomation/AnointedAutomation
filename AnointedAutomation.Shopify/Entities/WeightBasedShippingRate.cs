// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class WeightBasedShippingRate : ShopifyObject
{
    /// <summary>
    /// Minimum order weight
    /// </summary>
    [JsonProperty("weight_low")]
    public decimal? WeightLow { get; set; }

    /// <summary>
    /// Maximum order weight
    /// </summary>
    [JsonProperty("weight_high")]
    public decimal? WeightHigh { get; set; }

    /// <summary>
    /// Name of weight based rate
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Rate amount
    /// </summary>
    [JsonProperty("price")]
    public decimal? Price { get; set; }

    /// <summary>
    /// Shipping zone id
    /// </summary>
    [JsonProperty("shipping_zone_id")]
    public long? ShippingZoneId { get; set; }
}