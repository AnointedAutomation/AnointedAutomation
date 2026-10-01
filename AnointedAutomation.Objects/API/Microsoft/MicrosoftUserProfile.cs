// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Microsoft
{
    /// <summary>
    /// The human profile for a Microsoft (Azure AD / Entra) user, built from the identity-platform
    /// profile claims Microsoft returns (learn.microsoft.com "ID token claims reference"): the display
    /// <c>name</c>, <c>given_name</c>, <c>family_name</c>, <c>email</c>, and the <c>picture</c> URL when
    /// the account exposes one.
    /// </summary>
    public class MicrosoftUserProfile
    {
        /// <summary>The user's full display name.</summary>
        [JsonProperty("name")]
        public string name { get; set; }

        /// <summary>The user's given (first) name.</summary>
        [JsonProperty("given_name")]
        public string givenName { get; set; }

        /// <summary>The user's family (last) name.</summary>
        [JsonProperty("family_name")]
        public string familyName { get; set; }

        /// <summary>The user's email address.</summary>
        [JsonProperty("email")]
        public string email { get; set; }

        /// <summary>The URL to the user's profile picture, when available.</summary>
        [JsonProperty("picture")]
        public string picture { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MicrosoftUserProfile"/> class.
        /// </summary>
        public MicrosoftUserProfile()
        {
        }
    }
}
