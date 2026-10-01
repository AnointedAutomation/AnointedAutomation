// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

/// <summary>
/// Sent via the shop/redacted GDPR webhook, indicating that you should purge the shop's data from your systems.
/// </summary>
public class ShopRedactedWebhook
{
    /// <summary>
    /// The shop's id.
    /// </summary>
    [JsonProperty("shop_id")]
    public long ShopId { get; set; }

    /// <summary>
    /// The shop's *.myshopify.com domain.
    /// </summary>
    [JsonProperty("shop_domain")]
    public string ShopDomain { get; set; }
}