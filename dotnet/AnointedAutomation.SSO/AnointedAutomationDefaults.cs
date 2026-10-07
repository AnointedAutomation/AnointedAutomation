// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
namespace AnointedAutomation.SSO
{
    /// <summary>
    /// Fixed facts about the Anointed Automation OpenID Connect provider and the defaults this package uses.
    /// </summary>
    public static class AnointedAutomationDefaults
    {
        /// <summary>The provider's issuer (with the trailing slash, exactly as it appears in <c>iss</c>).</summary>
        public const string Issuer = "https://api.anointedautomation.net/";

        /// <summary>The default authentication scheme name.</summary>
        public const string AuthenticationScheme = "Anointed";

        /// <summary>The default display name shown on login pages.</summary>
        public const string DisplayName = "Anointed Automation";

        /// <summary>The prefix every Anointed Automation client_id starts with.</summary>
        public const string ClientIdPrefix = "aa_";

        /// <summary>The <c>typ</c> header a back-channel Logout Token must carry.</summary>
        public const string LogoutTokenType = "logout+jwt";

        /// <summary>The member of the <c>events</c> claim that marks a back-channel Logout Token.</summary>
        public const string BackchannelLogoutEvent = "http://schemas.openid.net/event/backchannel-logout";

        /// <summary>The only signing algorithm the provider uses and this package accepts.</summary>
        public const string SigningAlgorithm = "RS256";

        /// <summary>The form field the provider posts the Logout Token in.</summary>
        public const string LogoutTokenFormField = "logout_token";

        /// <summary>The <c>openid</c> scope (always requested).</summary>
        public const string OpenIdScope = "openid";

        /// <summary>The <c>email</c> scope (requested by default).</summary>
        public const string EmailScope = "email";

        /// <summary>The <c>profile</c> scope (asking for it makes users without a username pick one).</summary>
        public const string ProfileScope = "profile";

        /// <summary>The <c>offline_access</c> scope (adds a refresh token).</summary>
        public const string OfflineAccessScope = "offline_access";

        /// <summary>The relative path of the machine token exchange.</summary>
        public const string MachineTokenPath = "api/auth/machine-token";

        /// <summary>The relative path of the session status check.</summary>
        public const string SessionStatusPath = "api/session-status";
    }
}
