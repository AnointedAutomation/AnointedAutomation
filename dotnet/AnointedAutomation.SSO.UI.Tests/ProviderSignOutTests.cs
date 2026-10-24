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
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace AnointedAutomation.SSO.UI.Tests
{
    /// <summary>Sign-out with an OpenID Connect challenge scheme: RP-initiated logout at the provider.</summary>
    public class ProviderSignOutTests
    {
        private const string EndSession = "https://idp.example/connect/logout";

        private static async Task<IHost> StartAsync(string endSessionEndpoint, bool signOutOfProvider)
        {
            return await new HostBuilder()
                .ConfigureWebHost(web => web
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddRouting();
                        services.AddAntiforgery();
                        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                            .AddCookie()
                            .AddOpenIdConnect(AnointedAutomationDefaults.AuthenticationScheme, o =>
                            {
                                o.ClientId = "client-1";
                                o.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                                o.Configuration = new OpenIdConnectConfiguration
                                {
                                    Issuer = "https://idp.example",
                                    AuthorizationEndpoint = "https://idp.example/connect/authorize",
                                    EndSessionEndpoint = endSessionEndpoint,
                                };
                            });
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseAuthentication();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapAnointedAutomationAuth("/auth/anointed", o => o.SignOutOfProvider = signOutOfProvider);
                            endpoints.MapGet("/test/sign-in", async (HttpContext context) =>
                            {
                                ClaimsIdentity identity = new ClaimsIdentity(new[] { new Claim(AnointedClaims.Subject, "user-1") }, CookieAuthenticationDefaults.AuthenticationScheme);
                                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                            });
                            endpoints.MapGet("/test/token", (HttpContext context, IAntiforgery antiforgery) => antiforgery.GetAndStoreTokens(context).RequestToken);
                        });
                    }))
                .StartAsync();
        }

        private static string Cookies(HttpResponseMessage response)
        {
            return string.Join("; ", response.Headers.GetValues("Set-Cookie").Select(c => c.Split(';')[0]));
        }

        private static async Task<HttpResponseMessage> ScriptSignOutAsync(HttpClient client)
        {
            string cookie = Cookies(await client.GetAsync("/test/sign-in"));
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/auth/anointed/sign-out?returnTo=%2Fbye");
            request.Headers.Add("Cookie", cookie);
            request.Headers.Add(AnointedAuthEndpointOptions.CsrfHeaderName, "1");
            return await client.SendAsync(request);
        }

        [Fact]
        public async Task FormSignOut_RedirectsToProviderEndSession()
        {
            using IHost host = await StartAsync(EndSession, true);
            HttpClient client = host.GetTestClient();
            string session = Cookies(await client.GetAsync("/test/sign-in"));
            HttpRequestMessage tokenRequest = new HttpRequestMessage(HttpMethod.Get, "/test/token");
            tokenRequest.Headers.Add("Cookie", session);
            HttpResponseMessage tokenResponse = await client.SendAsync(tokenRequest);
            string token = await tokenResponse.Content.ReadAsStringAsync();

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/auth/anointed/sign-out");
            request.Headers.Add("Cookie", session + "; " + Cookies(tokenResponse));
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "__RequestVerificationToken", token },
                { "returnTo", "/bye" },
            });
            HttpResponseMessage response = await client.SendAsync(request);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.StartsWith(EndSession + "?", response.Headers.Location.ToString(), StringComparison.Ordinal);
            Assert.Contains("post_logout_redirect_uri=", response.Headers.Location.ToString());
            Assert.Contains("expires=Thu, 01 Jan 1970", string.Join(",", response.Headers.GetValues("Set-Cookie")));
            await host.StopAsync();
        }

        [Fact]
        public async Task ScriptSignOut_ReturnsProviderRedirectAsJson()
        {
            using IHost host = await StartAsync(EndSession, true);
            HttpResponseMessage response = await ScriptSignOutAsync(host.GetTestClient());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Null(response.Headers.Location);
            Assert.True(response.Headers.CacheControl.NoStore);
            Assert.Contains("expires=Thu, 01 Jan 1970", string.Join(",", response.Headers.GetValues("Set-Cookie")));
            JsonElement body = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
            Assert.StartsWith(EndSession + "?", body.GetProperty("redirect").GetString(), StringComparison.Ordinal);
            await host.StopAsync();
        }

        [Fact]
        public async Task NoEndSessionEndpoint_EndsLocalSessionOnly()
        {
            using IHost host = await StartAsync(null, true);
            HttpResponseMessage response = await ScriptSignOutAsync(host.GetTestClient());
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            await host.StopAsync();
        }

        [Fact]
        public async Task SignOutOfProviderOff_EndsLocalSessionOnly()
        {
            using IHost host = await StartAsync(EndSession, false);
            HttpResponseMessage response = await ScriptSignOutAsync(host.GetTestClient());
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            await host.StopAsync();
        }
    }
}
