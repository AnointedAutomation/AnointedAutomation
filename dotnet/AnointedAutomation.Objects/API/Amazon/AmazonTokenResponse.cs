// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

#nullable enable

using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Amazon
{
    /// <summary>
    /// The Login with Amazon token endpoint (<c>POST https://api.amazon.com/auth/o2/token</c>) answer for the
    /// authorization code grant
    /// (<c>https://developer.amazon.com/docs/login-with-amazon/authorization-code-grant.html</c>, "Access Token
    /// Response" and "Access Token Errors"). Transport only: tokens are used once and never stored (no BSON mapping).
    /// </summary>
    public class AmazonTokenResponse
    {
        /// <summary>The access token for the user account (max 2048 bytes; never stored).</summary>
        [JsonProperty("access_token")]
        public string? AccessToken { get; set; }

        /// <summary>The token type, <c>bearer</c>.</summary>
        [JsonProperty("token_type")]
        public string? TokenType { get; set; }

        /// <summary>Seconds before the access token becomes invalid.</summary>
        [JsonProperty("expires_in")]
        public long? ExpiresIn { get; set; }

        /// <summary>A refresh token for a new access token (max 2048 bytes; never stored).</summary>
        [JsonProperty("refresh_token")]
        public string? RefreshToken { get; set; }

        /// <summary>Error code on a refused exchange (<c>invalid_request</c>, <c>invalid_client</c>,
        /// <c>invalid_grant</c>, <c>unauthorized_client</c>, <c>unsupported_grant_type</c>, <c>ServerError</c>).</summary>
        [JsonProperty("error")]
        public string? Error { get; set; }

        /// <summary>Human-readable error text for the developer.</summary>
        [JsonProperty("error_description")]
        public string? ErrorDescription { get; set; }

        /// <summary>A URI to a page with more information about the error.</summary>
        [JsonProperty("error_uri")]
        public string? ErrorUri { get; set; }
    }
}
