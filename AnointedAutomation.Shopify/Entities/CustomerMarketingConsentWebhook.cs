// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

/// <summary>
/// Payload for webhook customers_marketing_consent/update
/// https://shopify.dev/docs/api/admin-rest/2023-07/resources/webhook#event-topics-customers-marketing-consent-update
/// </summary>
public class CustomerMarketingConsentWebhook : ShopifyObject
{
    [JsonProperty("phone")]
    public string Phone { get; set; }

    [JsonProperty("sms_marketing_consent")]
    public CustomerSmsMarketingConsent SmsMarketingConsent { get; set; }
}