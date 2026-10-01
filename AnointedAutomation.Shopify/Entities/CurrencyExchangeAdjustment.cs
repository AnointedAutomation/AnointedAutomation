// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class CurrencyExchangeAdjustment : ShopifyObject
{
    /// <summary>
    /// The difference between the amounts on the associated transaction and the parent transaction.
    /// </summary>
    [JsonProperty("adjustment")]
    public decimal? Adjustment { get; set; }

    /// <summary>
    /// The amount of the parent transaction in the shop currency.
    /// </summary>
    [JsonProperty("original_amount")]
    public decimal? OriginalAmount { get; set; }

    /// <summary>
    /// The amount of the associated transaction in the shop currency.
    /// </summary>
    [JsonProperty("final_amount")]
    public decimal? FinalAmount { get; set; }

    /// <summary>
    /// The shop currency.
    /// </summary>
    [JsonProperty("currency")]
    public string Currency { get; set; }
}