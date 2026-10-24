// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

#nullable enable

using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Discord
{
    /// <summary>
    /// The Discord OAuth2 token endpoint (<c>POST https://discord.com/api/oauth2/token</c>) response
    /// (<c>https://discord.com/developers/docs/topics/oauth2#authorization-code-grant-access-token-response</c>).
    /// Transport only: the tokens are used once and never stored (no BSON mapping). The identity slot types
    /// (<c>DiscordUser</c>, <c>DiscordLogin</c>) are stored at <c>User.SSO.Discord</c>.
    /// </summary>
    public class DiscordTokenResponse
    {
        /// <summary>The user access token (used once, for the guild join; never stored).</summary>
        [JsonProperty("access_token")]
        public string? AccessToken { get; set; }

        /// <summary>The token type, <c>Bearer</c>.</summary>
        [JsonProperty("token_type")]
        public string? TokenType { get; set; }

        /// <summary>Lifetime of the access token in seconds.</summary>
        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }

        /// <summary>The refresh token (never stored).</summary>
        [JsonProperty("refresh_token")]
        public string? RefreshToken { get; set; }

        /// <summary>The space-separated scopes Discord granted.</summary>
        [JsonProperty("scope")]
        public string? Scope { get; set; }
    }
}
