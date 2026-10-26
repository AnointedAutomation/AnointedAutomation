// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation Corporation. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;
using System;

namespace AnointedAutomation.Shopify;

public class Policy
{
    /// <summary>
    /// The name of the policy.
    /// </summary>
    [JsonProperty("title")]
    public string Title { get; set; }

    /// <summary>
    /// The content of the policy.
    /// </summary>
    [JsonProperty("body")]
    public string Body { get; set; }

    /// <summary>
    /// The public URL to policy.
    /// </summary>
    [JsonProperty("url")]
    public string Url { get; set; }

    /// <summary>
    /// The date and time when the policy was created. 
    /// </summary>
    [JsonProperty("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    /// The date and time when the policy was last modified. 
    /// </summary>
    [JsonProperty("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }
}