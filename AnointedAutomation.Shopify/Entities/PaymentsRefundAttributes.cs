// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class PaymentsRefundAttributes
{
    /// <summary>
    /// The current status of the refund. Valid values: pending, failure, success, and error.
    /// </summary>
    [JsonProperty("status")]
    public string Status { get; set; }

    /// <summary>
    /// A unique number associated with the transaction that can be used to track the refund.
    /// This property has a value only for transactions completed with Visa or Mastercard.
    /// </summary>
    [JsonProperty("acquirer_reference_number")]
    public string AcquirerReferenceNumber { get; set; }
}