// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation Corporation. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class HSCode : ShopifyObject
{
    /// <summary>
    /// The two-digit code for the country where the inventory item was made.
    /// </summary>
    [JsonProperty("country_code")]
    public string CountryCode { get; set; }

    /// <summary>
    /// The general Harmonized System (HS) code for the inventory item. Used if a country-specific HS code is not available.
    /// </summary>
    [JsonProperty("harmonized_system_code")]
    public string HarmonizedSystemCode { get; set; }
}