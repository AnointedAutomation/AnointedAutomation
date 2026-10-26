// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation Corporation. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

/// <summary>
/// An entity representing a Shopify redirect.
/// </summary>
public class Redirect : ShopifyObject
{
    /// <summary>
    /// The "before" path to be redirected. When the user navigates to this path, they will be redirected to the path specified by target.
    /// </summary>
    [JsonProperty("path")]
    public string Path { get; set; }

    /// <summary>
    /// The "after" path or URL to be redirected to. This property can be set to any path on the shop's site, or any URL, even one on a 
    /// completely different domain.
    /// </summary>
    [JsonProperty("target")]
    public string Target { get; set; }
}