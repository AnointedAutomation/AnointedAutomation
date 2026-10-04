// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class PrerequisiteValueRange
{
    [JsonProperty("less_than_or_equal_to")]
    public decimal? LessThanOrEqualTo { get; set; }

    [JsonProperty("greater_than_or_equal_to")]
    public decimal? GreaterThanOrEqualTo { get; set; }
}