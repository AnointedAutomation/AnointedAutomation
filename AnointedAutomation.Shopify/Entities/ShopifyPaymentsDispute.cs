// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;
using System;

namespace AnointedAutomation.Shopify;

/// <summary>
/// An object representing a Shopify payments dispute.
/// </summary>
public class ShopifyPaymentsDispute : ShopifyObject
{
    [JsonProperty("order_id")]
    public long? OrderId { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("currency")]
    public string Currency { get; set; }

    [JsonProperty("amount")]
    public decimal? Amount { get; set; }

    [JsonProperty("reason")]
    public string Reason { get; set; }

    [JsonProperty("network_reason_code")]
    public string NetworkReasonCode { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("evidence_due_by")]
    public DateTimeOffset? EvidenceDueBy { get; set; }

    [JsonProperty("initiated_at")]
    public DateTimeOffset? InitiatedAt { get; set; }

    [JsonProperty("evidence_sent_on")]
    public DateTimeOffset? EvidenceSentOn { get; set; }

    [JsonProperty("finalized_on")]
    public DateTimeOffset? FinalizedOn { get; set; }


}