// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Discord
{
    /// <summary>
    /// What Discord sign-in gives us: the OAuth2 <c>/users/@me</c> user. Stored at <c>User.SSO.Discord</c>.
    /// </summary>
    public class DiscordLogin
    {
        /// <summary>The <c>/users/@me</c> response.</summary>
        public DiscordUser User { get; set; }
    }
}
