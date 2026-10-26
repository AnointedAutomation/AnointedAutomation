// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation Corporation. See THIRD-PARTY-NOTICES.md.
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class DraftOrderInvoice
{
    [JsonProperty("to")]            
    public string To { get; set; }

    [JsonProperty("from")]    
    public string From { get; set; }

    [JsonProperty("bcc")]    
    public IEnumerable<string> BCC { get; set; }

    [JsonProperty("subject")]    
    public string Subject { get; set; }

    [JsonProperty("custom_message")]    
    public string CustomMessage { get; set; }
}