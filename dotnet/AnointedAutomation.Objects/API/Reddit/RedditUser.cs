// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Reddit
{
    /// <summary>
    /// Reddit's account identity as <c>GET /api/v1/me</c> returns it (<c>https://www.reddit.com/dev/api#GET_api_v1_me</c>),
    /// field-by-field from the documented "account" thing (<c>https://github.com/reddit-archive/reddit/wiki/JSON</c>)
    /// plus the identity fields <c>/api/v1/me</c> adds (<c>icon_img</c>, <c>snoovatar_img</c>, <c>verified</c>,
    /// <c>total_karma</c>, <c>is_employee</c>). Field names follow Reddit's snake_case wire JSON; times are UNIX epoch
    /// seconds. Reddit returns NO email address; <c>has_verified_email</c> only says one exists. The CSRF
    /// <c>modhash</c> is deliberately not modeled (never stored).
    /// </summary>
    public class RedditUser
    {
        /// <summary>ID of the account; prepend <c>t2_</c> to get the fullname.</summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>The account's username.</summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>Creation time, epoch seconds (local server time).</summary>
        [JsonProperty("created")]
        public double? Created { get; set; }

        /// <summary>Creation time, epoch seconds UTC.</summary>
        [JsonProperty("created_utc")]
        public double? CreatedUtc { get; set; }

        /// <summary>The user's link karma.</summary>
        [JsonProperty("link_karma")]
        public int? LinkKarma { get; set; }

        /// <summary>The user's comment karma.</summary>
        [JsonProperty("comment_karma")]
        public int? CommentKarma { get; set; }

        /// <summary>The user's total karma.</summary>
        [JsonProperty("total_karma")]
        public int? TotalKarma { get; set; }

        /// <summary>Reddit gold status.</summary>
        [JsonProperty("is_gold")]
        public bool? IsGold { get; set; }

        /// <summary>Whether this account moderates any subreddits.</summary>
        [JsonProperty("is_mod")]
        public bool? IsMod { get; set; }

        /// <summary>Whether this account is a Reddit employee.</summary>
        [JsonProperty("is_employee")]
        public bool? IsEmployee { get; set; }

        /// <summary>Whether the user has provided an email address and got it verified (the address itself is never returned).</summary>
        [JsonProperty("has_verified_email")]
        public bool? HasVerifiedEmail { get; set; }

        /// <summary>Whether the account is verified.</summary>
        [JsonProperty("verified")]
        public bool? Verified { get; set; }

        /// <summary>Whether this account is set to be over 18.</summary>
        [JsonProperty("over_18")]
        public bool? Over18 { get; set; }

        /// <summary>Whether the user has unread mail.</summary>
        [JsonProperty("has_mail")]
        public bool? HasMail { get; set; }

        /// <summary>Whether the user has unread mod mail.</summary>
        [JsonProperty("has_mod_mail")]
        public bool? HasModMail { get; set; }

        /// <summary>Number of unread messages in the inbox.</summary>
        [JsonProperty("inbox_count")]
        public int? InboxCount { get; set; }

        /// <summary>The user's avatar image URL.</summary>
        [JsonProperty("icon_img")]
        public string IconImg { get; set; }

        /// <summary>The user's Snoovatar image URL, when one is set.</summary>
        [JsonProperty("snoovatar_img")]
        public string SnoovatarImg { get; set; }
    }
}
