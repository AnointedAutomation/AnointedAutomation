// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using System;
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

/// <summary>
/// A CollectionListing resource represents a <see href="https://help.shopify.com/manual/products/collections?shpxid=393b2c3e-2944-4980-AAE0-3BF579EF5D21">product collection</see>
/// that a merchant has made available to your sales channel.
/// Merchants can make collections available to your sales channel directly from their Shopify admin.
/// You can use this resource to retrieve collections that a merchant has published and display them in your marketplace.
/// You can also retrieve a list of published product IDs that belong to a published collection.
/// </summary>
public class CollectionListing
{
    [JsonProperty("collection_id")]
    public long? CollectionId { get; set; }

    [JsonProperty("body_html")]
    public string BodyHtml { get; set; }

    [JsonProperty("default_product_image")]
    public ProductImage DefaultProductImage { get; set; }

    [JsonProperty("image")]
    public CollectionImage[] Image { get; set; }

    [JsonProperty("handle")]
    public string Handle { get; set; }

    [JsonProperty("published_at")]
    public DateTimeOffset? PublishedAt { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("sort_order")]
    public string SortOrder { get; set; }

    [JsonProperty("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }
}