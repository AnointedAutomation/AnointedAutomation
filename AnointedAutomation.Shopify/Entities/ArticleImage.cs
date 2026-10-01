// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;
using System;

namespace AnointedAutomation.Shopify;

/// <summary>
/// Represents a Shopify article's image.
/// </summary>
public class ArticleImage
{
    /// <summary>
    /// A base64 image string only used when creating an image. It will be converted to the <see cref="Src"/> property.
    /// </summary>
    [JsonProperty("attachment")]
    public string Attachment { get; set; }

    /// <summary>
    /// The date and time the image was created.
    /// </summary>
    [JsonProperty("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    /// The image's src URL.
    /// </summary>
    [JsonProperty("src")]
    public string Src { get; set; }
}