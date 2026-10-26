// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation Corporation. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;
using System;

namespace AnointedAutomation.Shopify;

/// <summary>
/// An object representing remote javascript tags that are loaded into the pages of a shop's storefront.
/// </summary>
public class ScriptTag: ShopifyObject
{
    /// <summary>
    /// The date and time when the <see cref="ScriptTag"/> was created.
    /// </summary>
    [JsonProperty("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    /// Where the script tag should be included on the store. Known values are 'online_store', 'order_status' or 'all'. Defaults to 'all'.
    /// </summary>
    [JsonProperty("display_scope")]
    public string DisplayScope { get; set; }

    /// <summary>
    /// DOM event which triggers the loading of the script. The only known value is 'onload'.
    /// </summary>
    [JsonProperty("event")]
    public string Event { get; set; }

    /// <summary>
    /// Specifies the location of the ScriptTag.
    /// </summary>
    [JsonProperty("src")]
    public string Src { get; set; }

    /// <summary>
    /// The date and time when the <see cref="ScriptTag"/> was updated.
    /// </summary>
    [JsonProperty("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }
}