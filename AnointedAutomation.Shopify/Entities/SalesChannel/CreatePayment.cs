// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class CreatePayment
{
    [JsonProperty("request_details")]
    public SalesChannelPaymentRequestDetails SalesChannelPaymentRequestDetails { get; set; }

    [JsonProperty("amount")]
    public string Amount { get; set; }

    [JsonProperty("session_id")]
    public string SessionId { get; set; }

    [JsonProperty("unique_token")]
    public string UniqueToken { get; set; }
}