// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

using System.Collections.Generic;

namespace AnointedAutomation.Objects.GitHub
{
    /// <summary>
    /// What GitHub sign-in gives us: the <c>GET /user</c> user plus the <c>GET /user/emails</c> list. Stored at
    /// <c>User.SSO.GitHub</c>. Never holds an access token.
    /// </summary>
    public class GitHubLogin
    {
        /// <summary>The <c>GET /user</c> response.</summary>
        public GitHubUser User { get; set; }

        /// <summary>
        /// The address from <c>GET /user/emails</c> that is BOTH <c>primary</c> and <c>verified</c>, null when there is none.
        /// </summary>
        public string VerifiedPrimaryEmail { get; set; }

        /// <summary>The <c>GET /user/emails</c> response (needs the <c>user:email</c> scope).</summary>
        public List<GitHubEmail> Emails { get; set; }
    }
}
