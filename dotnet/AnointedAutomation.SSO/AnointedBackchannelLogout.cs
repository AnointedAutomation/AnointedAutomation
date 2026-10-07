// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;

namespace AnointedAutomation.SSO
{
    /// <summary>
    /// Handles the provider's back-channel logout POST: 400 for an invalid token (your handler is not called),
    /// 200 for a valid one (and for a replayed jti, without calling your handler again), 500 when your handler
    /// throws or the provider's keys cannot be read, so the provider retries.
    /// </summary>
    public class AnointedBackchannelLogoutHandler
    {
        private readonly AnointedLogoutTokenValidator _validator;
        private readonly IAnointedLogoutReplayStore _replayStore;
        private readonly ILogger<AnointedBackchannelLogoutHandler> _logger;

        /// <summary>Creates the handler.</summary>
        /// <param name="validator">The Logout Token validator.</param>
        /// <param name="replayStore">Where processed jti values are remembered.</param>
        /// <param name="logger">The logger.</param>
        public AnointedBackchannelLogoutHandler(AnointedLogoutTokenValidator validator, IAnointedLogoutReplayStore replayStore, ILogger<AnointedBackchannelLogoutHandler> logger)
        {
            if (validator == null)
            {
                throw new ArgumentNullException(nameof(validator));
            }
            if (replayStore == null)
            {
                throw new ArgumentNullException(nameof(replayStore));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            _validator = validator;
            _replayStore = replayStore;
            _logger = logger;
        }

        /// <summary>Processes one back-channel logout request.</summary>
        /// <param name="context">The HTTP context of the POST.</param>
        /// <param name="onLogout">Your handler: end every session for the logout's sub (or sid).</param>
        /// <returns>A task that completes when the response status is set.</returns>
        public async Task HandleAsync(HttpContext context, Func<AnointedLogout, HttpContext, Task> onLogout)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (onLogout == null)
            {
                throw new ArgumentNullException(nameof(onLogout));
            }
            context.Response.Headers.CacheControl = "no-store";

            if (!context.Request.HasFormContentType)
            {
                _logger.LogWarning("Anointed back-channel logout rejected: the request is not a form post.");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }
            IFormCollection form = await context.Request.ReadFormAsync(context.RequestAborted).ConfigureAwait(false);
            StringValues values = form[AnointedAutomationDefaults.LogoutTokenFormField];
            if (values.Count != 1 || string.IsNullOrWhiteSpace(values[0]))
            {
                _logger.LogWarning("Anointed back-channel logout rejected: logout_token is missing or repeated.");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            AnointedLogoutValidationResult result;
            try
            {
                result = await _validator.ValidateAsync(values[0], context.RequestAborted).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Anointed back-channel logout could not be validated (provider keys unavailable); answering 500 so it is retried.");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                return;
            }
            if (!result.IsValid)
            {
                _logger.LogWarning("Anointed back-channel logout rejected: {Reason}", result.Error);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            AnointedLogout logout = result.Logout;
            DateTimeOffset rememberUntil = logout.ExpiresAt + _validator.Options.ClockSkew;
            bool isNew = await _replayStore.TryAddAsync(logout.Jti, rememberUntil, context.RequestAborted).ConfigureAwait(false);
            if (!isNew)
            {
                _logger.LogInformation("Anointed back-channel logout {Jti} was already processed.", logout.Jti);
                context.Response.StatusCode = StatusCodes.Status200OK;
                return;
            }

            try
            {
                await onLogout(logout, context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _replayStore.RemoveAsync(logout.Jti, context.RequestAborted).ConfigureAwait(false);
                _logger.LogError(ex, "Anointed back-channel logout {Jti} handler failed; answering 500 so it is retried.", logout.Jti);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                return;
            }
            context.Response.StatusCode = StatusCodes.Status200OK;
        }
    }

    /// <summary>Maps the back-channel logout endpoint.</summary>
    public static class AnointedBackchannelLogoutExtensions
    {
        /// <summary>
        /// Maps a POST endpoint at <paramref name="pattern"/> for the provider's back-channel Logout Token. Register
        /// its full https URL with Anointed Automation as your back-channel logout URI. The endpoint allows
        /// anonymous callers (the token is the proof). Needs <c>AddAnointedAutomation</c>.
        /// </summary>
        /// <param name="endpoints">The route builder.</param>
        /// <param name="pattern">The route, for example <c>/auth/anointed/backchannel-logout</c>.</param>
        /// <param name="onLogout">End every session for the logout's sub (or sid) and drop its refresh tokens.</param>
        /// <returns>The endpoint convention builder.</returns>
        public static IEndpointConventionBuilder MapAnointedBackchannelLogout(this IEndpointRouteBuilder endpoints, string pattern, Func<AnointedLogout, HttpContext, Task> onLogout)
        {
            if (endpoints == null)
            {
                throw new ArgumentNullException(nameof(endpoints));
            }
            if (string.IsNullOrWhiteSpace(pattern))
            {
                throw new ArgumentException("A route pattern is required.", nameof(pattern));
            }
            if (onLogout == null)
            {
                throw new ArgumentNullException(nameof(onLogout));
            }
            AnointedBackchannelLogoutHandler handler = endpoints.ServiceProvider.GetService<AnointedBackchannelLogoutHandler>();
            if (handler == null)
            {
                throw new InvalidOperationException("MapAnointedBackchannelLogout needs AddAnointedAutomation to be called on the authentication builder first.");
            }
            RequestDelegate requestDelegate = context => handler.HandleAsync(context, onLogout);
            return endpoints.MapPost(pattern, requestDelegate).AllowAnonymous().DisableAntiforgery();
        }
    }
}
