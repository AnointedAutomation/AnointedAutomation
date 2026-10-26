// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation Corporation. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

/// <summary>
/// An object representing a note attribute for <see cref="Order.NoteAttributes"/>
/// </summary>
public class NoteAttribute
{
    /// <summary>
    /// The name of the note attribute.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// The value of the note attribute.
    /// </summary>
    [JsonProperty("value")]
    public object Value { get; set; }
}