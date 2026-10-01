// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class OptionValue
{
    /// <summary>
    /// Custom property option id 
    /// </summary>
    [JsonProperty("option_id")]
    public long OptionId { get; set; }

    /// <summary>
    /// Custom property option name
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Custom property option value
    /// </summary>
    [JsonProperty("value")]
    public string Value { get; set; }
}