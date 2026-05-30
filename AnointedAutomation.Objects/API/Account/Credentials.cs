// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Account
{
    /// <summary>
    /// Represents user authentication credentials.
    /// </summary>
    public class Credentials
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Credentials"/> class.
        /// </summary>
        public Credentials()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Credentials"/> class with specified properties.
        /// </summary>
        /// <param name="email">User's email address.</param>
        /// <param name="password">User's password.</param>
        /// <param name="token">Authentication token.</param>
        /// <param name="googleToken">Google authentication token.</param>
        public Credentials(string email, string password, string token, string googleToken)
        {
            this.Email = email ?? throw new System.ArgumentNullException(nameof(email));
            this.Password = password ?? throw new System.ArgumentNullException(nameof(password));
            this.Token = token;
            this.GoogleToken = token;
        }

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Gets or sets the Google authentication token.
        /// </summary>
        public string GoogleToken { get; set; }
        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Gets or sets the authentication token.
        /// </summary>
        public string Token { get; set; }
    }
}
