// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class RefundDuty
{
    [JsonProperty("duty_id")]
    public long? DutyId { get; set; }

    [JsonProperty("amount_set")]
    public PriceSet AmountSet { get; set; }
}