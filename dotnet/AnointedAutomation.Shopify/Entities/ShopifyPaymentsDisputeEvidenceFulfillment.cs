// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation Corporation. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnointedAutomation.Shopify;

public class ShopifyPaymentsDisputeEvidenceFulfillment
{
    [JsonProperty("shipping_carrier")]
    public string ShippingCarrier { get; set; }

    [JsonProperty("shipping_tracking_number")]
    public long? ShippingTrackingNumber { get; set; }

    [JsonProperty("shipping_date")]
    public DateTimeOffset? ShippingDate { get; set; }
}