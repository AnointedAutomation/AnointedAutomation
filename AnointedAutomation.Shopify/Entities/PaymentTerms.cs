// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class PaymentTerms
{
    /// <summary>
    /// The amount that is owed according to the payment terms.
    /// </summary>
    [JsonProperty("amount")]
    public decimal? amount { get; set; }

    /// <summary>
    /// The presentment currency for the payment
    /// </summary>
    [JsonProperty("currency")]
    public string Currency { get; set; }

    /// <summary>
    /// The number of days between the invoice date and due date that is defined in the selected payment terms template.
    /// </summary>
    [JsonProperty("due_in_days")]
    public int? DueInDays { get; set; }

    /// <summary>
    /// The name of the selected payment terms template for the order.
    /// </summary>
    [JsonProperty("payment_terms_name")]
    public string PaymentTermsName { get; set; }

    /// <summary>
    /// The type of selected payment terms template for the order.
    /// </summary>
    [JsonProperty("payment_terms_type")]
    public string PaymentTermsType { get; set; }

    /// <summary>
    /// An array of schedules associated to the payment terms.
    /// </summary>
    [JsonProperty("payment_schedules")]
    public PaymentSchedule[] PaymentSchedules { get; set; }
}