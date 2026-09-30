// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Apple
{
    /// <summary>
    /// The real Sign in with Apple identity for a user who signed in with Apple, holding the ACTUAL
    /// claims Apple returns in the id_token plus the first-consent name (which Apple sends only ONCE, on
    /// the first authorization, and never inside the token). This is the object stored at
    /// <c>User.SSO.Apple</c>. The stable link key is <see cref="sub"/>. Mirrors the structure style of
    /// <see cref="AnointedAutomation.Objects.Google.GoogleObjects"/>.
    /// </summary>
    public class AppleObjects
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppleObjects"/> class.
        /// </summary>
        public AppleObjects()
        {
        }

        /// <summary>Apple's stable subject id (the durable link key).</summary>
        [JsonProperty("sub")]
        public string sub { get; set; }

        /// <summary>The email Apple reports (may be a private relay address).</summary>
        [JsonProperty("email")]
        public string email { get; set; }

        /// <summary>Whether Apple has verified the email. Apple sends this as a string ("true"/"false").</summary>
        [JsonProperty("email_verified")]
        public string emailVerified { get; set; }

        /// <summary>Whether the email is an Apple private-relay address. Apple sends this as a string.</summary>
        [JsonProperty("is_private_email")]
        public string isPrivateEmail { get; set; }

        /// <summary>Apple's real-user status signal (0 unsupported, 1 unknown, 2 likely real).</summary>
        [JsonProperty("real_user_status")]
        public string realUserStatus { get; set; }

        /// <summary>The auth_time claim (seconds since epoch, as Apple sends it).</summary>
        [JsonProperty("auth_time")]
        public string authTime { get; set; }

        /// <summary>The first (given) name, captured only on the FIRST authorization; blank thereafter.</summary>
        [JsonProperty("firstName")]
        public string firstName { get; set; }

        /// <summary>The last (family) name, captured only on the FIRST authorization; blank thereafter.</summary>
        [JsonProperty("lastName")]
        public string lastName { get; set; }

        /// <summary>When this identity was first linked to the user.</summary>
        [JsonProperty("linkedAt")]
        public System.DateTime? linkedAt { get; set; }
    }
}
