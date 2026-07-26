// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-02-03 13:40:00
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Google
{
    /// <summary>
    /// Represents Google OAuth token information and user details.
    /// </summary>
    public class GoogleTokenInfo
    {
        [JsonProperty("access_token")]
        /// <summary>
        /// Gets or sets the access token for API requests.
        /// </summary>
        public string accessToken { get; set; }

        [JsonProperty("token_type")]
        /// <summary>
        /// Gets or sets the type of token (usually "Bearer").
        /// </summary>
        public string tokenType { get; set; }

        [JsonProperty("id_token")]
        /// <summary>
        /// Gets or sets the ID token containing user information.
        /// </summary>
        public string idToken { get; set; }

        [JsonProperty("azp")]
        /// <summary>
        /// Gets or sets the authorized party that requested the token.
        /// </summary>
        public string azp { get; set; }

        [JsonProperty("aud")]
        /// <summary>
        /// Gets or sets the audience the token is intended for.
        /// </summary>
        public string aud { get; set; }

        [JsonProperty("sub")]
        /// <summary>
        /// Gets or sets the subject identifier for the user.
        /// </summary>
        public string sub { get; set; }

        [JsonProperty("scope")]
        /// <summary>
        /// Gets or sets the scope of access granted by the token.
        /// </summary>
        public string scope { get; set; }

        [JsonProperty("exp")]
        /// <summary>
        /// Gets or sets the token expiration timestamp.
        /// </summary>
        public long exp { get; set; }

        [JsonProperty("expires_in")]
        /// <summary>
        /// Gets or sets the number of seconds until token expiration.
        /// </summary>
        public long expires_in { get; set; }

        [JsonProperty("email")]
        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        public string email { get; set; }

        [JsonProperty("email_verified")]
        /// <summary>
        /// Gets or sets whether the user's email has been verified.
        /// </summary>
        public string emailVerified { get; set; }

        [JsonProperty("access_type")]
        /// <summary>
        /// Gets or sets the type of access granted (offline/online).
        /// </summary>
        public string accessType { get; set; }

        // Parameterless constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleTokenInfo"/> class.
        /// </summary>
        public GoogleTokenInfo()
        {
        }

        // Constructor with all properties
        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleTokenInfo"/> class with specified properties.
        /// </summary>
        /// <param name="azp">The authorized party that requested the token.</param>
        /// <param name="aud">The audience the token is intended for.</param>
        /// <param name="sub">The subject identifier for the user.</param>
        /// <param name="scope">The scope of access granted.</param>
        /// <param name="exp">The token expiration timestamp.</param>
        /// <param name="expires_in">The number of seconds until token expiration.</param>
        /// <param name="email">The user's email address.</param>
        /// <param name="emailVerified">Whether the email has been verified.</param>
        /// <param name="accessType">The type of access granted.</param>
        public GoogleTokenInfo(string azp, string aud, string sub, string scope, long exp, long expires_in, string email, string emailVerified, string accessType)
        {
            this.azp = azp;
            this.aud = aud;
            this.sub = sub;
            this.scope = scope;
            this.exp = exp;
            this.expires_in = expires_in;
            this.email = email;
            this.emailVerified = emailVerified;
            this.accessType = accessType;
        }
    }
}
