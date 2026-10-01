// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

/// <summary>
/// Sent via the GDPR customers/redact webhook, indicating that you should purge the customer's data from your systems.
/// </summary>
public class CustomerRedactedWebhook : ShopRedactedWebhook
{
    /// <summary>
    /// The customer who has been redacted.
    /// </summary>
    [JsonProperty("customer")]
    public RedactedCustomer Customer { get; set; }

    /// <summary>
    /// A list of order ids placed by the customer that must also be purged from your systems.
    /// </summary>
    [JsonProperty("orders_to_redact")]
    public IEnumerable<long> OrdersToRedact { get; set; }
}