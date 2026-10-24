// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

#nullable enable

using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Reddit
{
    /// <summary>
    /// The Reddit token endpoint (<c>POST https://www.reddit.com/api/v1/access_token</c>) answer
    /// (<c>https://github.com/reddit-archive/reddit/wiki/OAuth2</c>, "Retrieving the access token"). <c>refresh_token</c>
    /// is present only for <c>duration=permanent</c>. Errors are named by the <c>error</c> field
    /// (<c>unsupported_grant_type</c>, <c>invalid_grant</c>); bad client credentials answer HTTP 401. Transport only:
    /// tokens are used once and never stored (no BSON mapping).
    /// </summary>
    public class RedditTokenResponse
    {
        /// <summary>The access token (never stored).</summary>
        [JsonProperty("access_token")]
        public string? AccessToken { get; set; }

        /// <summary>The token type, <c>bearer</c>.</summary>
        [JsonProperty("token_type")]
        public string? TokenType { get; set; }

        /// <summary>Seconds until the access token expires.</summary>
        [JsonProperty("expires_in")]
        public long? ExpiresIn { get; set; }

        /// <summary>The space-separated scopes Reddit granted.</summary>
        [JsonProperty("scope")]
        public string? Scope { get; set; }

        /// <summary>The refresh token, only for <c>duration=permanent</c> (never stored).</summary>
        [JsonProperty("refresh_token")]
        public string? RefreshToken { get; set; }

        /// <summary>Error code on a refused exchange (e.g. <c>invalid_grant</c>).</summary>
        [JsonProperty("error")]
        public string? Error { get; set; }
    }
}
