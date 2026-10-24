// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using AnointedAutomation.SSO;
using AnointedAutomation.SSO.UI;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace AnointedAutomation.SSO.UI.Tests
{
    [TestFixture]
    public class AuthEndpointTests
    {
        private IHost _host;
        private HttpClient _client;

        [SetUp]
        public async Task SetUp()
        {
            _host = await new HostBuilder()
                .ConfigureWebHost(web => web
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddRouting();
                        services.AddAntiforgery();
                        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                            .AddCookie()
                            .AddCookie(AnointedAutomationDefaults.AuthenticationScheme, o => o.LoginPath = "/idp");
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseAuthentication();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapAnointedAutomationAuth("/auth/anointed");
                            endpoints.MapGet("/test/sign-in", async (HttpContext context) =>
                            {
                                ClaimsIdentity identity = new ClaimsIdentity(new[]
                                {
                                    new Claim(AnointedClaims.Subject, "user-1"),
                                    new Claim(AnointedClaims.Email, "a@example.com"),
                                    new Claim(AnointedClaims.Name, "Ann"),
                                }, CookieAuthenticationDefaults.AuthenticationScheme);
                                AuthenticationProperties properties = new AuthenticationProperties();
                                properties.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = "SECRET-TOKEN" } });
                                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), properties);
                            });
                            endpoints.MapGet("/test/token", (HttpContext context, IAntiforgery antiforgery) =>
                            {
                                AntiforgeryTokenSet tokens = antiforgery.GetAndStoreTokens(context);
                                return tokens.RequestToken;
                            });
                        });
                    }))
                .StartAsync();
            _client = _host.GetTestClient();
        }

        [TearDown]
        public async Task TearDown()
        {
            _client.Dispose();
            await _host.StopAsync();
            _host.Dispose();
        }

        private static string Cookies(HttpResponseMessage response)
        {
            return string.Join("; ", response.Headers.GetValues("Set-Cookie").Select(c => c.Split(';')[0]));
        }

        private async Task<string> SignInAsync()
        {
            HttpResponseMessage response = await _client.GetAsync("/test/sign-in");
            return Cookies(response);
        }

        private async Task<JsonElement> UserAsync(string cookie)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "/auth/anointed/user");
            if (cookie != null)
            {
                request.Headers.Add("Cookie", cookie);
            }
            HttpResponseMessage response = await _client.SendAsync(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Headers.CacheControl.NoStore, Is.True);
            string body = await response.Content.ReadAsStringAsync();
            Assert.That(body, Does.Not.Contain("SECRET-TOKEN"));
            return JsonDocument.Parse(body).RootElement;
        }

        [TestCase("/account", "%2Faccount")]
        [TestCase("//evil.example", "%2F")]
        [TestCase("https://evil.example", "%2F")]
        public async Task Start_ChallengesWithSafeReturn(string returnTo, string expectedEncoded)
        {
            HttpResponseMessage response = await _client.GetAsync("/auth/anointed/start?returnTo=" + Uri.EscapeDataString(returnTo));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
            string location = response.Headers.Location.ToString();
            Assert.That(location, Does.Contain("/idp?ReturnUrl=" + expectedEncoded));
            Assert.That(location.EndsWith("ReturnUrl=" + expectedEncoded, StringComparison.Ordinal), Is.True);
        }

        [Test]
        public async Task User_SignedOut_ReturnsFalse()
        {
            JsonElement user = await UserAsync(null);
            Assert.That(user.GetProperty("isAuthenticated").GetBoolean(), Is.False);
        }

        [Test]
        public async Task User_SignedIn_ReturnsClaimsButNoTokens()
        {
            string cookie = await SignInAsync();
            JsonElement user = await UserAsync(cookie);
            Assert.That(user.GetProperty("isAuthenticated").GetBoolean(), Is.True);
            Assert.That(user.GetProperty("sub").GetString(), Is.EqualTo("user-1"));
            Assert.That(user.GetProperty("email").GetString(), Is.EqualTo("a@example.com"));
            Assert.That(user.GetProperty("name").GetString(), Is.EqualTo("Ann"));
        }

        [Test]
        public async Task SignOut_WithoutCsrfProof_Is400()
        {
            string cookie = await SignInAsync();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/auth/anointed/sign-out");
            request.Headers.Add("Cookie", cookie);
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>());
            HttpResponseMessage response = await _client.SendAsync(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task SignOut_FromScript_ClearsCookieWith204()
        {
            string cookie = await SignInAsync();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/auth/anointed/sign-out");
            request.Headers.Add("Cookie", cookie);
            request.Headers.Add(AnointedAuthEndpointOptions.CsrfHeaderName, "1");
            HttpResponseMessage response = await _client.SendAsync(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
            Assert.That(string.Join(",", response.Headers.GetValues("Set-Cookie")), Does.Contain("expires=Thu, 01 Jan 1970"));
        }

        [TestCase("/bye", "/bye")]
        [TestCase("//evil.example", "/")]
        public async Task SignOut_FormWithAntiforgery_RedirectsToSafeTarget(string returnTo, string expected)
        {
            string sessionCookie = await SignInAsync();
            HttpRequestMessage tokenRequest = new HttpRequestMessage(HttpMethod.Get, "/test/token");
            tokenRequest.Headers.Add("Cookie", sessionCookie);
            HttpResponseMessage tokenResponse = await _client.SendAsync(tokenRequest);
            string requestToken = await tokenResponse.Content.ReadAsStringAsync();
            string cookie = sessionCookie + "; " + Cookies(tokenResponse);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/auth/anointed/sign-out");
            request.Headers.Add("Cookie", cookie);
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "__RequestVerificationToken", requestToken },
                { "returnTo", returnTo },
            });
            HttpResponseMessage response = await _client.SendAsync(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
            Assert.That(response.Headers.Location.ToString(), Is.EqualTo(expected));
        }

        [Test]
        public void Map_BadPrefix_Throws()
        {
            WebApplication app = WebApplication.CreateBuilder().Build();
            Assert.Throws<ArgumentException>(() => app.MapAnointedAutomationAuth("auth"));
            Assert.Throws<InvalidOperationException>(() => app.MapAnointedAutomationAuth("/auth", o => o.ReturnToParam = ""));
        }
    }
}
