// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AnointedAutomation.SSO
{
    /// <summary>
    /// Registers "Sign in with Anointed Automation" on top of the standard ASP.NET Core OpenID Connect handler.
    /// </summary>
    public static class AnointedAutomationExtensions
    {
        /// <summary>The named HttpClient used to read discovery and the JWKS for Logout Token validation.</summary>
        public const string MetadataHttpClientName = "AnointedAutomation.SSO.Metadata";

        /// <summary>
        /// Adds the Anointed Automation OpenID Connect handler under the scheme <c>Anointed</c>.
        /// </summary>
        /// <param name="builder">The authentication builder.</param>
        /// <param name="configure">Sets ClientId, ClientSecret, CallbackPath and any optional settings.</param>
        /// <returns>The same builder.</returns>
        /// <exception cref="InvalidOperationException">A required setting is missing or invalid (fail fast at startup).</exception>
        public static AuthenticationBuilder AddAnointedAutomation(this AuthenticationBuilder builder, Action<AnointedAutomationOptions> configure)
        {
            return AddAnointedAutomation(builder, AnointedAutomationDefaults.AuthenticationScheme, AnointedAutomationDefaults.DisplayName, configure);
        }

        /// <summary>
        /// Adds the Anointed Automation OpenID Connect handler under a scheme name of your choice.
        /// </summary>
        /// <param name="builder">The authentication builder.</param>
        /// <param name="authenticationScheme">The scheme name.</param>
        /// <param name="configure">Sets ClientId, ClientSecret, CallbackPath and any optional settings.</param>
        /// <returns>The same builder.</returns>
        public static AuthenticationBuilder AddAnointedAutomation(this AuthenticationBuilder builder, string authenticationScheme, Action<AnointedAutomationOptions> configure)
        {
            return AddAnointedAutomation(builder, authenticationScheme, AnointedAutomationDefaults.DisplayName, configure);
        }

        /// <summary>
        /// Adds the Anointed Automation OpenID Connect handler under a scheme and display name of your choice.
        /// The options are validated immediately, so a missing ClientId, ClientSecret or CallbackPath throws
        /// while the app is starting. The back-channel Logout Token validator is registered too, ready for
        /// <see cref="AnointedBackchannelLogoutExtensions.MapAnointedBackchannelLogout"/>.
        /// </summary>
        /// <param name="builder">The authentication builder.</param>
        /// <param name="authenticationScheme">The scheme name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="configure">Sets ClientId, ClientSecret, CallbackPath and any optional settings.</param>
        /// <returns>The same builder.</returns>
        public static AuthenticationBuilder AddAnointedAutomation(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<AnointedAutomationOptions> configure)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (string.IsNullOrWhiteSpace(authenticationScheme))
            {
                throw new ArgumentException("An authentication scheme name is required.", nameof(authenticationScheme));
            }
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            AnointedAutomationOptions anointed = new AnointedAutomationOptions();
            configure(anointed);
            anointed.Validate();

            RegisterLogoutServices(builder.Services, anointed);

            return builder.AddOpenIdConnect(authenticationScheme, displayName, oidc => ApplyTo(anointed, oidc));
        }

        /// <summary>
        /// Copies validated Anointed Automation settings onto the OpenID Connect handler options.
        /// </summary>
        /// <param name="anointed">Validated settings.</param>
        /// <param name="oidc">The handler options to configure.</param>
        internal static void ApplyTo(AnointedAutomationOptions anointed, OpenIdConnectOptions oidc)
        {
            oidc.Authority = anointed.Issuer;
            oidc.RequireHttpsMetadata = true;
            oidc.ClientId = anointed.ClientId;
            oidc.ClientSecret = anointed.ClientSecret;
            oidc.CallbackPath = anointed.CallbackPath;
            oidc.ResponseType = OpenIdConnectResponseType.Code;
            oidc.ResponseMode = OpenIdConnectResponseMode.Query;
            oidc.UsePkce = true;
            oidc.Scope.Clear();
            foreach (string scope in anointed.GetScopes())
            {
                oidc.Scope.Add(scope);
            }
            oidc.MapInboundClaims = false;
            oidc.TokenValidationParameters.NameClaimType = AnointedClaims.Name;
            oidc.TokenValidationParameters.ValidAlgorithms = new List<string> { AnointedAutomationDefaults.SigningAlgorithm };
            oidc.SaveTokens = anointed.SaveTokens;
            oidc.GetClaimsFromUserInfoEndpoint = false;

            OpenIdConnectEvents events = anointed.Events;
            if (events == null)
            {
                events = new OpenIdConnectEvents();
            }
            ChainTokenValidated(events, anointed);
            oidc.Events = events;

            if (anointed.ConfigureOpenIdConnect != null)
            {
                anointed.ConfigureOpenIdConnect(oidc);
            }
        }

        /// <summary>
        /// Puts the verified-email check in front of whatever OnTokenValidated handler is already set.
        /// </summary>
        /// <param name="events">The events object to modify.</param>
        /// <param name="anointed">Validated settings.</param>
        private static void ChainTokenValidated(OpenIdConnectEvents events, AnointedAutomationOptions anointed)
        {
            Func<TokenValidatedContext, Task> next = events.OnTokenValidated;
            bool enforce = anointed.RequestEmail && anointed.RequireVerifiedEmail;
            events.OnTokenValidated = async context =>
            {
                if (enforce && !EnforceVerifiedEmail(context))
                {
                    return;
                }
                if (next != null)
                {
                    await next(context).ConfigureAwait(false);
                }
            };
        }

        /// <summary>
        /// Fails the sign-in when the principal's <c>email_verified</c> claim is not true.
        /// </summary>
        /// <param name="context">The token validated context.</param>
        /// <returns>True when the sign-in may continue; false when it was failed.</returns>
        internal static bool EnforceVerifiedEmail(TokenValidatedContext context)
        {
            if (context.Principal != null && context.Principal.IsAnointedEmailVerified())
            {
                return true;
            }
            context.Fail("email_not_verified: Anointed Automation did not vouch for a verified email for this user.");
            return false;
        }

        /// <summary>
        /// Registers the Logout Token validator, its metadata client and the default replay store.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="anointed">Validated settings.</param>
        private static void RegisterLogoutServices(IServiceCollection services, AnointedAutomationOptions anointed)
        {
            AnointedLogoutValidationOptions validation = new AnointedLogoutValidationOptions
            {
                Issuer = anointed.Issuer,
                ClientId = anointed.ClientId,
                MaxTokenAge = anointed.BackchannelLogoutMaxAge,
                ClockSkew = anointed.ClockSkew,
            };
            validation.Validate();

            services.AddHttpClient(MetadataHttpClientName);
            services.TryAddSingleton(TimeProvider.System);
            services.AddLogging();
            services.TryAddSingleton<IAnointedLogoutReplayStore, InMemoryAnointedLogoutReplayStore>();
            services.TryAddSingleton(serviceProvider => new AnointedLogoutTokenValidator(
                validation,
                CreateConfigurationManager(serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(MetadataHttpClientName), validation.Issuer),
                serviceProvider.GetRequiredService<TimeProvider>()));
            services.TryAddSingleton<AnointedBackchannelLogoutHandler>();
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
}
