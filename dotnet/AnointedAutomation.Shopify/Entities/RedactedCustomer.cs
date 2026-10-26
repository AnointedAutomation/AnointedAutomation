// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation Corporation. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

/// <summary>
/// Represents a customer that should be purged from your database after receiving a GDPR webhook.
/// In some cases, a customer record contains only the customer's email address
/// </summary>
public class RedactedCustomer
{
    /// <summary>
    /// The customer's id.
    /// </summary>
    [JsonProperty("id")]
    public long? Id { get; set; }

    /// <summary>
    /// The customer's email address.
    /// </summary>
    [JsonProperty("email")]
    public string Email { get; set; }

    /// <summary>
    /// The customer's phone number.
    /// </summary>
    [JsonProperty("phone")]
    public string Phone { get; set; }
}