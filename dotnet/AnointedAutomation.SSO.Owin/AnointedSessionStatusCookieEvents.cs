// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using Microsoft.Owin.Security.Cookies;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AnointedAutomation.SSO.Owin
{
    /// <summary>
    /// A cookie authentication hook that ends a local session only when Anointed Automation says it has ended.
    /// </summary>
    public static class AnointedSessionStatusCookieEvents
    {
        /// <summary>
        /// For <see cref="CookieAuthenticationProvider.OnValidateIdentity"/>: checks the cookie identity's
        /// <c>sub</c> and rejects the identity (and signs the cookie out) only when the outcome is Ended. An
        /// identity without <c>sub</c> is left alone; Unknown keeps the session (fail open).
        /// </summary>
        /// <param name="context">The cookie validation context.</param>
        /// <param name="sessionStatus">Your single session status client.</param>
        /// <param name="grant">True when the session came from Sign in with Anointed Automation.</param>
        /// <returns>A task that completes when the check is done.</returns>
        public static async Task ValidateIdentityAsync(CookieValidateIdentityContext context, IAnointedSessionStatus sessionStatus, bool grant)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (sessionStatus == null)
            {
                throw new ArgumentNullException(nameof(sessionStatus));
            }
            if (context.Identity == null)
            {
                return;
            }
            Claim sub = context.Identity.FindFirst(AnointedClaims.Subject);
            if (sub == null || string.IsNullOrEmpty(sub.Value))
            {
                return;
            }
            AnointedSessionCheck check = await sessionStatus.CheckAsync(sub.Value, grant, context.Request.CallCancelled).ConfigureAwait(false);
            if (!check.ShouldEndSession)
            {
                return;
            }
            context.RejectIdentity();
            context.OwinContext.Authentication.SignOut(context.Options.AuthenticationType);
        }
    }
}
