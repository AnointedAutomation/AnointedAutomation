// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class FulfillmentHold
{
    /// <summary>
    /// A mandatory reason for the fulfillment hold.
    ///     awaiting_payment: The fulfillment hold is applied because payment is pending.
    ///     high_risk_of_fraud: The fulfillment hold is applied because of a high risk of fraud.
    ///     incorrect_address: The fulfillment hold is applied because of an incorrect address.
    ///     inventory_out_of_stock: The fulfillment hold is applied because inventory is out of stock.
    ///     other: The fulfillment hold is applied for any other reason.
    /// </summary>
    [JsonProperty("reason")]
    public string Reason { get; set; }

    [JsonProperty("reason_notes")]
    public string ReasonNotes { get; set; }
}