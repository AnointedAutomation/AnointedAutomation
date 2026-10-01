// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Apple
{
    /// <summary>
    /// The human profile for a Sign in with Apple user. Apple returns the name ONLY on the first
    /// authorization, in the authorization response's <c>user.name</c> object (givenName / familyName),
    /// and NEVER inside the id_token (developer.apple.com Sign in with Apple docs). So these fields are
    /// populated on first consent and blank thereafter; the email mirrors the token email for convenience.
    /// </summary>
    public class AppleUserProfile
    {
        /// <summary>The first (given) name, captured only on the FIRST authorization; blank thereafter.</summary>
        [JsonProperty("firstName")]
        public string firstName { get; set; }

        /// <summary>The last (family) name, captured only on the FIRST authorization; blank thereafter.</summary>
        [JsonProperty("lastName")]
        public string lastName { get; set; }

        /// <summary>The email Apple reports (may be a private relay address).</summary>
        [JsonProperty("email")]
        public string email { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppleUserProfile"/> class.
        /// </summary>
        public AppleUserProfile()
        {
        }
    }
}
