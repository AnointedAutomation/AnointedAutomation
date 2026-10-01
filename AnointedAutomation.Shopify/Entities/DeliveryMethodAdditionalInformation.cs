// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class DeliveryMethodAdditionalInformation
{
    /// <summary>
    /// instructions: The delivery instructions to follow when performing the delivery.
    /// </summary>
    [JsonProperty("instructions")]
    public string Instructions { get; set; }

    /// <summary>
    /// The phone number to contact when performing the delivery.
    /// </summary>
    [JsonProperty("phone")]
    public string Phone { get; set; }
}
