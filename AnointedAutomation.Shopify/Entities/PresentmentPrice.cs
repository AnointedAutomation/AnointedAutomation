// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class PresentmentPrice
{
    /// <summary>
    /// The price of the product variant.
    /// </summary>
    [JsonProperty("price")]
    public Price Price { get; set; }

    /// <summary>
    /// The competitors prices for the same item.
    /// </summary>
    [JsonProperty("compare_at_price")]
    public Price CompareAtPrice { get; set; }
}