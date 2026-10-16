// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

#nullable enable

using System.Collections.Generic;
using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Twitch
{
    /// <summary>
    /// The Twitch token endpoint (<c>POST https://id.twitch.tv/oauth2/token</c>) answer for the authorization_code
    /// grant (<c>https://dev.twitch.tv/docs/authentication/getting-tokens-oidc/</c>). Transport only: tokens are used
    /// once and never stored (no BSON mapping).
    /// </summary>
    public class TwitchTokenResponse
    {
        /// <summary>The user access token (never stored).</summary>
        [JsonProperty("access_token")]
        public string? AccessToken { get; set; }

        /// <summary>Seconds until the access token expires.</summary>
        [JsonProperty("expires_in")]
        public long? ExpiresIn { get; set; }

        /// <summary>The signed OIDC id_token (present because the <c>openid</c> scope was requested).</summary>
        [JsonProperty("id_token")]
        public string? IdToken { get; set; }

        /// <summary>The refresh token (never stored).</summary>
        [JsonProperty("refresh_token")]
        public string? RefreshToken { get; set; }

        /// <summary>Twitch answers the granted scopes as a JSON array.</summary>
        [JsonProperty("scope")]
        public List<string>? Scope { get; set; }

        /// <summary>The token type, <c>bearer</c>.</summary>
        [JsonProperty("token_type")]
        public string? TokenType { get; set; }

        /// <summary>Error answers carry <c>status</c> + <c>message</c>.</summary>
        [JsonProperty("status")]
        public int? Status { get; set; }

        /// <summary>Error text on a refused exchange (e.g. <c>Invalid authorization code</c>).</summary>
        [JsonProperty("message")]
        public string? Message { get; set; }
    }
}
