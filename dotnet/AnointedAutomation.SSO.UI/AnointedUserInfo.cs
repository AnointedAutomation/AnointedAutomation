// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using System;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace AnointedAutomation.SSO.UI
{
    /// <summary>
    /// What the user-info route returns to a browser: who is signed in, never a token.
    /// </summary>
    public class AnointedUserInfo
    {
        /// <summary>True when a local session cookie is present and valid.</summary>
        [JsonPropertyName("isAuthenticated")]
        public bool IsAuthenticated { get; set; }

        /// <summary>The Anointed Automation user id (<c>sub</c>), or null when signed out.</summary>
        [JsonPropertyName("sub")]
        public string Sub { get; set; }

        /// <summary>The verified email, or null.</summary>
        [JsonPropertyName("email")]
        public string Email { get; set; }

        /// <summary>The display name (profile scope), or null.</summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>The handle (profile scope), or null.</summary>
        [JsonPropertyName("preferredUsername")]
        public string PreferredUsername { get; set; }

        /// <summary>The avatar URL (profile scope), or null.</summary>
        [JsonPropertyName("picture")]
        public string Picture { get; set; }

        /// <summary>The signed-out answer.</summary>
        /// <returns>An unauthenticated user info.</returns>
        public static AnointedUserInfo SignedOut()
        {
            return new AnointedUserInfo { IsAuthenticated = false };
        }

        /// <summary>Builds the user info from a signed-in principal.</summary>
        /// <param name="principal">The principal from the session cookie.</param>
        /// <returns>The user info.</returns>
        public static AnointedUserInfo FromPrincipal(ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }
            return new AnointedUserInfo
            {
                IsAuthenticated = true,
                Sub = principal.GetAnointedSubject(),
                Email = principal.GetAnointedEmail(),
                Name = Value(principal, AnointedClaims.Name),
                PreferredUsername = Value(principal, AnointedClaims.PreferredUsername),
                Picture = Value(principal, AnointedClaims.Picture),
            };
        }

        /// <summary>The first non-empty claim value or null.</summary>
        /// <param name="principal">The principal.</param>
        /// <param name="type">The claim type.</param>
        /// <returns>The value or null.</returns>
        private static string Value(ClaimsPrincipal principal, string type)
        {
            Claim claim = principal.FindFirst(type);
            if (claim == null || string.IsNullOrEmpty(claim.Value))
            {
                return null;
            }
            return claim.Value;
        }
    }
}
