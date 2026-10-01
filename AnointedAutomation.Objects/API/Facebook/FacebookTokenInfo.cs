// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

using System.Collections.Generic;
using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Facebook
{
    /// <summary>
    /// The access-token metadata for a Facebook Login user, from the Graph <c>debug_token</c> response
    /// documented at developers.facebook.com ("Getting Info about Access Tokens and Debugging"): the
    /// owning <c>app_id</c>, the app-scoped <c>user_id</c>, whether the token <c>is_valid</c>, and the
    /// granted <c>scopes</c>. A raw-fields dictionary captures anything else the debug response carries.
    /// Facebook does not issue a JWT, so this is debug metadata rather than a decoded token.
    /// </summary>
    public class FacebookTokenInfo
    {
        /// <summary>The Facebook app id the access token was minted for.</summary>
        [JsonProperty("app_id")]
        public string appId { get; set; }

        /// <summary>The app-scoped Facebook user id (matches Graph <c>/me</c> <c>id</c>); the durable link key.</summary>
        [JsonProperty("user_id")]
        public string appScopedId { get; set; }

        /// <summary>Whether the debug_token response reported the token as valid.</summary>
        [JsonProperty("is_valid")]
        public bool? isValid { get; set; }

        /// <summary>The permissions granted to the token, as Facebook returns them.</summary>
        [JsonProperty("scopes")]
        public List<string> scopes { get; set; }

        /// <summary>Any other debug_token fields, captured raw so nothing Facebook returns is lost. May be null.</summary>
        [JsonProperty("rawFields")]
        public Dictionary<string, string> rawFields { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FacebookTokenInfo"/> class.
        /// </summary>
        public FacebookTokenInfo()
        {
        }
    }
}
