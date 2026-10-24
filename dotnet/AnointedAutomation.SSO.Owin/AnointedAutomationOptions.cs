// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using Microsoft.Owin.Security.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnointedAutomation.SSO.Owin
{
    /// <summary>
    /// Settings for "Sign in with Anointed Automation" on OWIN/Katana. <see cref="ClientId"/>,
    /// <see cref="ClientSecret"/> and <see cref="RedirectUri"/> are required and have no defaults;
    /// <see cref="Validate"/> throws when one is missing so a misconfigured app fails at startup.
    /// </summary>
    public class AnointedAutomationOptions
    {
        /// <summary>The issuer. Leave it as <see cref="AnointedAutomationDefaults.Issuer"/> unless told otherwise. Must be https.</summary>
        public string Issuer { get; set; } = AnointedAutomationDefaults.Issuer;

        /// <summary>Your client_id, <c>aa_&lt;your key id&gt;</c>. Required.</summary>
        public string ClientId { get; set; }

        /// <summary>Your client secret. Required. Server only: read it from a secret store, never ship it to a browser or device.</summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// The full https callback URL, for example <c>https://www.example.com/auth/anointed/callback</c>. Required;
        /// it must be registered exactly. Its path becomes the middleware's callback path.
        /// </summary>
        public string RedirectUri { get; set; }

        /// <summary>The OWIN authentication type (scheme). Default <see cref="AnointedAutomationDefaults.AuthenticationType"/>.</summary>
        public string AuthenticationType { get; set; } = AnointedAutomationDefaults.AuthenticationType;

        /// <summary>The caption shown on login pages. Default <see cref="AnointedAutomationDefaults.DisplayName"/>.</summary>
        public string Caption { get; set; } = AnointedAutomationDefaults.DisplayName;

        /// <summary>
        /// The cookie authentication type the signed-in identity is handed to. When null, the app's default
        /// (<c>app.SetDefaultSignInAsAuthenticationType(...)</c>) is used, and Katana throws if none was set.
        /// </summary>
        public string SignInAsAuthenticationType { get; set; }

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

        /// <summary>Your own notifications. They are kept and chained after this package's checks, never overwritten.</summary>
        public OpenIdConnectAuthenticationNotifications Notifications { get; set; }

        /// <summary>How old a back-channel Logout Token may be (from its <c>iat</c>). Default 5 minutes.</summary>
        public TimeSpan BackchannelLogoutMaxAge { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>Allowed clock skew for Logout Token <c>iat</c> and <c>exp</c>. Default 60 seconds.</summary>
        public TimeSpan ClockSkew { get; set; } = TimeSpan.FromSeconds(60);

        /// <summary>
        /// Where processed Logout Token ids are remembered. Default: in memory (single instance). Set your own
        /// (for example over Redis) when the app runs on more than one server.
        /// </summary>
        public IAnointedLogoutReplayStore LogoutReplayStore { get; set; } = new InMemoryAnointedLogoutReplayStore(TimeProvider.System);

        /// <summary>Optional last word on the produced <see cref="OpenIdConnectAuthenticationOptions"/> (runs after this package configured them).</summary>
        public Action<OpenIdConnectAuthenticationOptions> ConfigureOpenIdConnect { get; set; }

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
            if (string.IsNullOrWhiteSpace(RedirectUri))
            {
                throw new InvalidOperationException("AnointedAutomationOptions.RedirectUri is required, for example https://www.example.com/auth/anointed/callback.");
            }
            if (!Uri.TryCreate(RedirectUri, UriKind.Absolute, out Uri redirect) || !string.Equals(redirect.Scheme, Uri.UriSchemeHttps, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("AnointedAutomationOptions.RedirectUri must be an absolute https URL (Anointed Automation never registers an http callback).");
            }
            if (!string.IsNullOrEmpty(redirect.Query) || !string.IsNullOrEmpty(redirect.Fragment) || string.Equals(redirect.AbsolutePath, "/", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("AnointedAutomationOptions.RedirectUri needs a dedicated path (for example /auth/anointed/callback) and no query or fragment.");
            }
            if (string.IsNullOrWhiteSpace(AuthenticationType))
            {
                throw new InvalidOperationException("AnointedAutomationOptions.AuthenticationType is required.");
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
            if (LogoutReplayStore == null)
            {
                throw new InvalidOperationException("AnointedAutomationOptions.LogoutReplayStore is required.");
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
