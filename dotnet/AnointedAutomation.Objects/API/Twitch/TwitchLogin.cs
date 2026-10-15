// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Twitch
{
    /// <summary>
    /// What Twitch sign-in gives us: the Helix <c>GET /helix/users</c> user and the OIDC id_token claims. Stored at
    /// <c>User.SSO.Twitch</c>. Never holds an access or refresh token.
    /// </summary>
    public class TwitchLogin
    {
        /// <summary>The token owner's entry of the Get Users response.</summary>
        public TwitchUser User { get; set; }

        /// <summary>The verified id_token (or userinfo) claims.</summary>
        public TwitchIdTokenClaims IdToken { get; set; }
    }
}
