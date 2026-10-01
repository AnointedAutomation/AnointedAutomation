// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class PrerequisiteToEntitlementQuantityRatio
{
    [JsonProperty("prerequisite_quantity")]
    public int? PrerequisiteQuantity { get; set; }

    [JsonProperty("entitled_quantity")]
    public int? EntitledQuantity { get; set; }
}