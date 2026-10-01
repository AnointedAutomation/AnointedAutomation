// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Facebook
{
    /// <summary>
    /// The real Facebook Login identity for a user who signed in with Facebook. Mirrors the
    /// container-of-objects shape of <see cref="AnointedAutomation.Objects.Google.GoogleObjects"/>: a
    /// token-debug object (<see cref="FacebookTokenInfo"/>, from the Graph <c>debug_token</c> response)
    /// plus the human profile object (<see cref="FacebookUserProfile"/>, from Graph <c>/me</c>). This is
    /// the object stored at <c>User.SSO.Facebook</c>; the stable link key is the app-scoped
    /// <see cref="FacebookUserProfile.id"/> / <see cref="FacebookTokenInfo.appScopedId"/>.
    /// </summary>
    public class FacebookObjects
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FacebookObjects"/> class.
        /// </summary>
        public FacebookObjects()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FacebookObjects"/> class with specified parts.
        /// </summary>
        /// <param name="tokenInfo">The Graph debug_token metadata.</param>
        /// <param name="userProfile">The Graph /me profile.</param>
        public FacebookObjects(FacebookTokenInfo tokenInfo, FacebookUserProfile userProfile)
        {
            TokenInfo = tokenInfo ?? new FacebookTokenInfo();
            UserProfile = userProfile ?? new FacebookUserProfile();
        }

        /// <summary>Gets or sets the Graph <c>debug_token</c> metadata for the access token.</summary>
        public FacebookTokenInfo TokenInfo { get; set; }

        /// <summary>Gets or sets the human profile returned by Graph <c>/me</c>.</summary>
        public FacebookUserProfile UserProfile { get; set; }

        /// <summary>When this identity was first linked to the user.</summary>
        public System.DateTime? linkedAt { get; set; }
    }
}
