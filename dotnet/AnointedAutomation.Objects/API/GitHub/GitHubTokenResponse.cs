// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

#nullable enable

using Newtonsoft.Json;

namespace AnointedAutomation.Objects.GitHub
{
    /// <summary>
    /// The GitHub OAuth token exchange answer (<c>POST https://github.com/login/oauth/access_token</c> with
    /// <c>Accept: application/json</c>,
    /// <c>https://docs.github.com/en/apps/oauth-apps/building-oauth-apps/authorizing-oauth-apps</c>). GitHub can answer
    /// a bad or expired code with an <c>error</c> body instead of a token (bad_verification_code,
    /// redirect_uri_mismatch,
    /// <c>https://docs.github.com/en/apps/oauth-apps/maintaining-oauth-apps/troubleshooting-oauth-app-access-token-request-errors</c>),
    /// so both are modelled. Transport only: the token is used once and never stored (no BSON mapping).
    /// </summary>
    public class GitHubTokenResponse
    {
        /// <summary>The user access token (never stored).</summary>
        [JsonProperty("access_token")]
        public string? AccessToken { get; set; }

        /// <summary>The comma-separated scopes GitHub granted.</summary>
        [JsonProperty("scope")]
        public string? Scope { get; set; }

        /// <summary>The token type, <c>bearer</c>.</summary>
        [JsonProperty("token_type")]
        public string? TokenType { get; set; }

        /// <summary>Error code on a refused exchange (e.g. <c>bad_verification_code</c>).</summary>
        [JsonProperty("error")]
        public string? Error { get; set; }

        /// <summary>Human-readable error text on a refused exchange.</summary>
        [JsonProperty("error_description")]
        public string? ErrorDescription { get; set; }

        /// <summary>Link to GitHub's page for the error (documented on
        /// https://docs.github.com/en/apps/oauth-apps/maintaining-oauth-apps/troubleshooting-oauth-app-access-token-request-errors).</summary>
        [JsonProperty("error_uri")]
        public string? ErrorUri { get; set; }
    }
}
