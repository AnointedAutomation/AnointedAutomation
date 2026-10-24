// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace AnointedAutomation.SSO.UI
{
    /// <summary>
    /// Minimal API routes for a backend-for-frontend sign-in: the browser only ever holds your session cookie.
    /// Under the prefix you choose:
    /// <list type="bullet">
    /// <item><c>GET {prefix}/start?returnTo=/path</c> starts the Anointed Automation sign-in.</item>
    /// <item><c>POST {prefix}/sign-out</c> ends the local session (form with an antiforgery token, or a script sending <c>X-Anointed-CSRF: 1</c>).</item>
    /// <item><c>GET {prefix}/user</c> returns <see cref="AnointedUserInfo"/> JSON for single page apps (no tokens).</item>
    /// </list>
    /// </summary>
    public static class AnointedAuthEndpoints
    {
        /// <summary>The start route, relative to the prefix.</summary>
        public const string StartRoute = "start";

        /// <summary>The sign-out route, relative to the prefix.</summary>
        public const string SignOutRoute = "sign-out";

        /// <summary>The user-info route, relative to the prefix.</summary>
        public const string UserRoute = "user";

        /// <summary>Maps the start, sign-out and user-info routes with the default settings.</summary>
        /// <param name="endpoints">The route builder.</param>
        /// <param name="prefix">The route prefix, for example <c>/auth/anointed</c>. Required.</param>
        /// <returns>The route group, for adding conventions such as rate limiting.</returns>
        public static RouteGroupBuilder MapAnointedAutomationAuth(this IEndpointRouteBuilder endpoints, string prefix)
        {
            return MapAnointedAutomationAuth(endpoints, prefix, null);
        }

        /// <summary>Maps the start, sign-out and user-info routes.</summary>
        /// <param name="endpoints">The route builder.</param>
        /// <param name="prefix">The route prefix, for example <c>/auth/anointed</c>. Required.</param>
        /// <param name="configure">Optional settings changes.</param>
        /// <returns>The route group, for adding conventions such as rate limiting.</returns>
        /// <exception cref="InvalidOperationException">A setting is invalid (fail fast at startup).</exception>
        public static RouteGroupBuilder MapAnointedAutomationAuth(this IEndpointRouteBuilder endpoints, string prefix, Action<AnointedAuthEndpointOptions> configure)
        {
            if (endpoints == null)
            {
                throw new ArgumentNullException(nameof(endpoints));
            }
            if (string.IsNullOrWhiteSpace(prefix) || prefix[0] != '/')
            {
                throw new ArgumentException("A route prefix starting with / is required, for example /auth/anointed.", nameof(prefix));
            }
            AnointedAuthEndpointOptions options = new AnointedAuthEndpointOptions();
            if (configure != null)
            {
                configure(options);
            }
            options.Validate();

            RouteGroupBuilder group = endpoints.MapGroup(prefix);
            group.MapGet(StartRoute, (HttpContext context) => StartAsync(context, options)).ExcludeFromDescription();
            group.MapPost(SignOutRoute, (HttpContext context) => SignOutAsync(context, options)).ExcludeFromDescription();
            group.MapGet(UserRoute, (HttpContext context) => UserAsync(context, options)).ExcludeFromDescription();
            return group;
        }

        /// <summary>Challenges the Anointed scheme, landing on a safe return path afterwards.</summary>
        /// <param name="context">The request.</param>
        /// <param name="options">Validated settings.</param>
        /// <returns>The challenge.</returns>
        internal static Task StartAsync(HttpContext context, AnointedAuthEndpointOptions options)
        {
            string requested = context.Request.Query[options.ReturnToParam];
            string target = ResolveReturn(requested, options);
            AuthenticationProperties properties = new AuthenticationProperties { RedirectUri = target };
            return context.ChallengeAsync(options.ChallengeScheme, properties);
        }

        /// <summary>Ends the local session after a CSRF check.</summary>
        /// <param name="context">The request.</param>
        /// <param name="options">Validated settings.</param>
        /// <returns>The response.</returns>
        internal static async Task SignOutAsync(HttpContext context, AnointedAuthEndpointOptions options)
        {
            bool fromScript = context.Request.Headers.ContainsKey(AnointedAuthEndpointOptions.CsrfHeaderName);
            string requested = context.Request.Query[options.ReturnToParam];
            if (!fromScript)
            {
                IAntiforgery antiforgery = context.RequestServices.GetService<IAntiforgery>();
                if (antiforgery == null || !context.Request.HasFormContentType)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }
                if (!await antiforgery.IsRequestValidAsync(context).ConfigureAwait(false))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }
                IFormCollection form = await context.Request.ReadFormAsync().ConfigureAwait(false);
                string posted = form[options.ReturnToParam];
                if (posted != null)
                {
                    requested = posted;
                }
            }

            await context.SignOutAsync(options.CookieScheme).ConfigureAwait(false);
            context.Response.Headers.CacheControl = "no-store";
            if (fromScript)
            {
                context.Response.StatusCode = StatusCodes.Status204NoContent;
                return;
            }
            context.Response.Redirect(ResolveReturn(requested, options));
        }

        /// <summary>Writes the signed-in user (or the signed-out answer) as JSON. Never a token.</summary>
        /// <param name="context">The request.</param>
        /// <param name="options">Validated settings.</param>
        /// <returns>The response.</returns>
        internal static async Task UserAsync(HttpContext context, AnointedAuthEndpointOptions options)
        {
            AuthenticateResult result = await context.AuthenticateAsync(options.CookieScheme).ConfigureAwait(false);
            AnointedUserInfo info = result.Succeeded && result.Principal.FindFirst(AnointedClaims.Subject) != null
                ? AnointedUserInfo.FromPrincipal(result.Principal)
                : AnointedUserInfo.SignedOut();
            context.Response.Headers.CacheControl = "no-store";
            await context.Response.WriteAsJsonAsync(info).ConfigureAwait(false);
        }

        /// <summary>The safe return target, or the configured default path.</summary>
        /// <param name="requested">The requested target, possibly null.</param>
        /// <param name="options">Validated settings.</param>
        /// <returns>A same-site path.</returns>
        internal static string ResolveReturn(string requested, AnointedAuthEndpointOptions options)
        {
            string safe = AnointedReturnTo.SafeRelativePath(requested);
            if (safe == null)
            {
                return options.DefaultReturnPath;
            }
            return safe;
        }
    }
}
