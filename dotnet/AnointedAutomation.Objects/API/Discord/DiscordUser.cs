// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Discord
{
    /// <summary>
    /// Discord's full documented User object (<c>https://docs.discord.com/developers/resources/user</c>, "User
    /// Object Structure"), as the OAuth2 <c>GET /users/@me</c> endpoint returns it. Field names follow Discord's
    /// snake_case wire JSON; snowflakes are strings. Optional or nullable fields are null when Discord omits
    /// them (e.g. <c>email</c> and <c>verified</c> need the <c>email</c> scope, <c>premium_type</c> needs
    /// <c>identify.premium</c>).
    /// </summary>
    public class DiscordUser
    {
        /// <summary>The user's id (snowflake), stable across every app.</summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>The user's username, not unique across the platform.</summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>The user's Discord tag (legacy 4-digit discriminator, <c>"0"</c> for migrated usernames).</summary>
        [JsonProperty("discriminator")]
        public string Discriminator { get; set; }

        /// <summary>The user's display name, if it is set. For bots, this is the application name.</summary>
        [JsonProperty("global_name")]
        public string GlobalName { get; set; }

        /// <summary>The user's avatar hash, null when none is set.</summary>
        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        /// <summary>Whether the user belongs to an OAuth2 application.</summary>
        [JsonProperty("bot")]
        public bool? Bot { get; set; }

        /// <summary>Whether the user is an Official Discord System user (part of the urgent message system).</summary>
        [JsonProperty("system")]
        public bool? System { get; set; }

        /// <summary>Whether the user has two factor enabled on their account.</summary>
        [JsonProperty("mfa_enabled")]
        public bool? MfaEnabled { get; set; }

        /// <summary>The user's banner hash.</summary>
        [JsonProperty("banner")]
        public string Banner { get; set; }

        /// <summary>The user's banner color encoded as an integer representation of a hexadecimal color code.</summary>
        [JsonProperty("accent_color")]
        public int? AccentColor { get; set; }

        /// <summary>The user's chosen language option.</summary>
        [JsonProperty("locale")]
        public string Locale { get; set; }

        /// <summary>Whether the email on this account has been verified (<c>email</c> scope).</summary>
        [JsonProperty("verified")]
        public bool? Verified { get; set; }

        /// <summary>The user's email (<c>email</c> scope).</summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>The flags on the user's account (bit field).</summary>
        [JsonProperty("flags")]
        public long? Flags { get; set; }

        /// <summary>The type of Nitro subscription on the user's account (0 None, 1 Nitro Classic, 2 Nitro, 3 Nitro Basic).</summary>
        [JsonProperty("premium_type")]
        public int? PremiumType { get; set; }

        /// <summary>The public flags on the user's account (bit field).</summary>
        [JsonProperty("public_flags")]
        public long? PublicFlags { get; set; }

        /// <summary>Data for the user's avatar decoration.</summary>
        [JsonProperty("avatar_decoration_data")]
        public DiscordAvatarDecorationData AvatarDecorationData { get; set; }

        /// <summary>Data for the user's collectibles.</summary>
        [JsonProperty("collectibles")]
        public DiscordCollectibles Collectibles { get; set; }

        /// <summary>The user's primary guild (server tag).</summary>
        [JsonProperty("primary_guild")]
        public DiscordPrimaryGuild PrimaryGuild { get; set; }
    }

    /// <summary>Discord "Avatar Decoration Data Structure".</summary>
    public class DiscordAvatarDecorationData
    {
        /// <summary>The avatar decoration hash.</summary>
        [JsonProperty("asset")]
        public string Asset { get; set; }

        /// <summary>The id (snowflake) of the avatar decoration's SKU.</summary>
        [JsonProperty("sku_id")]
        public string SkuId { get; set; }
    }

    /// <summary>Discord "Collectibles Structure": the collectibles the user has, excluding avatar decorations and profile effects.</summary>
    public class DiscordCollectibles
    {
        /// <summary>The user's nameplate.</summary>
        [JsonProperty("nameplate")]
        public DiscordNameplate Nameplate { get; set; }
    }

    /// <summary>Discord "Nameplate Structure".</summary>
    public class DiscordNameplate
    {
        /// <summary>The id (snowflake) of the nameplate SKU.</summary>
        [JsonProperty("sku_id")]
        public string SkuId { get; set; }

        /// <summary>The path to the nameplate asset.</summary>
        [JsonProperty("asset")]
        public string Asset { get; set; }

        /// <summary>The label of this nameplate (currently unused).</summary>
        [JsonProperty("label")]
        public string Label { get; set; }

        /// <summary>The background color of the nameplate (crimson, berry, sky, teal, forest, bubble_gum, violet, cobalt, clover, lemon, white).</summary>
        [JsonProperty("palette")]
        public string Palette { get; set; }
    }

    /// <summary>Discord "User Primary Guild Structure".</summary>
    public class DiscordPrimaryGuild
    {
        /// <summary>The id (snowflake) of the user's primary guild.</summary>
        [JsonProperty("identity_guild_id")]
        public string IdentityGuildId { get; set; }

        /// <summary>
        /// Whether the user is displaying the primary guild's server tag. Null when the system cleared the
        /// identity (e.g. the guild disabled tags); false when the user removed it themselves.
        /// </summary>
        [JsonProperty("identity_enabled")]
        public bool? IdentityEnabled { get; set; }

        /// <summary>The text of the user's server tag (up to 4 characters).</summary>
        [JsonProperty("tag")]
        public string Tag { get; set; }

        /// <summary>The server tag badge hash.</summary>
        [JsonProperty("badge")]
        public string Badge { get; set; }
    }
}
