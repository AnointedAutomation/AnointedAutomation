// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Owin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnointedAutomation.SSO.Owin
{
    /// <summary>
    /// Handles the provider's back-channel logout POST: 400 for an invalid token (your handler is not called),
    /// 200 for a valid one (and for a replayed jti, without calling your handler again), 500 when your handler
    /// throws or the provider's keys cannot be read, so the provider retries.
    /// </summary>
    public class AnointedBackchannelLogoutHandler
    {
        private const string FormContentType = "application/x-www-form-urlencoded";
        private readonly AnointedLogoutTokenValidator _validator;
        private readonly IAnointedLogoutReplayStore _replayStore;
        private readonly ILogger _logger;

        /// <summary>Creates the handler.</summary>
        /// <param name="validator">The Logout Token validator.</param>
        /// <param name="replayStore">Where processed jti values are remembered.</param>
        /// <param name="logger">The logger.</param>
        public AnointedBackchannelLogoutHandler(AnointedLogoutTokenValidator validator, IAnointedLogoutReplayStore replayStore, ILogger logger)
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

        /// <summary>The validator, for callers that want to validate a token themselves.</summary>
        public AnointedLogoutTokenValidator Validator
        {
            get { return _validator; }
        }

        /// <summary>Processes one back-channel logout request.</summary>
        /// <param name="context">The OWIN context of the POST.</param>
        /// <param name="onLogout">Your handler: end every session for the logout's sub (or sid).</param>
        /// <returns>A task that completes when the response status is set.</returns>
        public async Task HandleAsync(IOwinContext context, Func<AnointedLogout, IOwinContext, Task> onLogout)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (onLogout == null)
            {
                throw new ArgumentNullException(nameof(onLogout));
            }
            context.Response.Headers.Set("Cache-Control", "no-store");

            string contentType = context.Request.ContentType;
            if (contentType == null || !contentType.StartsWith(FormContentType, StringComparison.OrdinalIgnoreCase))
            {
                _logger.WriteInformation("Anointed back-channel logout rejected: the request is not a form post.");
                context.Response.StatusCode = 400;
                return;
            }
            IFormCollection form = await context.Request.ReadFormAsync().ConfigureAwait(false);
            IList<string> values = form.GetValues(AnointedAutomationDefaults.LogoutTokenFormField);
            if (values == null || values.Count != 1 || string.IsNullOrWhiteSpace(values[0]))
            {
                _logger.WriteInformation("Anointed back-channel logout rejected: logout_token is missing or repeated.");
                context.Response.StatusCode = 400;
                return;
            }

            AnointedLogoutValidationResult result;
            try
            {
                result = await _validator.ValidateAsync(values[0], context.Request.CallCancelled).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.WriteError("Anointed back-channel logout could not be validated (provider keys unavailable); answering 500 so it is retried.", ex);
                context.Response.StatusCode = 500;
                return;
            }
            if (!result.IsValid)
            {
                _logger.WriteWarning("Anointed back-channel logout rejected: " + result.Error);
                context.Response.StatusCode = 400;
                return;
            }

            AnointedLogout logout = result.Logout;
            DateTimeOffset rememberUntil = logout.ExpiresAt + _validator.Options.ClockSkew;
            bool isNew = await _replayStore.TryAddAsync(logout.Jti, rememberUntil, context.Request.CallCancelled).ConfigureAwait(false);
            if (!isNew)
            {
                _logger.WriteInformation("Anointed back-channel logout " + logout.Jti + " was already processed.");
                context.Response.StatusCode = 200;
                return;
            }

            try
            {
                await onLogout(logout, context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _replayStore.RemoveAsync(logout.Jti, context.Request.CallCancelled).ConfigureAwait(false);
                _logger.WriteError("Anointed back-channel logout " + logout.Jti + " handler failed; answering 500 so it is retried.", ex);
                context.Response.StatusCode = 500;
                return;
            }
            context.Response.StatusCode = 200;
        }
    }

    /// <summary>
    /// OWIN middleware that answers POSTs to one path with <see cref="AnointedBackchannelLogoutHandler"/>; any
    /// other method on that path gets 405, and every other path passes through.
    /// </summary>
    public class AnointedBackchannelLogoutMiddleware : OwinMiddleware
    {
        private readonly PathString _path;
        private readonly AnointedBackchannelLogoutHandler _handler;
        private readonly Func<AnointedLogout, IOwinContext, Task> _onLogout;

        /// <summary>Creates the middleware.</summary>
        /// <param name="next">The next middleware.</param>
        /// <param name="path">The exact path, for example <c>/auth/anointed/backchannel-logout</c>.</param>
        /// <param name="handler">The logout handler.</param>
        /// <param name="onLogout">Your handler.</param>
        public AnointedBackchannelLogoutMiddleware(OwinMiddleware next, PathString path, AnointedBackchannelLogoutHandler handler, Func<AnointedLogout, IOwinContext, Task> onLogout) : base(next)
        {
            if (!path.HasValue)
            {
                throw new ArgumentException("A path is required.", nameof(path));
            }
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            if (onLogout == null)
            {
                throw new ArgumentNullException(nameof(onLogout));
            }
            _path = path;
            _handler = handler;
            _onLogout = onLogout;
        }

        /// <summary>Handles the logout path or passes the request on.</summary>
        /// <param name="context">The OWIN context.</param>
        /// <returns>A task that completes when the request is handled.</returns>
        public override Task Invoke(IOwinContext context)
        {
            if (!context.Request.Path.Equals(_path))
            {
                return Next.Invoke(context);
            }
            if (!string.Equals(context.Request.Method, "POST", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 405;
                context.Response.Headers.Set("Allow", "POST");
                return Task.CompletedTask;
            }
            return _handler.HandleAsync(context, _onLogout);
        }
    }

    /// <summary>Maps the back-channel logout endpoint.</summary>
    public static class AnointedBackchannelLogoutExtensions
    {
        /// <summary>
        /// Answers POSTs at <paramref name="path"/> with the provider's back-channel Logout Token. Register its full
        /// https URL with Anointed Automation as your back-channel logout URI. The endpoint allows anonymous callers
        /// (the token is the proof) and runs before MVC, so no antiforgery token is involved. Call
        /// <see cref="AnointedAutomationAppBuilderExtensions.UseAnointedAutomation"/> first.
        /// </summary>
        /// <param name="app">The OWIN app builder.</param>
        /// <param name="path">The path, for example <c>/auth/anointed/backchannel-logout</c>.</param>
        /// <param name="onLogout">End every session for the logout's sub (or sid) and drop its refresh tokens.</param>
        /// <returns>The same app builder.</returns>
        public static IAppBuilder MapAnointedBackchannelLogout(this IAppBuilder app, string path, Func<AnointedLogout, IOwinContext, Task> onLogout)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (string.IsNullOrWhiteSpace(path) || !path.StartsWith("/", StringComparison.Ordinal))
            {
                throw new ArgumentException("A path starting with / is required.", nameof(path));
            }
            if (onLogout == null)
            {
                throw new ArgumentNullException(nameof(onLogout));
            }
            object handler;
            if (!app.Properties.TryGetValue(AnointedAutomationAppBuilderExtensions.BackchannelLogoutHandlerKey, out handler) || !(handler is AnointedBackchannelLogoutHandler))
            {
                throw new InvalidOperationException("MapAnointedBackchannelLogout needs app.UseAnointedAutomation(...) to be called first.");
            }
            return app.Use(typeof(AnointedBackchannelLogoutMiddleware), new PathString(path), (AnointedBackchannelLogoutHandler)handler, onLogout);
        }
    }
}
