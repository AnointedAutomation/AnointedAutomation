// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnointedAutomation.Shopify;

public class ShopifyPaymentsDisputeEvidenceProductDescription
{
    [JsonProperty("Product name")]
    public string ProductName { get; set; }
    public string Title { get; set; }
    public string Price { get; set; }
    public string Quantity { get; set; }

    [JsonProperty("Product Description")]
    public string ProductDescription { get; set; }
}