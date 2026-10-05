// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class TaxLine
{
    /// <summary>
    /// Whether the channel that submitted the tax line is responsible for remitting it.
    /// </summary>
    [JsonProperty("channel_liable")]
    public bool? ChannelLiable { get; set; }

    /// <summary>
    /// The amount of tax to be charged.
    /// </summary>
    [JsonProperty("price")]
    public decimal? Price { get; set; }

    /// <summary>
    /// The rate of tax to be applied.
    /// </summary>
    [JsonProperty("rate")]
    public decimal? Rate { get; set; }

    /// <summary>
    /// The proportion of the line item price represented by the tax, expressed as a percentage.
    /// </summary>
    [JsonProperty("ratePercentage")]
    public decimal? RatePercentage { get; set; }

    /// <summary>
    /// The origin of the tax.
    /// </summary>
    [JsonProperty("source")]
    public string Source { get; set; }

    /// <summary>
    /// The name of the tax.
    /// </summary>
    [JsonProperty("title")]
    public string Title { get; set; }

    /// <summary>
    /// The amount added to the order for this tax in shop and presentment currencies.
    /// </summary>
    [JsonProperty("price_set")]
    public PriceSet PriceSet { get; set; }
}
