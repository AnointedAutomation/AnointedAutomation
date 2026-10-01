// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

/// <summary>
/// An object representing the staff member attributed to the line item
/// </summary>
public class LineItemAttributedStaff
{
    /// <summary>
    /// The GraphQL id of the staff member
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// The quantity of the line item attributed to the staff member.
    /// </summary>
    [JsonProperty("quantity")]
    public long? Quantity { get; set; }
}