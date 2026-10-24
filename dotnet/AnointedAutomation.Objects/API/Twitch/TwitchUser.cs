// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

using System;
using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Twitch
{
    /// <summary>
    /// One user of Twitch's Get Users response (<c>https://dev.twitch.tv/docs/api/reference/#get-users</c>,
    /// <c>data[]</c>). Field names follow Twitch's snake_case wire JSON.
    /// </summary>
    public class TwitchUser
    {
        /// <summary>An ID that identifies the user.</summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>The user's login name.</summary>
        [JsonProperty("login")]
        public string Login { get; set; }

        /// <summary>The user's display name.</summary>
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        /// <summary>The type of user: <c>admin</c>, <c>global_mod</c>, <c>staff</c>, or empty for a normal user.</summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>The type of broadcaster: <c>affiliate</c>, <c>partner</c>, or empty for a normal broadcaster.</summary>
        [JsonProperty("broadcaster_type")]
        public string BroadcasterType { get; set; }

        /// <summary>The user's description of their channel.</summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>A URL to the user's profile image.</summary>
        [JsonProperty("profile_image_url")]
        public string ProfileImageUrl { get; set; }

        /// <summary>A URL to the user's offline image.</summary>
        [JsonProperty("offline_image_url")]
        public string OfflineImageUrl { get; set; }

        /// <summary>Deprecated by Twitch; any value is not valid.</summary>
        [JsonProperty("view_count")]
        public long? ViewCount { get; set; }

        /// <summary>
        /// The user's VERIFIED email address; present only when the user access token has the
        /// <c>user:read:email</c> scope.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>When the account was created (UTC, RFC3339).</summary>
        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }
    }
}
