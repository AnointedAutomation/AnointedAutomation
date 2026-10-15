// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Amazon
{
    /// <summary>
    /// What Login with Amazon gives us: the <c>GET https://api.amazon.com/user/profile</c> customer profile. Stored at
    /// <c>User.SSO.Amazon</c>. Never holds an access or refresh token.
    /// </summary>
    public class AmazonLogin
    {
        /// <summary>The customer profile response.</summary>
        public AmazonProfile Profile { get; set; }
    }
}
