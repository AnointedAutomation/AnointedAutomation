// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

/// <summary>
/// An object representing an access scope
/// </summary>
public class AccessScope
{
    /// <summary>
    /// The scope's handle, such as "read_orders", "write_products", etc...
    /// </summary>
    [JsonProperty("handle")]
    public string Handle { get; set; }
}