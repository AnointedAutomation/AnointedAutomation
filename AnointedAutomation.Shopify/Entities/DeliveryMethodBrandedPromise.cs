// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class DeliveryMethodBrandedPromise
{
    /// <summary>
    /// The name of the branded promise. For example: `Shop Promise`.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// The handle of the branded promise. For example: `shop_promise`
    /// </summary>
    [JsonProperty("handle")]
    public string Handle { get; set; }
}
