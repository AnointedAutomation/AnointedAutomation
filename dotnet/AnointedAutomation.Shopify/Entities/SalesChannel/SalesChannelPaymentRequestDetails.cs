// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation Corporation. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class SalesChannelPaymentRequestDetails
{
    [JsonProperty("ip_address")]
    public string IpAddress { get; set; }

    [JsonProperty("accept_language")]
    public string AcceptLanguage { get; set; }

    [JsonProperty("user_agent")]
    public string UserAgent { get; set; }
}