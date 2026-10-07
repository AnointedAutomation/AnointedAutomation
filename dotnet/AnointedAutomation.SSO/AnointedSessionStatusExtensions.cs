// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AnointedAutomation.SSO
{
    /// <summary>Registers <see cref="IAnointedSessionStatus"/>.</summary>
    public static class AnointedSessionStatusExtensions
    {
        /// <summary>
        /// Registers <see cref="IAnointedSessionStatus"/> as a typed HttpClient. The options are validated
        /// immediately, so a missing ApiKey or AppName throws while the app is starting.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configure">Sets ApiKey, AppName and any optional settings.</param>
        /// <returns>The HttpClient builder, for adding handlers or a proxy.</returns>
        public static IHttpClientBuilder AddAnointedSessionStatus(this IServiceCollection services, Action<AnointedSessionStatusOptions> configure)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            AnointedSessionStatusOptions options = new AnointedSessionStatusOptions();
            configure(options);
            options.Validate();

            services.AddSingleton(options);
            services.TryAddSingleton(TimeProvider.System);
            services.TryAddSingleton<AnointedSessionStatusCache>();
            services.AddLogging();
            return services.AddHttpClient<IAnointedSessionStatus, AnointedSessionStatusClient>(client =>
            {
                client.BaseAddress = options.BaseAddress;
                client.Timeout = options.Timeout + options.Timeout;
            });
        }
    }

    /// <summary>
    /// A cookie authentication hook that ends a local session only when Anointed Automation says it has ended.
    /// </summary>
    public static class AnointedSessionStatusCookieEvents
    {
        /// <summary>
        /// For <see cref="CookieAuthenticationEvents.OnValidatePrincipal"/>: checks the cookie principal's
        /// <c>sub</c> and rejects the principal (and signs the cookie out) only when the outcome is Ended.
        /// A principal without <c>sub</c> is left alone; Unknown keeps the session (fail open). Needs
        /// <c>AddAnointedSessionStatus</c>.
        /// </summary>
        /// <param name="context">The cookie validation context.</param>
        /// <param name="grant">True when the session came from Sign in with Anointed Automation.</param>
        /// <returns>A task that completes when the check is done.</returns>
        public static async Task ValidatePrincipalAsync(CookieValidatePrincipalContext context, bool grant)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (context.Principal == null)
            {
                return;
            }
            Claim sub = context.Principal.FindFirst(AnointedClaims.Subject);
            if (sub == null || string.IsNullOrEmpty(sub.Value))
            {
                return;
            }
            IAnointedSessionStatus status = context.HttpContext.RequestServices.GetRequiredService<IAnointedSessionStatus>();
            AnointedSessionCheck check = await status.CheckAsync(sub.Value, grant, context.HttpContext.RequestAborted).ConfigureAwait(false);
            if (!check.ShouldEndSession)
            {
                return;
            }
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync(context.Scheme.Name).ConfigureAwait(false);
        }
    }
}
