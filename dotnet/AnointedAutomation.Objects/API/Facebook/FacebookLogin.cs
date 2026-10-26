// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Facebook
{
    /// <summary>
    /// What Facebook Login actually gives us: the Graph <c>/me</c> user and the <c>/debug_token</c> data
    /// that verified the token. Stored at <c>User.SSO.Facebook</c>.
    /// </summary>
    public class FacebookLogin
    {
        /// <summary>The Graph <c>/me</c> response.</summary>
        public FacebookUser User { get; set; }

        /// <summary>The <c>/debug_token</c> data, null if not captured.</summary>
        public FacebookDebugTokenData DebugToken { get; set; }
    }
}
