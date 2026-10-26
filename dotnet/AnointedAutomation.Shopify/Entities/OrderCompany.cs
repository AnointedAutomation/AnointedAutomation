// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation Corporation. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

/// <summary>
/// An object representing a company and its location
/// </summary>
public class OrderCompany
{
    [JsonProperty("id")]
    public long? Id { get; set; }

    [JsonProperty("location_id")]
    public long? LocationId { get; set; }
}