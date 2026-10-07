// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using AnointedAutomation.SSO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace AnointedAutomation.SSO.Tests
{
    public class AnointedAutomationOptionsTests
    {
        /// <summary>A complete, valid configuration.</summary>
        /// <param name="options">The options to fill.</param>
        private static void Valid(AnointedAutomationOptions options)
        {
            options.ClientId = "aa_key1";
            options.ClientSecret = "secret";
            options.CallbackPath = "/auth/anointed/callback";
        }

        /// <summary>Registers the handler and returns its resolved options.</summary>
        /// <param name="configure">The configuration.</param>
        /// <param name="scheme">The scheme to resolve.</param>
        /// <returns>The handler options.</returns>
        private static OpenIdConnectOptions Resolve(Action<AnointedAutomationOptions> configure, string scheme)
        {
            ServiceCollection services = new ServiceCollection();
            services.AddLogging();
            if (string.Equals(scheme, AnointedAutomationDefaults.AuthenticationScheme, StringComparison.Ordinal))
            {
                services.AddAuthentication().AddAnointedAutomation(configure);
            }
            else
            {
                services.AddAuthentication().AddAnointedAutomation(scheme, configure);
            }
            ServiceProvider provider = services.BuildServiceProvider();
            return provider.GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>().Get(scheme);
        }

        /// <summary>Builds a token validated context for a principal.</summary>
        /// <param name="options">The handler options.</param>
        /// <param name="claims">The principal's claims.</param>
        /// <returns>The context.</returns>
        private static TokenValidatedContext Context(OpenIdConnectOptions options, params Claim[] claims)
        {
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "test"));
            AuthenticationScheme scheme = new AuthenticationScheme(AnointedAutomationDefaults.AuthenticationScheme, null, typeof(OpenIdConnectHandler));
            return new TokenValidatedContext(new DefaultHttpContext(), scheme, options, principal, new AuthenticationProperties());
        }

        [Fact]
        public void Validate_Valid_DoesNotThrow()
        {
            AnointedAutomationOptions options = new AnointedAutomationOptions();
            Valid(options);
            options.Validate();
        }

        [Theory]
        [InlineData(null, "secret", "/cb", "ClientId")]
        [InlineData("", "secret", "/cb", "ClientId")]
        [InlineData("key1", "secret", "/cb", "aa_")]
        [InlineData("aa_", "secret", "/cb", "aa_")]
        [InlineData("aa_key1", null, "/cb", "ClientSecret")]
        [InlineData("aa_key1", " ", "/cb", "ClientSecret")]
        [InlineData("aa_key1", "secret", null, "CallbackPath")]
        public void Validate_Invalid_ThrowsNamingSetting(string clientId, string secret, string callback, string named)
        {
            AnointedAutomationOptions options = new AnointedAutomationOptions { ClientId = clientId, ClientSecret = secret };
            if (callback != null)
            {
                options.CallbackPath = callback;
            }
            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => options.Validate());
            Assert.Contains(named, ex.Message, StringComparison.Ordinal);
        }

        [Theory]
        [InlineData("http://api.anointedautomation.net/")]
        [InlineData("not a url")]
        [InlineData("")]
        public void Validate_BadIssuer_Throws(string issuer)
        {
            AnointedAutomationOptions options = new AnointedAutomationOptions();
            Valid(options);
            options.Issuer = issuer;
            Assert.Throws<InvalidOperationException>(() => options.Validate());
        }

        [Fact]
        public void Validate_ScopeWithSpace_Throws()
        {
            AnointedAutomationOptions options = new AnointedAutomationOptions();
            Valid(options);
            options.AdditionalScopes.Add("profile offline_access");
            Assert.Throws<InvalidOperationException>(() => options.Validate());
        }

        [Fact]
        public void AddAnointedAutomation_MissingSecret_ThrowsAtRegistration()
        {
            ServiceCollection services = new ServiceCollection();
            Assert.Throws<InvalidOperationException>(() => services.AddAuthentication().AddAnointedAutomation(options =>
            {
                options.ClientId = "aa_key1";
                options.CallbackPath = "/cb";
            }));
        }

        [Fact]
        public void Configures_OpenIdConnectHandler()
        {
            OpenIdConnectOptions oidc = Resolve(Valid, AnointedAutomationDefaults.AuthenticationScheme);
            Assert.Equal("https://api.anointedautomation.net/", oidc.Authority);
            Assert.True(oidc.RequireHttpsMetadata);
            Assert.Equal("aa_key1", oidc.ClientId);
            Assert.Equal("secret", oidc.ClientSecret);
            Assert.Equal("/auth/anointed/callback", oidc.CallbackPath.Value);
            Assert.Equal(OpenIdConnectResponseType.Code, oidc.ResponseType);
            Assert.Equal(OpenIdConnectResponseMode.Query, oidc.ResponseMode);
            Assert.True(oidc.UsePkce);
            Assert.Equal(new[] { "openid", "email" }, oidc.Scope.ToArray());
            Assert.False(oidc.MapInboundClaims);
            Assert.Equal("name", oidc.TokenValidationParameters.NameClaimType);
            Assert.Equal(new[] { "RS256" }, oidc.TokenValidationParameters.ValidAlgorithms.ToArray());
            Assert.False(oidc.SaveTokens);
            Assert.False(oidc.GetClaimsFromUserInfoEndpoint);
        }

        [Fact]
        public void Scopes_AdditionalAndDeduplicated()
        {
            OpenIdConnectOptions oidc = Resolve(options =>
            {
                Valid(options);
                options.AdditionalScopes.Add("email");
                options.AdditionalScopes.Add(AnointedAutomationDefaults.ProfileScope);
                options.AdditionalScopes.Add(AnointedAutomationDefaults.OfflineAccessScope);
                options.SaveTokens = true;
            }, AnointedAutomationDefaults.AuthenticationScheme);
            Assert.Equal(new[] { "openid", "email", "profile", "offline_access" }, oidc.Scope.ToArray());
            Assert.True(oidc.SaveTokens);
        }

        [Fact]
        public void Scopes_WithoutEmail_OnlyOpenId()
        {
            OpenIdConnectOptions oidc = Resolve(options =>
            {
                Valid(options);
                options.RequestEmail = false;
            }, AnointedAutomationDefaults.AuthenticationScheme);
            Assert.Equal(new[] { "openid" }, oidc.Scope.ToArray());
        }

        [Fact]
        public void CustomScheme_IsRegistered()
        {
            OpenIdConnectOptions oidc = Resolve(Valid, "Partner");
            Assert.Equal("aa_key1", oidc.ClientId);
        }

        [Fact]
        public void ConfigureOpenIdConnect_RunsLast()
        {
            OpenIdConnectOptions oidc = Resolve(options =>
            {
                Valid(options);
                options.ConfigureOpenIdConnect = handler => handler.ResponseMode = OpenIdConnectResponseMode.FormPost;
            }, AnointedAutomationDefaults.AuthenticationScheme);
            Assert.Equal(OpenIdConnectResponseMode.FormPost, oidc.ResponseMode);
        }

        [Fact]
        public async Task VerifiedEmail_PassesAndChainsUserHandler()
        {
            bool userHandlerCalled = false;
            OpenIdConnectEvents events = new OpenIdConnectEvents
            {
                OnTokenValidated = context =>
                {
                    userHandlerCalled = true;
                    return Task.CompletedTask;
                },
            };
            OpenIdConnectOptions oidc = Resolve(options =>
            {
                Valid(options);
                options.Events = events;
            }, AnointedAutomationDefaults.AuthenticationScheme);
            Assert.Same(events, oidc.Events);
            TokenValidatedContext context = Context(oidc, new Claim("sub", "u1"), new Claim("email", "a@b.c"), new Claim("email_verified", "true", ClaimValueTypes.Boolean));
            await oidc.Events.OnTokenValidated(context);
            Assert.True(userHandlerCalled);
            Assert.Null(context.Result);
        }

        [Theory]
        [InlineData("false")]
        [InlineData(null)]
        public async Task UnverifiedEmail_FailsAndSkipsUserHandler(string verified)
        {
            bool userHandlerCalled = false;
            OpenIdConnectOptions oidc = Resolve(options =>
            {
                Valid(options);
                options.Events = new OpenIdConnectEvents
                {
                    OnTokenValidated = context =>
                    {
                        userHandlerCalled = true;
                        return Task.CompletedTask;
                    },
                };
            }, AnointedAutomationDefaults.AuthenticationScheme);
            TokenValidatedContext context = verified == null
                ? Context(oidc, new Claim("sub", "u1"))
                : Context(oidc, new Claim("sub", "u1"), new Claim("email", "a@b.c"), new Claim("email_verified", verified));
            await oidc.Events.OnTokenValidated(context);
            Assert.False(userHandlerCalled);
            Assert.NotNull(context.Result);
            Assert.NotNull(context.Result.Failure);
            Assert.Contains("email_not_verified", context.Result.Failure.Message, StringComparison.Ordinal);
        }

        [Fact]
        public async Task RequireVerifiedEmailOff_DoesNotFail()
        {
            OpenIdConnectOptions oidc = Resolve(options =>
            {
                Valid(options);
                options.RequireVerifiedEmail = false;
            }, AnointedAutomationDefaults.AuthenticationScheme);
            TokenValidatedContext context = Context(oidc, new Claim("sub", "u1"));
            await oidc.Events.OnTokenValidated(context);
            Assert.Null(context.Result);
        }

        [Fact]
        public async Task NoEmailScope_DoesNotRequireVerifiedEmail()
        {
            OpenIdConnectOptions oidc = Resolve(options =>
            {
                Valid(options);
                options.RequestEmail = false;
            }, AnointedAutomationDefaults.AuthenticationScheme);
            TokenValidatedContext context = Context(oidc, new Claim("sub", "u1"));
            await oidc.Events.OnTokenValidated(context);
            Assert.Null(context.Result);
        }

        [Fact]
        public void RegistersLogoutServices()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddLogging();
            services.AddAuthentication().AddAnointedAutomation(Valid);
            ServiceProvider provider = services.BuildServiceProvider();
            AnointedLogoutTokenValidator validator = provider.GetRequiredService<AnointedLogoutTokenValidator>();
            Assert.Equal("aa_key1", validator.Options.ClientId);
            Assert.Equal(AnointedAutomationDefaults.Issuer, validator.Options.Issuer);
            Assert.IsType<InMemoryAnointedLogoutReplayStore>(provider.GetRequiredService<IAnointedLogoutReplayStore>());
            Assert.NotNull(provider.GetRequiredService<AnointedBackchannelLogoutHandler>());
        }
    }

    public class AnointedClaimsTests
    {
        /// <summary>A principal with the given claims.</summary>
        /// <param name="claims">The claims.</param>
        /// <returns>The principal.</returns>
        private static ClaimsPrincipal Principal(params Claim[] claims)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(claims, "test"));
        }

        [Fact]
        public void Reads_Sub_Sid_Email()
        {
            ClaimsPrincipal principal = Principal(new Claim("sub", "u1"), new Claim("sid", "g1"), new Claim("email", "a@b.c"));
            Assert.Equal("u1", principal.GetAnointedSubject());
            Assert.Equal("g1", principal.GetAnointedSessionId());
            Assert.Equal("a@b.c", principal.GetAnointedEmail());
        }

        [Fact]
        public void MissingSub_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => Principal(new Claim("email", "a@b.c")).GetAnointedSubject());
        }

        [Fact]
        public void MissingOptional_ReturnsNull()
        {
            ClaimsPrincipal principal = Principal(new Claim("sub", "u1"));
            Assert.Null(principal.GetAnointedSessionId());
            Assert.Null(principal.GetAnointedEmail());
        }

        [Theory]
        [InlineData("true", true)]
        [InlineData("True", true)]
        [InlineData("false", false)]
        [InlineData("yes", false)]
        public void EmailVerified_OnlyTrueCounts(string value, bool expected)
        {
            Assert.Equal(expected, Principal(new Claim("email_verified", value)).IsAnointedEmailVerified());
        }

        [Fact]
        public void EmailVerified_Missing_False()
        {
            Assert.False(Principal(new Claim("sub", "u1")).IsAnointedEmailVerified());
        }

        [Fact]
        public void NullPrincipal_Throws()
        {
            ClaimsPrincipal principal = null;
            Assert.Throws<ArgumentNullException>(() => principal.GetAnointedEmail());
        }
    }
}
