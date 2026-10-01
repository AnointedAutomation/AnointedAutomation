// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

/// <summary>
/// An object representing a properties for <see cref="LineItem.Properties"/>
/// </summary>
public class LineItemProperty
{
    /// <summary>
    /// The name of the note attribute.
    /// </summary>
    [JsonProperty("name")]
    public object Name { get; set; }

    /// <summary>
    /// The value of the note attribute.
    /// </summary>
    [JsonProperty("value")]
    public object Value { get; set; }
}