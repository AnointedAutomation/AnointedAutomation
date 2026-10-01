// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Apple
{
    /// <summary>
    /// The <c>user</c> object Apple posts ONCE, on the user's first consent, outside the identity token:
    /// <c>{ "name": { "firstName", "lastName" }, "email" }</c>. Apple never sends it again, so it must be
    /// captured on that first authorization.
    /// </summary>
    public class AppleUser
    {
        /// <summary>The user's name as entered on first consent.</summary>
        [JsonProperty("name")]
        public AppleUserName Name { get; set; }

        /// <summary>The email shared on first consent.</summary>
        [JsonProperty("email")]
        public string Email { get; set; }
    }

    /// <summary>The <c>name</c> member of Apple's first-consent <see cref="AppleUser"/> object.</summary>
    public class AppleUserName
    {
        /// <summary>Given name.</summary>
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        /// <summary>Family name.</summary>
        [JsonProperty("lastName")]
        public string LastName { get; set; }
    }
}
