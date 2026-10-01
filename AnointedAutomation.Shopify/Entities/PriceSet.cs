// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class PriceSet
{
    [JsonProperty("shop_money")]
    public Price ShopMoney { get; set; }

    [JsonProperty("presentment_money")]
    public Price PresentmentMoney { get; set; }
}