// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation Corporation. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

/// <summary>
/// An object representing a Shopify payments balance.
/// </summary>
public class ShopifyPaymentsBalance
{
    [JsonProperty("currency")]
    public string Currency { get; set; }

    [JsonProperty("amount")]
    public decimal? Amount { get; set; }
}