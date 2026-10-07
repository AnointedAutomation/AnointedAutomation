// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnointedAutomation.SSO
{
    /// <summary>What Anointed Automation said about a session.</summary>
    public enum AnointedSessionOutcome
    {
        /// <summary>No answer (network error, timeout, non-200, bad JSON). Keep the session: the check fails open.</summary>
        Unknown = 0,

        /// <summary>The session may continue.</summary>
        Active = 1,

        /// <summary>Anointed Automation said the session has ended. End it.</summary>
        Ended = 2,
    }

    /// <summary>The result of a session status check.</summary>
    public class AnointedSessionCheck
    {
        /// <summary>The outcome.</summary>
        public AnointedSessionOutcome Outcome { get; private set; }

        /// <summary>
        /// The provider's reason when it answered: <c>active</c>, <c>account_gone</c>, <c>account_banned</c>,
        /// <c>signin_off</c> or <c>app_disconnected</c>. Null when the outcome is Unknown or no reason was sent.
        /// </summary>
        public string Reason { get; private set; }

        /// <summary>Why the outcome is Unknown (for your logs); null otherwise.</summary>
        public string Error { get; private set; }

        /// <summary>True only when Anointed Automation answered that the session has ended.</summary>
        public bool ShouldEndSession
        {
            get { return Outcome == AnointedSessionOutcome.Ended; }
        }

        /// <summary>An active answer.</summary>
        /// <param name="reason">The provider's reason, if any.</param>
        /// <returns>An Active check.</returns>
        public static AnointedSessionCheck Active(string reason)
        {
            return new AnointedSessionCheck { Outcome = AnointedSessionOutcome.Active, Reason = reason };
        }

        /// <summary>An ended answer.</summary>
        /// <param name="reason">The provider's reason, if any.</param>
        /// <returns>An Ended check.</returns>
        public static AnointedSessionCheck Ended(string reason)
        {
            return new AnointedSessionCheck { Outcome = AnointedSessionOutcome.Ended, Reason = reason };
        }

        /// <summary>No answer; the caller keeps the session.</summary>
        /// <param name="error">Why there was no answer.</param>
        /// <returns>An Unknown check.</returns>
        public static AnointedSessionCheck Unknown(string error)
        {
            if (string.IsNullOrWhiteSpace(error))
            {
                throw new ArgumentException("An error description is required.", nameof(error));
            }
            return new AnointedSessionCheck { Outcome = AnointedSessionOutcome.Unknown, Error = error };
        }
    }

    /// <summary>
    /// Asks Anointed Automation whether a user's session may continue. FAIL-OPEN: only an explicit
    /// "active: false" answer ends a session; every failure is <see cref="AnointedSessionOutcome.Unknown"/>.
    /// </summary>
    public interface IAnointedSessionStatus
    {
        /// <summary>Checks one user's session.</summary>
        /// <param name="userId">The Anointed Automation user id (the <c>sub</c> claim).</param>
        /// <param name="grant">True for a session that came from Sign in with Anointed Automation (it also ends when the user disconnected your app).</param>
        /// <param name="cancellationToken">Cancels the check.</param>
        /// <returns>The check; never throws for provider or network failures.</returns>
        Task<AnointedSessionCheck> CheckAsync(string userId, bool grant, CancellationToken cancellationToken);
    }

    /// <summary>Settings for <see cref="IAnointedSessionStatus"/>. <see cref="ApiKey"/> and <see cref="AppName"/> are required.</summary>
    public class AnointedSessionStatusOptions
    {
        /// <summary>Your partner API key (it needs the <c>sessions:status</c> scope). Required. Server only.</summary>
        public string ApiKey { get; set; }

        /// <summary>Your app's name, sent with the machine token exchange. Required.</summary>
        public string AppName { get; set; }

        /// <summary>The API base address. Leave it unless told otherwise. Must be https and end with a slash.</summary>
        public Uri BaseAddress { get; set; } = new Uri(AnointedAutomationDefaults.Issuer);

        /// <summary>Timeout per HTTP request. Default 3 seconds.</summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(3);

        /// <summary>How long an Active or Ended answer is reused per (userId, grant). Default 5 minutes.</summary>
        public TimeSpan AnswerCacheDuration { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>How long before its expiry the machine token is re-exchanged. Default 60 seconds.</summary>
        public TimeSpan TokenRefreshMargin { get; set; } = TimeSpan.FromSeconds(60);

        /// <summary>Throws <see cref="InvalidOperationException"/> naming the first missing or invalid setting.</summary>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(ApiKey))
            {
                throw new InvalidOperationException("AnointedSessionStatusOptions.ApiKey is required. Read it from your server's secret store.");
            }
            if (string.IsNullOrWhiteSpace(AppName))
            {
                throw new InvalidOperationException("AnointedSessionStatusOptions.AppName is required.");
            }
            if (BaseAddress == null || !BaseAddress.IsAbsoluteUri || !string.Equals(BaseAddress.Scheme, Uri.UriSchemeHttps, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("AnointedSessionStatusOptions.BaseAddress must be an absolute https URL.");
            }
            if (!BaseAddress.AbsolutePath.EndsWith("/", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("AnointedSessionStatusOptions.BaseAddress must end with a slash.");
            }
            if (Timeout <= TimeSpan.Zero)
            {
                throw new InvalidOperationException("AnointedSessionStatusOptions.Timeout must be positive.");
            }
            if (AnswerCacheDuration < TimeSpan.Zero)
            {
                throw new InvalidOperationException("AnointedSessionStatusOptions.AnswerCacheDuration must not be negative.");
            }
            if (TokenRefreshMargin < TimeSpan.Zero)
            {
                throw new InvalidOperationException("AnointedSessionStatusOptions.TokenRefreshMargin must not be negative.");
            }
        }
    }
}
