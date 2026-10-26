// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation Corporation. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;
using System;

namespace AnointedAutomation.Shopify;

/// <summary>
/// Represents the image for a <see cref="CustomCollection"/>
/// </summary>
public class CustomCollectionImage
{
    /// <summary>
    /// An image attached to a shop's theme returned as Base64-encoded binary data.
    /// </summary>
    [JsonProperty("attachment")]
    public string Attachment { get; set; }

    /// <summary>
    /// Source URL that specifies the location of the image.
    /// </summary>
    [JsonProperty("src")]
    public string Src { get; set; }

    /// <summary>
    /// Alternative text that describes the collection image.
    /// </summary>
    [JsonProperty("alt")]
    public string Alt { get; set; }

    /// <summary>
    /// The date the image was created.
    /// </summary>
    [JsonProperty("created_at")]
    public DateTimeOffset? Created_At { get; set; }

    /// <summary>
    /// Width of the image in pixels.
    /// </summary>
    [JsonProperty("width")]
    public int? Width { get; set; }

    /// <summary>
    /// Height of the image in pixels.
    /// </summary>
    [JsonProperty("height")]
    public int? Height { get; set; }
}