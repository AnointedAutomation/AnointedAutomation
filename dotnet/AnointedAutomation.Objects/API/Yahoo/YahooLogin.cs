// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Yahoo
{
    /// <summary>
    /// What Yahoo OpenID Connect sign-in gives us: the id_token / userinfo claims. Stored at <c>User.SSO.Yahoo</c>.
    /// Never holds an access or refresh token.
    /// </summary>
    public class YahooLogin
    {
        /// <summary>The verified id_token claims merged with the <c>GET /openid/v1/userinfo</c> response.</summary>
        public YahooUserInfo UserInfo { get; set; }
    }
}
