// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class PaymentCreditCard
{
    [JsonProperty("first_name")]
    public string FirstName { get; set; }

    [JsonProperty("last_name")]
    public string LastName { get; set; }

    [JsonProperty("first_digits")]
    public long FirstDigits { get; set; }

    [JsonProperty("last_digits")]
    public long LastDigits { get; set; }

    [JsonProperty("brand")]
    public string Brand { get; set; }

    [JsonProperty("expiry_month")]
    public long ExpiryMonth { get; set; }

    [JsonProperty("expiry_year")]
    public long ExpiryYear { get; set; }

    [JsonProperty("customer_id")]
    public long CustomerId { get; set; }
}