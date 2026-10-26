// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.LinkedIn
{
    /// <summary>
    /// What Sign In with LinkedIn using OpenID Connect gives us: the <c>GET https://api.linkedin.com/v2/userinfo</c>
    /// response. Stored at <c>User.SSO.LinkedIn</c>. Never holds an access token.
    /// </summary>
    public class LinkedInLogin
    {
        /// <summary>The OIDC userinfo response.</summary>
        public LinkedInUserInfo UserInfo { get; set; }
    }
}
