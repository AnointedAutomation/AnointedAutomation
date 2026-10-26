// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using AnointedAutomation.SSO.Owin;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace AnointedAutomation.SSO.Owin.Tests
{
    public class AnointedAutomationOptionsTests
    {
        /// <summary>Valid settings.</summary>
        /// <returns>Options that pass validation.</returns>
        internal static AnointedAutomationOptions ValidOptions()
        {
            return new AnointedAutomationOptions
            {
                ClientId = "aa_key1",
                ClientSecret = "secret",
                RedirectUri = "https://www.example.com/auth/anointed/callback",
            };
        }

        /// <summary>Applies the options onto fresh Katana options.</summary>
        /// <param name="anointed">The settings.</param>
        /// <returns>The configured Katana options.</returns>
        private static OpenIdConnectAuthenticationOptions Apply(AnointedAutomationOptions anointed)
        {
            anointed.Validate();
            OpenIdConnectAuthenticationOptions oidc = new OpenIdConnectAuthenticationOptions(anointed.AuthenticationType);
            AnointedAutomationAppBuilderExtensions.ApplyTo(anointed, oidc);
            return oidc;
        }

        /// <summary>Runs the chained SecurityTokenValidated notification against an identity.</summary>
        /// <param name="oidc">The configured options.</param>
        /// <param name="claims">The identity's claims.</param>
        /// <returns>The notification that ran.</returns>
        private static Task Validated(OpenIdConnectAuthenticationOptions oidc, params Claim[] claims)
        {
            SecurityTokenValidatedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> notification =
                new SecurityTokenValidatedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions>(new Microsoft.Owin.OwinContext(), oidc)
                {
                    AuthenticationTicket = new Microsoft.Owin.Security.AuthenticationTicket(new ClaimsIdentity(claims, "Anointed"), new Microsoft.Owin.Security.AuthenticationProperties()),
                };
            return oidc.Notifications.SecurityTokenValidated(notification);
        }

        [Fact]
        public void Validate_Valid_DoesNotThrow()
        {
            ValidOptions().Validate();
        }

        [Theory]
        [InlineData(null, "secret", "https://a.test/cb", "ClientId")]
        [InlineData("", "secret", "https://a.test/cb", "ClientId")]
        [InlineData("key1", "secret", "https://a.test/cb", "aa_")]
        [InlineData("aa_", "secret", "https://a.test/cb", "aa_")]
        [InlineData("aa_key1", null, "https://a.test/cb", "ClientSecret")]
        [InlineData("aa_key1", " ", "https://a.test/cb", "ClientSecret")]
        [InlineData("aa_key1", "secret", null, "RedirectUri")]
        [InlineData("aa_key1", "secret", "/cb", "RedirectUri")]
        [InlineData("aa_key1", "secret", "http://a.test/cb", "RedirectUri")]
        [InlineData("aa_key1", "secret", "https://a.test/", "RedirectUri")]
        [InlineData("aa_key1", "secret", "https://a.test/cb?x=1", "RedirectUri")]
        public void Validate_Invalid_ThrowsNamingSetting(string clientId, string secret, string redirect, string named)
        {
            AnointedAutomationOptions options = new AnointedAutomationOptions { ClientId = clientId, ClientSecret = secret, RedirectUri = redirect };
            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => options.Validate());
            Assert.Contains(named, ex.Message, StringComparison.Ordinal);
        }

        [Theory]
        [InlineData("http://api.anointedautomation.net/")]
        [InlineData("not a url")]
        [InlineData("")]
        public void Validate_BadIssuer_Throws(string issuer)
        {
            AnointedAutomationOptions options = ValidOptions();
            options.Issuer = issuer;
            Assert.Throws<InvalidOperationException>(() => options.Validate());
        }

        [Fact]
        public void Validate_ScopeWithSpace_Throws()
        {
            AnointedAutomationOptions options = ValidOptions();
            options.AdditionalScopes.Add("profile email");
            Assert.Throws<InvalidOperationException>(() => options.Validate());
        }

        [Fact]
        public void Validate_NullReplayStore_Throws()
        {
            AnointedAutomationOptions options = ValidOptions();
            options.LogoutReplayStore = null;
            Assert.Throws<InvalidOperationException>(() => options.Validate());
        }

        [Fact]
        public void UseAnointedAutomation_MissingSecret_ThrowsAtStartup()
        {
            AnointedAutomationOptions options = ValidOptions();
            options.ClientSecret = null;
            Assert.Throws<InvalidOperationException>(() => new FakeAppBuilder().UseAnointedAutomation(options));
        }

        [Fact]
        public void Configures_OpenIdConnectMiddleware()
        {
            OpenIdConnectAuthenticationOptions oidc = Apply(ValidOptions());
            Assert.Equal("Anointed", oidc.AuthenticationType);
            Assert.Equal("Anointed Automation", oidc.Caption);
            Assert.Equal(AnointedAutomationDefaults.Issuer, oidc.Authority);
            Assert.True(oidc.RequireHttpsMetadata);
            Assert.Equal("aa_key1", oidc.ClientId);
            Assert.Equal("secret", oidc.ClientSecret);
            Assert.Equal("https://www.example.com/auth/anointed/callback", oidc.RedirectUri);
            Assert.Equal("/auth/anointed/callback", oidc.CallbackPath.Value);
            Assert.Equal(OpenIdConnectResponseType.Code, oidc.ResponseType);
            Assert.Equal(OpenIdConnectResponseMode.Query, oidc.ResponseMode);
            Assert.True(oidc.RedeemCode);
            Assert.True(oidc.UsePkce);
            Assert.Equal("openid email", oidc.Scope);
            Assert.False(oidc.SaveTokens);
            Assert.Null(oidc.SignInAsAuthenticationType);
            Assert.False(Assert.IsType<JwtSecurityTokenHandler>(oidc.SecurityTokenValidator).MapInboundClaims);
            Assert.Equal("name", oidc.TokenValidationParameters.NameClaimType);
            Assert.Equal(new[] { "RS256" }, oidc.TokenValidationParameters.ValidAlgorithms.ToArray());
        }

        [Fact]
        public void SignInAsAndSaveTokens_AreCopied()
        {
            AnointedAutomationOptions options = ValidOptions();
            options.SignInAsAuthenticationType = "Cookies";
            options.SaveTokens = true;
            OpenIdConnectAuthenticationOptions oidc = Apply(options);
            Assert.Equal("Cookies", oidc.SignInAsAuthenticationType);
            Assert.True(oidc.SaveTokens);
        }

        [Fact]
        public void Scopes_AdditionalAndDeduplicated()
        {
            AnointedAutomationOptions options = ValidOptions();
            options.AdditionalScopes.Add("profile");
            options.AdditionalScopes.Add("email");
            options.AdditionalScopes.Add("offline_access");
            Assert.Equal("openid email profile offline_access", Apply(options).Scope);
        }

        [Fact]
        public void Scopes_WithoutEmail_OnlyOpenId()
        {
            AnointedAutomationOptions options = ValidOptions();
            options.RequestEmail = false;
            Assert.Equal("openid", Apply(options).Scope);
        }

        [Fact]
        public void ConfigureOpenIdConnect_RunsLast()
        {
            AnointedAutomationOptions options = ValidOptions();
            options.ConfigureOpenIdConnect = oidc => oidc.Caption = "Custom";
            Assert.Equal("Custom", Apply(options).Caption);
        }

        [Fact]
        public async Task VerifiedEmail_PassesAndChainsUserNotification()
        {
            AnointedAutomationOptions options = ValidOptions();
            int userCalls = 0;
            options.Notifications = new OpenIdConnectAuthenticationNotifications
            {
                SecurityTokenValidated = notification =>
                {
                    userCalls++;
                    return Task.CompletedTask;
                },
            };
            await Validated(Apply(options), new Claim("sub", "u1"), new Claim("email_verified", "true"));
            Assert.Equal(1, userCalls);
        }

        [Theory]
        [InlineData("false")]
        [InlineData(null)]
        public async Task UnverifiedEmail_ThrowsAndSkipsUserNotification(string verified)
        {
            AnointedAutomationOptions options = ValidOptions();
            int userCalls = 0;
            options.Notifications = new OpenIdConnectAuthenticationNotifications
            {
                SecurityTokenValidated = notification =>
                {
                    userCalls++;
                    return Task.CompletedTask;
                },
            };
            List<Claim> claims = new List<Claim> { new Claim("sub", "u1") };
            if (verified != null)
            {
                claims.Add(new Claim("email_verified", verified));
            }
            OpenIdConnectAuthenticationOptions oidc = Apply(options);
            AnointedSignInRejectedException ex = await Assert.ThrowsAsync<AnointedSignInRejectedException>(() => Validated(oidc, claims.ToArray()));
            Assert.StartsWith("email_not_verified", ex.Message, StringComparison.Ordinal);
            Assert.Equal(0, userCalls);
        }

        [Fact]
        public async Task RequireVerifiedEmailOff_DoesNotThrow()
        {
            AnointedAutomationOptions options = ValidOptions();
            options.RequireVerifiedEmail = false;
            await Validated(Apply(options), new Claim("sub", "u1"));
        }

        [Fact]
        public async Task NoEmailScope_DoesNotRequireVerifiedEmail()
        {
            AnointedAutomationOptions options = ValidOptions();
            options.RequestEmail = false;
            await Validated(Apply(options), new Claim("sub", "u1"));
        }

        [Fact]
        public void MapBackchannelLogout_WithoutUseAnointedAutomation_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => new FakeAppBuilder().MapAnointedBackchannelLogout("/logout", (logout, context) => Task.CompletedTask));
        }

        [Fact]
        public void UseAnointedAutomation_RegistersMiddlewareAndLogoutHandler()
        {
            FakeAppBuilder app = new FakeAppBuilder();
            AnointedAutomationOptions options = ValidOptions();
            options.SignInAsAuthenticationType = "Cookies";
            app.UseAnointedAutomation(options);
            Assert.IsType<AnointedBackchannelLogoutHandler>(app.Properties["AnointedAutomation.SSO.Owin.BackchannelLogoutHandler"]);
            Assert.Single(app.Registered);
            app.MapAnointedBackchannelLogout("/auth/anointed/backchannel-logout", (logout, context) => Task.CompletedTask);
            Assert.Equal(typeof(AnointedBackchannelLogoutMiddleware), app.Registered[1]);
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

        [Theory]
        [InlineData("true", true)]
        [InlineData("True", true)]
        [InlineData("false", false)]
        [InlineData("yes", false)]
        public void EmailVerified_OnlyTrueCounts(string value, bool expected)
        {
            Assert.Equal(expected, Principal(new Claim("email_verified", value)).IsAnointedEmailVerified());
        }
    }
}
