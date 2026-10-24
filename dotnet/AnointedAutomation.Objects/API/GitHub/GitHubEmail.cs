// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

using Newtonsoft.Json;

namespace AnointedAutomation.Objects.GitHub
{
    /// <summary>
    /// One entry of GitHub's <c>GET /user/emails</c> response ("Email" schema,
    /// <c>https://docs.github.com/en/rest/users/emails#list-email-addresses-for-the-authenticated-user</c>).
    /// </summary>
    public class GitHubEmail
    {
        /// <summary>The email address.</summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>Whether this is the account's primary address.</summary>
        [JsonProperty("primary")]
        public bool? Primary { get; set; }

        /// <summary>Whether GitHub has verified this address.</summary>
        [JsonProperty("verified")]
        public bool? Verified { get; set; }

        /// <summary>The address's visibility (<c>public</c>, <c>private</c>), or null.</summary>
        [JsonProperty("visibility")]
        public string Visibility { get; set; }
    }
}
