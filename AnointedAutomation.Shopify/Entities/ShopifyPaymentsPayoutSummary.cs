// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

/// <summary>
/// An object representing a Shopify payments payout summary.
/// </summary>
public class ShopifyPaymentsPayoutSummary
{
    [JsonProperty("adjustments_fee_amount")]
    public decimal? AdjustmentsFeeAmount { get; set; }

    [JsonProperty("adjustments_gross_amount")]
    public decimal? AdjustmentsGrossAmount { get; set; }

    [JsonProperty("charges_fee_amount")]
    public decimal? ChargesFeeAmount { get; set; }

    [JsonProperty("charges_gross_amount")]
    public decimal? ChargesGrossAmount { get; set; }

    [JsonProperty("refunds_fee_amount")]
    public decimal? RefundsFeeAmount { get; set; }

    [JsonProperty("refunds_gross_amount")]
    public decimal? RefundsGrossAmount { get; set; }

    [JsonProperty("reserved_funds_fee_amount")]
    public decimal? ReservedFundsFeeAmount { get; set; }

    [JsonProperty("reserved_funds_gross_amount")]
    public decimal? ReservedFundsGrossAmount { get; set; }

    [JsonProperty("retried_payouts_fee_amount")]
    public decimal? RetriedPayoutsFeeAmount { get; set; }

    [JsonProperty("retried_payouts_gross_amount")]
    public decimal? RetriedPayoutsGrossAmount { get; set; }
}