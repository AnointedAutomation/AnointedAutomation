// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class CheckoutShippingRatePrices
{
    [JsonProperty("totalTax")]
    public string TotalTax { get; set; }

    [JsonProperty("totalPrice")]
    public string TotalPrice { get; set; }

    [JsonProperty("subtotalPrice")]
    public string SubtotalPrice { get; set; }
}