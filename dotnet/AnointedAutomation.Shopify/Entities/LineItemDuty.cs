// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation Corporation. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AnointedAutomation.Shopify;

public class LineItemDuty : ShopifyObject
{
    [JsonProperty("harmonized_system_code")]
    public string HarmonizedSystemCode { get; set; }

    [JsonProperty("country_code_of_origin")]
    public string CountryCodeOfOrigin { get; set; }

    [JsonProperty("price_set")]
    public PriceSet PriceSet { get; set; }

    [JsonProperty("tax_lines")]
    public IEnumerable<TaxLine> TaxLines { get; set; }
}