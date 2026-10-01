// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Apple
{
    /// <summary>
    /// The validated claim set from a Sign in with Apple id_token. Fields are the claims Apple documents
    /// it returns (developer.apple.com "Authenticating users with Sign in with Apple" / the id_token
    /// payload): <c>sub</c>, <c>iss</c>, <c>aud</c>, <c>email</c>, <c>email_verified</c>,
    /// <c>is_private_email</c>, <c>real_user_status</c> and <c>auth_time</c>. Apple sends the booleans and
    /// numbers as JSON strings, so they are captured as strings.
    /// </summary>
    public class AppleTokenInfo
    {
        /// <summary>Apple's stable subject id (the durable link key).</summary>
        [JsonProperty("sub")]
        public string sub { get; set; }

        /// <summary>The token issuer (<c>https://appleid.apple.com</c>).</summary>
        [JsonProperty("iss")]
        public string iss { get; set; }

        /// <summary>The audience the token was issued for (our client / service id).</summary>
        [JsonProperty("aud")]
        public string aud { get; set; }

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

        /// <summary>
        /// Initializes a new instance of the <see cref="AppleTokenInfo"/> class.
        /// </summary>
        public AppleTokenInfo()
        {
        }
    }
}
