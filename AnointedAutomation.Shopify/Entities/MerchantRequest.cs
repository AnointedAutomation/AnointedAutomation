// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class MerchantRequest
{
    /// <summary>
    /// The message returned by the merchant, if any.
    /// </summary>
    [JsonProperty("message")]
    public string Message { get; set; }

    /// <summary>
    /// The request options returned by the merchant, if any.
    /// </summary>
    [JsonProperty("request_options")]
    public MerchantRequestOptions RequestOptions { get; set; }

    /// <summary>
    /// The kind of request. Known valid values: "fulfillment_request", "cancellation_request", or "legacy_fulfill_request".
    /// </summary>
    [JsonProperty("kind")]
    public string Kind { get; set; }
}