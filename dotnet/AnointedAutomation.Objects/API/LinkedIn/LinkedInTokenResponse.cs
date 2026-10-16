// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

#nullable enable

using Newtonsoft.Json;

namespace AnointedAutomation.Objects.LinkedIn
{
    /// <summary>
    /// The LinkedIn token endpoint (<c>POST https://www.linkedin.com/oauth/v2/accessToken</c>) answer, every documented
    /// field (<c>https://learn.microsoft.com/en-us/linkedin/shared/authentication/authorization-code-flow</c>).
    /// Transport only: the tokens are used once and never stored (no BSON mapping).
    /// </summary>
    public class LinkedInTokenResponse
    {
        /// <summary>The member access token (never stored).</summary>
        [JsonProperty("access_token")]
        public string? AccessToken { get; set; }

        /// <summary>Seconds until the access token expires.</summary>
        [JsonProperty("expires_in")]
        public long? ExpiresIn { get; set; }

        /// <summary>The refresh token, for apps approved for programmatic refresh (never stored).</summary>
        [JsonProperty("refresh_token")]
        public string? RefreshToken { get; set; }

        /// <summary>Seconds until the refresh token expires.</summary>
        [JsonProperty("refresh_token_expires_in")]
        public long? RefreshTokenExpiresIn { get; set; }

        /// <summary>The scopes LinkedIn granted.</summary>
        [JsonProperty("scope")]
        public string? Scope { get; set; }

        /// <summary>The token type. NOT in LinkedIn's field table; kept because RFC 6749 section 5.1 requires it in
        /// every successful token answer (https://datatracker.ietf.org/doc/html/rfc6749#section-5.1).</summary>
        [JsonProperty("token_type")]
        public string? TokenType { get; set; }

        /// <summary>The OIDC id_token (returned when the <c>openid</c> scope was granted). Not needed here: the
        /// identity is read from userinfo with an access token only OUR client secret could obtain.</summary>
        [JsonProperty("id_token")]
        public string? IdToken { get; set; }

        /// <summary>Error code on a refused exchange (e.g. <c>invalid_request</c>).</summary>
        [JsonProperty("error")]
        public string? Error { get; set; }

        /// <summary>Human-readable error text on a refused exchange.</summary>
        [JsonProperty("error_description")]
        public string? ErrorDescription { get; set; }
    }
}
