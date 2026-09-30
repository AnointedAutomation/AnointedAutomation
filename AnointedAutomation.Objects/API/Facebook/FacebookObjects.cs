// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Facebook
{
    /// <summary>
    /// The real Facebook Login identity for a user who signed in with Facebook, holding the ACTUAL fields
    /// the Graph <c>/me</c> endpoint returns. This is the object stored at <c>User.SSO.Facebook</c>. The
    /// stable link key is the app-scoped <see cref="id"/> from <c>/me</c>. Mirrors the structure style of
    /// <see cref="AnointedAutomation.Objects.Google.GoogleObjects"/>.
    /// </summary>
    public class FacebookObjects
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FacebookObjects"/> class.
        /// </summary>
        public FacebookObjects()
        {
        }

        /// <summary>The app-scoped Facebook user id (the durable link key).</summary>
        [JsonProperty("id")]
        public string id { get; set; }

        /// <summary>The user's full name.</summary>
        [JsonProperty("name")]
        public string name { get; set; }

        /// <summary>The user's first name.</summary>
        [JsonProperty("first_name")]
        public string firstName { get; set; }

        /// <summary>The user's last name.</summary>
        [JsonProperty("last_name")]
        public string lastName { get; set; }

        /// <summary>The email Facebook shares (Facebook can withhold it).</summary>
        [JsonProperty("email")]
        public string email { get; set; }

        /// <summary>The profile picture URL returned by the Graph API, when requested.</summary>
        [JsonProperty("picture")]
        public string picture { get; set; }

        /// <summary>When this identity was first linked to the user.</summary>
        [JsonProperty("linkedAt")]
        public System.DateTime? linkedAt { get; set; }
    }
}
