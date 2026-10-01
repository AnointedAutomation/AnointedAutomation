// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;
using System;

namespace AnointedAutomation.Shopify;

/// <summary>
/// An object representing a Shopify collect.
/// </summary>
public class Collect : ShopifyObject
{
    /// <summary>
    /// The id of the custom collection containing the product.
    /// </summary>
    [JsonProperty("collection_id")]
    public long? CollectionId { get; set; }

    /// <summary>
    /// The unique numeric identifier for the product in the custom collection.
    /// </summary>
    [JsonProperty("product_id")]
    public long? ProductId { get; set; }

    /// <summary>
    /// The date and time when the collect was created. The API returns this value in ISO 8601 format.
    /// </summary>
    [JsonProperty("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    /// The date and time when the collect was last updated. The API returns this value in ISO 8601 format.
    /// </summary>
    [JsonProperty("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// A number specifying the manually sorted position of this product in a custom collection. The first position is 1. This value only applies when the custom collection is viewed using the Manual sort order.
    /// </summary>
    [JsonProperty("position")]
    public long? Position { get; set; }

    /// <summary>
    /// This is the same value as position but padded with leading zeroes to make it alphanumeric-sortable.
    /// </summary>
    [JsonProperty("sort_value")]
    public string SortValue { get; set; }
}
