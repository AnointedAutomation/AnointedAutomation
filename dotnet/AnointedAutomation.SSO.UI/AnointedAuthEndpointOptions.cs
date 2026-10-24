// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using Microsoft.AspNetCore.Authentication.Cookies;
using System;

namespace AnointedAutomation.SSO.UI
{
    /// <summary>
    /// Settings for <see cref="AnointedAuthEndpoints.MapAnointedAutomationAuth(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder, string, Action{AnointedAuthEndpointOptions})"/>.
    /// </summary>
    public class AnointedAuthEndpointOptions
    {
        /// <summary>The header a script sends to sign out without a form antiforgery token (BFF CSRF guard).</summary>
        public const string CsrfHeaderName = "X-Anointed-CSRF";

        /// <summary>The scheme that runs the sign-in challenge. Default <c>Anointed</c>.</summary>
        public string ChallengeScheme { get; set; } = AnointedAutomationDefaults.AuthenticationScheme;

        /// <summary>The local cookie scheme the session lives in. Default <c>Cookies</c>.</summary>
        public string CookieScheme { get; set; } = CookieAuthenticationDefaults.AuthenticationScheme;

        /// <summary>The query (start) or form/query (sign-out) parameter carrying the return target. Default <c>returnTo</c>.</summary>
        public string ReturnToParam { get; set; } = AnointedButtonLook.DefaultReturnToParam;

        /// <summary>Where to land when no safe return target was given. Must be a same-site path. Default <c>/</c>.</summary>
        public string DefaultReturnPath { get; set; } = "/";

        /// <summary>Throws <see cref="InvalidOperationException"/> naming the first invalid setting.</summary>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(ChallengeScheme))
            {
                throw new InvalidOperationException("AnointedAuthEndpointOptions.ChallengeScheme is required.");
            }
            if (string.IsNullOrWhiteSpace(CookieScheme))
            {
                throw new InvalidOperationException("AnointedAuthEndpointOptions.CookieScheme is required.");
            }
            if (string.IsNullOrWhiteSpace(ReturnToParam))
            {
                throw new InvalidOperationException("AnointedAuthEndpointOptions.ReturnToParam is required.");
            }
            if (AnointedReturnTo.SafeRelativePath(DefaultReturnPath) == null)
            {
                throw new InvalidOperationException("AnointedAuthEndpointOptions.DefaultReturnPath must be a same-site path such as /.");
            }
        }
    }
}
