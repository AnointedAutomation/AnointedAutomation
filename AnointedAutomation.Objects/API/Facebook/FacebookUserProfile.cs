// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Facebook
{
    /// <summary>
    /// The human profile for a Facebook Login user, from the Graph <c>/me</c> endpoint fields documented
    /// at developers.facebook.com (the User node): <c>id</c>, <c>name</c>, <c>first_name</c>,
    /// <c>last_name</c>, <c>email</c> (Facebook can withhold it), and the picture URL (which Graph nests
    /// under <c>picture.data.url</c>).
    /// </summary>
    public class FacebookUserProfile
    {
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
        [JsonProperty("pictureUrl")]
        public string pictureUrl { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FacebookUserProfile"/> class.
        /// </summary>
        public FacebookUserProfile()
        {
        }
    }
}
