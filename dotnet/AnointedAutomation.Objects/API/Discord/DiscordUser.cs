// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Discord
{
    /// <summary>
    /// A Discord user as the OAuth2 <c>GET /users/@me</c> endpoint returns it (Discord API v10 User object),
    /// limited to the fields a sign-in reads. Field names follow Discord's snake_case wire JSON. Every member
    /// is optional: <c>email</c> and <c>verified</c> are present only when the token carries the <c>email</c> scope.
    /// </summary>
    public class DiscordUser
    {
        /// <summary>The Discord snowflake user id, a STRING in Discord JSON, stable across every app.</summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>The username (new-style unique name; no discriminator for migrated accounts).</summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>The legacy 4-digit discriminator, <c>"0"</c> for migrated usernames.</summary>
        [JsonProperty("discriminator")]
        public string Discriminator { get; set; }

        /// <summary>The display ("global") name, when the account has one.</summary>
        [JsonProperty("global_name")]
        public string GlobalName { get; set; }

        /// <summary>The account email, only with the <c>email</c> scope.</summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>Whether Discord verified the account email, only with the <c>email</c> scope.</summary>
        [JsonProperty("verified")]
        public bool? Verified { get; set; }

        /// <summary>The avatar hash (builds the CDN avatar URL), null when none is set.</summary>
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
    }
}
