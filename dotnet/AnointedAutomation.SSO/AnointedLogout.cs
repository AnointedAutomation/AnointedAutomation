// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using System;

namespace AnointedAutomation.SSO
{
    /// <summary>
    /// A validated back-channel logout: end every session for <see cref="Sub"/> (or <see cref="Sid"/>) and drop
    /// any refresh token you hold for that user.
    /// </summary>
    public class AnointedLogout
    {
        /// <summary>The Anointed Automation user id, or null when the token named only a sid.</summary>
        public string Sub { get; set; }

        /// <summary>The grant id, or null when the token named only a sub.</summary>
        public string Sid { get; set; }

        /// <summary>The token id. The same logout retried by the provider carries the same jti.</summary>
        public string Jti { get; set; }

        /// <summary>When the Logout Token expires.</summary>
        public DateTimeOffset ExpiresAt { get; set; }
    }

    /// <summary>
    /// What a Logout Token must satisfy. Built for you by <c>AddAnointedAutomation</c>; construct it yourself only
    /// when using <see cref="AnointedLogoutTokenValidator"/> directly.
    /// </summary>
    public class AnointedLogoutValidationOptions
    {
        /// <summary>The exact expected <c>iss</c>.</summary>
        public string Issuer { get; set; }

        /// <summary>Your client_id; it must be in <c>aud</c>.</summary>
        public string ClientId { get; set; }

        /// <summary>How old a token may be, measured from <c>iat</c>. Default 5 minutes.</summary>
        public TimeSpan MaxTokenAge { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>Allowed clock skew for <c>iat</c> and <c>exp</c>. Default 60 seconds.</summary>
        public TimeSpan ClockSkew { get; set; } = TimeSpan.FromSeconds(60);

        /// <summary>Throws <see cref="InvalidOperationException"/> when a setting is missing or invalid.</summary>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Issuer))
            {
                throw new InvalidOperationException("AnointedLogoutValidationOptions.Issuer is required.");
            }
            if (string.IsNullOrWhiteSpace(ClientId))
            {
                throw new InvalidOperationException("AnointedLogoutValidationOptions.ClientId is required.");
            }
            if (MaxTokenAge <= TimeSpan.Zero)
            {
                throw new InvalidOperationException("AnointedLogoutValidationOptions.MaxTokenAge must be positive.");
            }
            if (ClockSkew < TimeSpan.Zero)
            {
                throw new InvalidOperationException("AnointedLogoutValidationOptions.ClockSkew must not be negative.");
            }
        }
    }

    /// <summary>The outcome of validating a Logout Token.</summary>
    public class AnointedLogoutValidationResult
    {
        /// <summary>Whether the token passed every rule.</summary>
        public bool IsValid { get; private set; }

        /// <summary>The validated logout when <see cref="IsValid"/>; otherwise null.</summary>
        public AnointedLogout Logout { get; private set; }

        /// <summary>The first rule that failed when not valid (for your logs, never for the caller); otherwise null.</summary>
        public string Error { get; private set; }

        /// <summary>A successful result.</summary>
        /// <param name="logout">The validated logout.</param>
        /// <returns>A valid result.</returns>
        public static AnointedLogoutValidationResult Success(AnointedLogout logout)
        {
            if (logout == null)
            {
                throw new ArgumentNullException(nameof(logout));
            }
            return new AnointedLogoutValidationResult { IsValid = true, Logout = logout };
        }

        /// <summary>A failed result.</summary>
        /// <param name="error">The rule that failed.</param>
        /// <returns>An invalid result.</returns>
        public static AnointedLogoutValidationResult Failure(string error)
        {
            if (string.IsNullOrWhiteSpace(error))
            {
                throw new ArgumentException("An error description is required.", nameof(error));
            }
            return new AnointedLogoutValidationResult { IsValid = false, Error = error };
        }
    }
}
