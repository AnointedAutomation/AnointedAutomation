// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using System;
using System.Security.Claims;

namespace AnointedAutomation.SSO
{
    /// <summary>
    /// Claim names Anointed Automation issues and helpers to read them. The handler keeps the raw OIDC claim
    /// names (inbound claim mapping is off), so <c>sub</c> stays <c>sub</c>. Key your users on <c>sub</c>,
    /// never on email or username.
    /// </summary>
    public static class AnointedClaims
    {
        /// <summary>Stable Anointed Automation user id.</summary>
        public const string Subject = "sub";

        /// <summary>The grant id; back-channel Logout Tokens carry the same value.</summary>
        public const string SessionId = "sid";

        /// <summary>The verified email (needs the email scope).</summary>
        public const string Email = "email";

        /// <summary>Whether the email is verified (always true when email is present).</summary>
        public const string EmailVerified = "email_verified";

        /// <summary>Display name (needs the profile scope).</summary>
        public const string Name = "name";

        /// <summary>The user's unique handle (needs the profile scope).</summary>
        public const string PreferredUsername = "preferred_username";

        /// <summary>Given name (needs the profile scope).</summary>
        public const string GivenName = "given_name";

        /// <summary>Family name (needs the profile scope).</summary>
        public const string FamilyName = "family_name";

        /// <summary>Avatar URL (needs the profile scope).</summary>
        public const string Picture = "picture";

        /// <summary>
        /// The Anointed Automation user id (<c>sub</c>). Throws when the principal has none, because a user
        /// must never be keyed on anything else.
        /// </summary>
        /// <param name="principal">The signed-in principal.</param>
        /// <returns>The <c>sub</c> claim value.</returns>
        public static string GetAnointedSubject(this ClaimsPrincipal principal)
        {
            string sub = FindValue(principal, Subject);
            if (sub == null)
            {
                throw new InvalidOperationException("The principal carries no sub claim; it did not come from Sign in with Anointed Automation.");
            }
            return sub;
        }

        /// <summary>The <c>sid</c> (grant id), or null when the principal has none.</summary>
        /// <param name="principal">The signed-in principal.</param>
        /// <returns>The <c>sid</c> claim value or null.</returns>
        public static string GetAnointedSessionId(this ClaimsPrincipal principal)
        {
            return FindValue(principal, SessionId);
        }

        /// <summary>The verified email, or null when the email scope was not requested or no email was sent.</summary>
        /// <param name="principal">The signed-in principal.</param>
        /// <returns>The <c>email</c> claim value or null.</returns>
        public static string GetAnointedEmail(this ClaimsPrincipal principal)
        {
            return FindValue(principal, Email);
        }

        /// <summary>Whether the principal's <c>email_verified</c> claim is the boolean true.</summary>
        /// <param name="principal">The principal to inspect.</param>
        /// <returns>True only when the claim is present and equals <c>true</c> (case-insensitive).</returns>
        public static bool IsAnointedEmailVerified(this ClaimsPrincipal principal)
        {
            string value = FindValue(principal, EmailVerified);
            return value != null && string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>The first non-empty value of a claim type, or null.</summary>
        /// <param name="principal">The principal to search.</param>
        /// <param name="claimType">The exact claim type.</param>
        /// <returns>The claim value, or null when absent or empty.</returns>
        private static string FindValue(ClaimsPrincipal principal, string claimType)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }
            Claim claim = principal.FindFirst(claimType);
            if (claim == null || string.IsNullOrEmpty(claim.Value))
            {
                return null;
            }
            return claim.Value;
        }
    }
}
