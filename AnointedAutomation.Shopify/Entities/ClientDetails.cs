// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json;

namespace AnointedAutomation.Shopify;

public class ClientDetails
{
    /// <summary>
    /// Shopify does not offer documentation for this field.
    /// </summary>
    [JsonProperty("accept_language")]
    public string AcceptLanguage { get; set; }

    /// <summary>
    /// The browser screen height in pixels, if available.
    /// </summary>
    [JsonProperty("browser_height")]
    public int? BrowserHeight { get; set; }

    /// <summary>
    /// The browser IP address.
    /// </summary>
    [JsonProperty("browser_ip")]
    public string BrowserIp { get; set; }

    /// <summary>
    /// The browser screen width in pixels, if available.
    /// </summary>
    [JsonProperty("browser_width")]
    public int? BrowserWidth { get; set; }

    /// <summary>
    /// A hash of the session.
    /// </summary>
    [JsonProperty("session_hash")]
    public string SessionHash { get; set; }

    /// <summary>
    /// The browser's user agent string.
    /// </summary>
    [JsonProperty("user_agent")]
    public string UserAgent { get; set; }
}