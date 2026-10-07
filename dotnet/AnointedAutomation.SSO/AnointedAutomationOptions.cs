// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnointedAutomation.SSO
{
    /// <summary>
    /// Settings for "Sign in with Anointed Automation". <see cref="ClientId"/>, <see cref="ClientSecret"/> and
    /// <see cref="CallbackPath"/> are required and have no defaults; <see cref="Validate"/> throws when one is
    /// missing so a misconfigured app fails at startup.
    /// </summary>
    public class AnointedAutomationOptions
    {
        /// <summary>The issuer. Leave it as <see cref="AnointedAutomationDefaults.Issuer"/> unless told otherwise. Must be https.</summary>
        public string Issuer { get; set; } = AnointedAutomationDefaults.Issuer;

        /// <summary>Your client_id, <c>aa_&lt;your key id&gt;</c>. Required.</summary>
        public string ClientId { get; set; }

        /// <summary>Your client secret. Required. Server only: read it from a secret store, never ship it to a browser or device.</summary>
        public string ClientSecret { get; set; }

        /// <summary>The callback path, for example <c>/auth/anointed/callback</c>. Required; the full URL must be registered exactly.</summary>
        public PathString CallbackPath { get; set; }

        /// <summary>Request the <c>email</c> scope. Default true (the default scope is <c>openid email</c>).</summary>
        public bool RequestEmail { get; set; } = true;

        /// <summary>
        /// Extra scopes on top of <c>openid</c> (and <c>email</c>), such as <c>profile</c> or <c>offline_access</c>.
        /// Asking for <c>profile</c> makes users without a username pick one before consenting.
        /// </summary>
        public ICollection<string> AdditionalScopes { get; } = new List<string>();

        /// <summary>With the email scope, refuse a sign-in whose <c>email_verified</c> claim is not true. Default true.</summary>
        public bool RequireVerifiedEmail { get; set; } = true;

        /// <summary>Store the tokens in the authentication properties. Default false; turn it on only if you call userinfo or refresh later.</summary>
        public bool SaveTokens { get; set; }

        /// <summary>Your own handler events. They are kept and chained after this package's checks, never overwritten.</summary>
        public OpenIdConnectEvents Events { get; set; }

        /// <summary>How old a back-channel Logout Token may be (from its <c>iat</c>). Default 5 minutes.</summary>
        public TimeSpan BackchannelLogoutMaxAge { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>Allowed clock skew for Logout Token <c>iat</c> and <c>exp</c>. Default 60 seconds.</summary>
        public TimeSpan ClockSkew { get; set; } = TimeSpan.FromSeconds(60);

        /// <summary>Optional last word on the produced <see cref="OpenIdConnectOptions"/> (runs after this package configured them).</summary>
        public Action<OpenIdConnectOptions> ConfigureOpenIdConnect { get; set; }

        /// <summary>
        /// Throws <see cref="InvalidOperationException"/> naming the first missing or invalid setting.
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Issuer))
            {
                throw new InvalidOperationException("AnointedAutomationOptions.Issuer is required.");
            }
            if (!Uri.TryCreate(Issuer, UriKind.Absolute, out Uri issuerUri) || !string.Equals(issuerUri.Scheme, Uri.UriSchemeHttps, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("AnointedAutomationOptions.Issuer must be an absolute https URL.");
            }
            if (string.IsNullOrWhiteSpace(ClientId))
            {
                throw new InvalidOperationException("AnointedAutomationOptions.ClientId is required (it looks like aa_<your key id>).");
            }
            if (!ClientId.StartsWith(AnointedAutomationDefaults.ClientIdPrefix, StringComparison.Ordinal) || ClientId.Length <= AnointedAutomationDefaults.ClientIdPrefix.Length)
            {
                throw new InvalidOperationException("AnointedAutomationOptions.ClientId must start with \"aa_\" followed by your key id.");
            }
            if (string.IsNullOrWhiteSpace(ClientSecret))
            {
                throw new InvalidOperationException("AnointedAutomationOptions.ClientSecret is required. Read it from your server's secret store.");
            }
            if (!CallbackPath.HasValue)
            {
                throw new InvalidOperationException("AnointedAutomationOptions.CallbackPath is required, for example /auth/anointed/callback.");
            }
            foreach (string scope in AdditionalScopes)
            {
                if (string.IsNullOrWhiteSpace(scope) || scope.Contains(' '))
                {
                    throw new InvalidOperationException("AnointedAutomationOptions.AdditionalScopes holds an empty scope or one with spaces; add each scope separately.");
                }
            }
            if (BackchannelLogoutMaxAge <= TimeSpan.Zero)
            {
                throw new InvalidOperationException("AnointedAutomationOptions.BackchannelLogoutMaxAge must be positive.");
            }
            if (ClockSkew < TimeSpan.Zero)
            {
                throw new InvalidOperationException("AnointedAutomationOptions.ClockSkew must not be negative.");
            }
        }

        /// <summary>The scopes that will be requested, in order and without duplicates.</summary>
        /// <returns>openid, then email when requested, then the additional scopes.</returns>
        public IReadOnlyList<string> GetScopes()
        {
            List<string> scopes = new List<string> { AnointedAutomationDefaults.OpenIdScope };
            if (RequestEmail)
            {
                scopes.Add(AnointedAutomationDefaults.EmailScope);
            }
            foreach (string scope in AdditionalScopes)
            {
                if (!scopes.Contains(scope, StringComparer.Ordinal))
                {
                    scopes.Add(scope);
                }
            }
            return scopes;
        }
    }
}
