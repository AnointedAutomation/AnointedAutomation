// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class Price
{
    /// <summary>
    /// The three-letter code (ISO 4217 format) for currency.
    /// </summary>
    [JsonProperty("currency_code")]
    public string CurrencyCode { get; set; }

    /// <summary>
    /// The amount in the currency.
    /// </summary>
    [JsonProperty("amount")]
    public decimal? Amount { get; set; }
}