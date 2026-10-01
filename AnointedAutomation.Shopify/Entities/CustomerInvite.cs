// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AnointedAutomation.Shopify;

public class CustomerInvite : ShopifyObject
{
    /// <summary>
    /// The email address of the customer to be invited
    /// </summary>
    [JsonProperty("to")]
    public string To { get; set; }

    /// <summary>
    /// The sender of the email
    /// </summary>
    [JsonProperty("from")]
    public string From { get; set; }

    /// <summary>
    /// the subject
    /// </summary>
    [JsonProperty("subject")]
    public string Subject { get; set; }

    /// <summary>
    /// a custom message to include
    /// </summary>
    [JsonProperty("custom_message")]
    public string CustomMessage { get; set; }

    /// <summary>
    /// blind copy addresses
    /// </summary>
    [JsonProperty("bcc")]
    public IEnumerable<string> Bcc { get; set; }

}