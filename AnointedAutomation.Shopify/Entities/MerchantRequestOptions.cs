// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using System;
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class MerchantRequestOptions
{
    [JsonProperty("shipping_method")]
    public string ShippingMethod { get; set; }

    [JsonProperty("note")]
    public string Note { get; set; }

    [JsonProperty("date")]
    public DateTimeOffset? Date { get; set; }
}