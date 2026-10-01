// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class InventoryLevelAdjust
{
    /// <summary>
    /// The unique identifier of the inventory item that the inventory level belongs to.
    /// </summary>
    [JsonProperty("inventory_item_id")]
    public long? InventoryItemId { get; set; }

    /// <summary>
    /// The unique identifier of the location that the inventory level belongs to.
    /// </summary>
    [JsonProperty("location_id")]
    public long? LocationId { get; set; }

    /// <summary>
    /// The quantity adjust of inventory items.
    /// </summary>
    [JsonProperty("available_adjustment")]
    public int? AvailableAdjustment { get; set; }
}