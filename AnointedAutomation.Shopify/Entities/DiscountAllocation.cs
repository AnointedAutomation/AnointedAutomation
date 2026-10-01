// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class DiscountAllocation
{
    /// <summary>
    /// The discount amount allocated to the line (not sure why it is a string)
    /// </summary>
    [JsonProperty("amount")]
    public string Amount { get; set; }

    /// <summary>
    /// The index of the associated discount application in the order's discount_applications list.
    /// </summary>
    [JsonProperty("discount_application_index")]
    public long DiscountApplicationIndex { get; set; }

    /// <summary>
    /// The discount amount allocated to the line item in shop and presentment currencies.
    /// </summary>
    [JsonProperty("amount_set")]
    public PriceSet AmountSet { get; set; }
}