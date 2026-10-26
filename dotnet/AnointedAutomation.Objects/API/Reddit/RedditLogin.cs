// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Reddit
{
    /// <summary>
    /// What Reddit OAuth2 sign-in gives us: the <c>GET /api/v1/me</c> identity (scope <c>identity</c>). Stored at
    /// <c>User.SSO.Reddit</c>. Reddit never returns an email address. Never holds an access or refresh token.
    /// </summary>
    public class RedditLogin
    {
        /// <summary>The <c>/api/v1/me</c> response.</summary>
        public RedditUser User { get; set; }
    }
}
