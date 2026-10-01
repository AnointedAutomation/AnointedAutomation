// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Microsoft
{
    /// <summary>
    /// The real Microsoft (Azure AD / Entra) identity for a user who signed in with Microsoft. Mirrors
    /// the container-of-objects shape of <see cref="AnointedAutomation.Objects.Google.GoogleObjects"/>:
    /// a validated token-claim object (<see cref="MicrosoftTokenInfo"/>) plus a human profile object
    /// (<see cref="MicrosoftUserProfile"/>), so no data the provider gives is thrown away. This is the
    /// object stored at <c>User.SSO.Microsoft</c>; the stable link key is <see cref="MicrosoftTokenInfo.oid"/>,
    /// falling back to <see cref="MicrosoftTokenInfo.sub"/>.
    /// </summary>
    public class MicrosoftObjects
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicrosoftObjects"/> class.
        /// </summary>
        public MicrosoftObjects()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MicrosoftObjects"/> class with specified parts.
        /// </summary>
        /// <param name="tokenInfo">The validated id_token claim set.</param>
        /// <param name="userProfile">The human profile derived from the token.</param>
        public MicrosoftObjects(MicrosoftTokenInfo tokenInfo, MicrosoftUserProfile userProfile)
        {
            TokenInfo = tokenInfo ?? new MicrosoftTokenInfo();
            UserProfile = userProfile ?? new MicrosoftUserProfile();
        }

        /// <summary>Gets or sets the validated Microsoft id_token claim set.</summary>
        public MicrosoftTokenInfo TokenInfo { get; set; }

        /// <summary>Gets or sets the human profile (name / email / picture) derived from the token.</summary>
        public MicrosoftUserProfile UserProfile { get; set; }

        /// <summary>When this identity was first linked to the user.</summary>
        public System.DateTime? linkedAt { get; set; }
    }
}
