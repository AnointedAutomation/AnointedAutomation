// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AnointedAutomation.SSO.Owin
{
    /// <summary>
    /// Registers "Sign in with Anointed Automation" on top of the standard Katana OpenID Connect middleware.
    /// </summary>
    public static class AnointedAutomationAppBuilderExtensions
    {
        /// <summary>The <c>app.Properties</c> key holding the back-channel logout handler.</summary>
        internal const string BackchannelLogoutHandlerKey = "AnointedAutomation.SSO.Owin.BackchannelLogoutHandler";

        /// <summary>
        /// Adds the Anointed Automation OpenID Connect middleware (authorization code + PKCE S256). The options
        /// are validated immediately, so a missing ClientId, ClientSecret or RedirectUri throws while the app is
        /// starting. The back-channel Logout Token validator is prepared too, ready for
        /// <see cref="AnointedBackchannelLogoutExtensions.MapAnointedBackchannelLogout"/>. Add cookie
        /// authentication before this call.
        /// </summary>
        /// <param name="app">The OWIN app builder.</param>
        /// <param name="options">ClientId, ClientSecret, RedirectUri and any optional settings.</param>
        /// <returns>The same app builder.</returns>
        /// <exception cref="InvalidOperationException">A required setting is missing or invalid (fail fast at startup).</exception>
        public static IAppBuilder UseAnointedAutomation(this IAppBuilder app, AnointedAutomationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            options.Validate();

            AnointedLogoutValidationOptions validation = new AnointedLogoutValidationOptions
            {
                Issuer = options.Issuer,
                ClientId = options.ClientId,
                MaxTokenAge = options.BackchannelLogoutMaxAge,
                ClockSkew = options.ClockSkew,
            };
            AnointedLogoutTokenValidator validator = new AnointedLogoutTokenValidator(
                validation,
                CreateConfigurationManager(new HttpClient(), validation.Issuer),
                TimeProvider.System);
            ILogger logger = app.CreateLogger<AnointedBackchannelLogoutHandler>();
            app.Properties[BackchannelLogoutHandlerKey] = new AnointedBackchannelLogoutHandler(validator, options.LogoutReplayStore, logger);

            OpenIdConnectAuthenticationOptions oidc = new OpenIdConnectAuthenticationOptions(options.AuthenticationType);
            ApplyTo(options, oidc);
            return app.UseOpenIdConnectAuthentication(oidc);
        }

        /// <summary>
        /// Copies validated Anointed Automation settings onto the Katana OpenID Connect options.
        /// </summary>
        /// <param name="anointed">Validated settings.</param>
        /// <param name="oidc">The middleware options to configure.</param>
        internal static void ApplyTo(AnointedAutomationOptions anointed, OpenIdConnectAuthenticationOptions oidc)
        {
            Uri redirect = new Uri(anointed.RedirectUri, UriKind.Absolute);
            oidc.Caption = anointed.Caption;
            oidc.Authority = anointed.Issuer;
            oidc.RequireHttpsMetadata = true;
            oidc.ClientId = anointed.ClientId;
            oidc.ClientSecret = anointed.ClientSecret;
            oidc.RedirectUri = anointed.RedirectUri;
            oidc.CallbackPath = new PathString(redirect.AbsolutePath);
            oidc.ResponseType = OpenIdConnectResponseType.Code;
            oidc.ResponseMode = OpenIdConnectResponseMode.Query;
            oidc.RedeemCode = true;
            oidc.UsePkce = true;
            oidc.Scope = string.Join(" ", anointed.GetScopes());
            if (anointed.SignInAsAuthenticationType != null)
            {
                oidc.SignInAsAuthenticationType = anointed.SignInAsAuthenticationType;
            }
            oidc.SecurityTokenValidator = new JwtSecurityTokenHandler { MapInboundClaims = false };
            oidc.TokenValidationParameters.NameClaimType = AnointedClaims.Name;
            oidc.TokenValidationParameters.ValidAlgorithms = new List<string> { AnointedAutomationDefaults.SigningAlgorithm };
            oidc.SaveTokens = anointed.SaveTokens;

            OpenIdConnectAuthenticationNotifications notifications = anointed.Notifications;
            if (notifications == null)
            {
                notifications = new OpenIdConnectAuthenticationNotifications();
            }
            ChainSecurityTokenValidated(notifications, anointed);
            oidc.Notifications = notifications;

            if (anointed.ConfigureOpenIdConnect != null)
            {
                anointed.ConfigureOpenIdConnect(oidc);
            }
        }

        /// <summary>
        /// Puts the verified-email check in front of whatever SecurityTokenValidated notification is already set.
        /// </summary>
        /// <param name="notifications">The notifications to modify.</param>
        /// <param name="anointed">Validated settings.</param>
        private static void ChainSecurityTokenValidated(OpenIdConnectAuthenticationNotifications notifications, AnointedAutomationOptions anointed)
        {
            Func<SecurityTokenValidatedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions>, Task> next = notifications.SecurityTokenValidated;
            bool enforce = anointed.RequestEmail && anointed.RequireVerifiedEmail;
            notifications.SecurityTokenValidated = async notification =>
            {
                if (enforce)
                {
                    ClaimsIdentity identity = notification.AuthenticationTicket == null ? null : notification.AuthenticationTicket.Identity;
                    EnforceVerifiedEmail(identity);
                }
                if (next != null)
                {
                    await next(notification).ConfigureAwait(false);
                }
            };
        }

        /// <summary>
        /// Throws <see cref="AnointedSignInRejectedException"/> when the identity's <c>email_verified</c> claim is
        /// not true. Katana turns the exception into its AuthenticationFailed notification.
        /// </summary>
        /// <param name="identity">The signed-in identity.</param>
        /// <exception cref="AnointedSignInRejectedException">The email is not verified.</exception>
        internal static void EnforceVerifiedEmail(ClaimsIdentity identity)
        {
            if (identity != null && new ClaimsPrincipal(identity).IsAnointedEmailVerified())
            {
                return;
            }
            throw new AnointedSignInRejectedException("email_not_verified: Anointed Automation did not vouch for a verified email for this user.");
        }

        /// <summary>
        /// Builds the discovery and JWKS cache the Logout Token validator reads signing keys from.
        /// </summary>
        /// <param name="httpClient">The client used for discovery and JWKS requests.</param>
        /// <param name="issuer">The issuer whose discovery document is read.</param>
        /// <returns>A configuration manager over https discovery.</returns>
        public static IConfigurationManager<OpenIdConnectConfiguration> CreateConfigurationManager(HttpClient httpClient, string issuer)
        {
            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }
            if (string.IsNullOrWhiteSpace(issuer))
            {
                throw new ArgumentException("An issuer is required.", nameof(issuer));
            }
            string metadataAddress = issuer.TrimEnd('/') + "/.well-known/openid-configuration";
            HttpDocumentRetriever retriever = new HttpDocumentRetriever(httpClient) { RequireHttps = true };
            return new ConfigurationManager<OpenIdConnectConfiguration>(metadataAddress, new OpenIdConnectConfigurationRetriever(), retriever);
        }
    }

    /// <summary>
    /// A sign-in this package refused (for example an unverified email). Handle it in the
    /// <c>AuthenticationFailed</c> notification and show the user a generic message.
    /// </summary>
    public class AnointedSignInRejectedException : Exception
    {
        /// <summary>Creates the exception.</summary>
        /// <param name="message">Why the sign-in was refused (for your logs).</param>
        public AnointedSignInRejectedException(string message) : base(message)
        {
        }
    }
}
