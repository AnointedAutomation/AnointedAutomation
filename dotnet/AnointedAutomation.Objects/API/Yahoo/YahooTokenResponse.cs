// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

#nullable enable

using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Yahoo
{
    /// <summary>
    /// The Yahoo token endpoint (<c>POST https://api.login.yahoo.com/oauth2/get_token</c>) answer for the
    /// authorization_code grant (<c>https://developer.yahoo.com/oauth2/guide/openid_connect/getting_started.html</c>).
    /// Transport only: tokens are used once and never stored (no BSON mapping).
    /// </summary>
    public class YahooTokenResponse
    {
        /// <summary>The access token (never stored).</summary>
        [JsonProperty("access_token")]
        public string? AccessToken { get; set; }

        /// <summary>The refresh token (never stored).</summary>
        [JsonProperty("refresh_token")]
        public string? RefreshToken { get; set; }

        /// <summary>Seconds until the access token expires.</summary>
        [JsonProperty("expires_in")]
        public long? ExpiresIn { get; set; }

        /// <summary>The token type, <c>bearer</c>.</summary>
        [JsonProperty("token_type")]
        public string? TokenType { get; set; }

        /// <summary>Yahoo's own GUID for the member (legacy; the OIDC <c>sub</c> is the subject we keep).</summary>
        [JsonProperty("xoauth_yahoo_guid")]
        public string? XoauthYahooGuid { get; set; }

        /// <summary>The signed OIDC id_token (present because the <c>openid</c> scope was requested).</summary>
        [JsonProperty("id_token")]
        public string? IdToken { get; set; }

        /// <summary>Error code on a refused exchange (e.g. <c>invalid_grant</c>). Yahoo's guides document NO error body;
        /// this and <see cref="ErrorDescription"/> are the RFC 6749 section 5.2 standard error fields
        /// (https://datatracker.ietf.org/doc/html/rfc6749#section-5.2).</summary>
        [JsonProperty("error")]
        public string? Error { get; set; }

        /// <summary>Human-readable error text on a refused exchange (RFC 6749 section 5.2, not Yahoo-documented).</summary>
        [JsonProperty("error_description")]
        public string? ErrorDescription { get; set; }
    }
}
