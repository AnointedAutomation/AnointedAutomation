// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Apple
{
    /// <summary>
    /// The real Sign in with Apple identity for a user who signed in with Apple. Mirrors the
    /// container-of-objects shape of <see cref="AnointedAutomation.Objects.Google.GoogleObjects"/>: a
    /// validated token-claim object (<see cref="AppleTokenInfo"/>) plus the human profile object
    /// (<see cref="AppleUserProfile"/>) carrying the first-consent name Apple sends only ONCE (never in
    /// the token). This is the object stored at <c>User.SSO.Apple</c>; the stable link key is
    /// <see cref="AppleTokenInfo.sub"/>.
    /// </summary>
    public class AppleObjects
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppleObjects"/> class.
        /// </summary>
        public AppleObjects()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppleObjects"/> class with specified parts.
        /// </summary>
        /// <param name="tokenInfo">The validated id_token claim set.</param>
        /// <param name="userProfile">The first-consent human profile (name / email).</param>
        public AppleObjects(AppleTokenInfo tokenInfo, AppleUserProfile userProfile)
        {
            TokenInfo = tokenInfo ?? new AppleTokenInfo();
            UserProfile = userProfile ?? new AppleUserProfile();
        }

        /// <summary>Gets or sets the validated Apple id_token claim set.</summary>
        public AppleTokenInfo TokenInfo { get; set; }

        /// <summary>Gets or sets the first-consent human profile (name only present on first authorization).</summary>
        public AppleUserProfile UserProfile { get; set; }

        /// <summary>When this identity was first linked to the user.</summary>
        public System.DateTime? linkedAt { get; set; }
    }
}
