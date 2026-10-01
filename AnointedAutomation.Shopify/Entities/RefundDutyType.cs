// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class RefundDutyType
{
    [JsonProperty("duty_id")]
    public long? DutyId { get; set; }

    [JsonProperty("refund_type")]
    public string RefundType { get; set; }
}