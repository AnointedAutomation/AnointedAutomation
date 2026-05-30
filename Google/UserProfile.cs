// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

using Newtonsoft.Json;

namespace Google.Integrations
{
    //* JsonProperty(PropertyName = "name") attribute used here
    //* to map json property name with object property name
    //* while deseralizing a json into object
    /// <summary>
    /// Represents a Google user profile with personal information.
    /// </summary>
    public partial class UserProfile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfile"/> class.
        /// </summary>
        public UserProfile()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfile"/> class with specified properties.
        /// </summary>
        /// <param name="id">The user's Google ID.</param>
        /// <param name="email">The user's email address.</param>
        /// <param name="verifiedEmail">Whether the email is verified.</param>
        /// <param name="name">The user's full name.</param>
        /// <param name="givenName">The user's given (first) name.</param>
        /// <param name="familyName">The user's family (last) name.</param>
        /// <param name="link">The URL to the user's Google profile.</param>
        /// <param name="picture">The URL to the user's profile picture.</param>
        /// <param name="gender">The user's gender.</param>
        /// <param name="locale">The user's locale preference.</param>
        /// <param name="hd">The hosted domain for G Suite users.</param>
        public UserProfile(string id, string email, bool verifiedEmail, string name, string givenName, string familyName, string link, string picture, string gender, string locale, string hd)
        {
            this.id = id;
            this.email = email;
            this.verifiedEmail = verifiedEmail;
            this.name = name;
            this.givenName = givenName;
            this.familyName = familyName;
            this.link = link;
            this.picture = picture;
            this.gender = gender;
            this.locale = locale;
            this.hd = hd;
        }

        [JsonProperty("id")]
        /// <summary>
        /// Gets or sets the user's Google ID.
        /// </summary>
        public string id
        {
            get; set;
        }

        [JsonProperty("email")]
        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        public string email
        {
            get; set;
        }

        [JsonProperty("verified_email")]
        /// <summary>
        /// Gets or sets a value indicating whether the user's email is verified.
        /// </summary>
        public bool verifiedEmail
        {
            get; set;
        }

        [JsonProperty("name")]
        /// <summary>
        /// Gets or sets the user's full name.
        /// </summary>
        public string name
        {
            get; set;
        }

        [JsonProperty("given_name")]
        /// <summary>
        /// Gets or sets the user's given (first) name.
        /// </summary>
        public string givenName
        {
            get; set;
        }

        [JsonProperty("family_name")]
        /// <summary>
        /// Gets or sets the user's family (last) name.
        /// </summary>
        public string familyName
        {
            get; set;
        }

        [JsonProperty("link")]
        /// <summary>
        /// Gets or sets the URL to the user's Google profile.
        /// </summary>
        public string link
        {
            get; set;
        }

        [JsonProperty("picture")]
        /// <summary>
        /// Gets or sets the URL to the user's profile picture.
        /// </summary>
        public string picture
        {
            get; set;
        }

        [JsonProperty("gender")]
        /// <summary>
        /// Gets or sets the user's gender.
        /// </summary>
        public string gender
        {
            get; set;
        }

        [JsonProperty("locale")]
        /// <summary>
        /// Gets or sets the user's locale preference.
        /// </summary>
        public string locale
        {
            get; set;
        }
        [JsonProperty("hd")]
        /// <summary>
        /// Gets or sets the hosted domain for G Suite users.
        /// </summary>
        public string hd { get; set; }
    }
}
