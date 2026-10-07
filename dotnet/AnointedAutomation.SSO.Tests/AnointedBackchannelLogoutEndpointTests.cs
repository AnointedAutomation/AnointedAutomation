// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using AnointedAutomation.SSO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AnointedAutomation.SSO.Tests
{
    public class AnointedBackchannelLogoutEndpointTests
    {
        private const string Route = "/auth/anointed/backchannel-logout";
        private static readonly DateTimeOffset Now = new DateTimeOffset(2026, 10, 7, 12, 0, 0, TimeSpan.Zero);
        private readonly TestKeys _keys = new TestKeys("k1");
        private readonly MutableTimeProvider _clock = new MutableTimeProvider(Now);
        private readonly FakeConfigurationManager _configuration;
        private readonly List<AnointedLogout> _calls = new List<AnointedLogout>();

        /// <summary>Sets up the key source.</summary>
        public AnointedBackchannelLogoutEndpointTests()
        {
            _configuration = new FakeConfigurationManager(TestKeys.ConfigurationWith(_keys), null);
        }

        /// <summary>When true, the logout handler throws.</summary>
        private bool HandlerFails { get; set; }

        /// <summary>Starts a test server with the endpoint mapped.</summary>
        /// <returns>The running host.</returns>
        private async Task<IHost> StartAsync()
        {
            AnointedLogoutTokenValidator validator = AnointedLogoutTokenValidatorTests.CreateValidator(_configuration, _clock);
            IHost host = new HostBuilder()
                .ConfigureWebHost(web =>
                {
                    web.UseTestServer();
                    web.ConfigureServices(services =>
                    {
                        services.AddRouting();
                        services.AddSingleton<TimeProvider>(_clock);
                        services.AddSingleton(validator);
                        services.AddAuthentication().AddAnointedAutomation(options =>
                        {
                            options.Issuer = TestKeys.Issuer;
                            options.ClientId = TestKeys.ClientId;
                            options.ClientSecret = "secret";
                            options.CallbackPath = "/auth/anointed/callback";
                        });
                    });
                    web.Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints => endpoints.MapAnointedBackchannelLogout(Route, OnLogout));
                    });
                })
                .Build();
            await host.StartAsync();
            return host;
        }

        /// <summary>The app's logout handler under test.</summary>
        /// <param name="logout">The validated logout.</param>
        /// <param name="context">The request context.</param>
        /// <returns>A completed task, or throws when <see cref="HandlerFails"/>.</returns>
        private Task OnLogout(AnointedLogout logout, HttpContext context)
        {
            if (HandlerFails)
            {
                throw new InvalidOperationException("session store down");
            }
            _calls.Add(logout);
            return Task.CompletedTask;
        }

        /// <summary>Posts a logout_token form.</summary>
        /// <param name="host">The host.</param>
        /// <param name="token">The token.</param>
        /// <returns>The response status.</returns>
        private static async Task<HttpStatusCode> PostAsync(IHost host, string token)
        {
            HttpClient client = host.GetTestClient();
            FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string> { { "logout_token", token } });
            HttpResponseMessage response = await client.PostAsync(Route, content);
            return response.StatusCode;
        }

        [Fact]
        public async Task ValidToken_200_HandlerCalledOnce()
        {
            using (IHost host = await StartAsync())
            {
                string token = _keys.SignRs256(_keys.ValidHeader(), TestKeys.ValidPayload(Now));
                Assert.Equal(HttpStatusCode.OK, await PostAsync(host, token));
                Assert.Single(_calls);
                Assert.Equal("user-1", _calls[0].Sub);
            }
        }

        [Fact]
        public async Task ReplayedJti_200_HandlerNotCalledAgain()
        {
            using (IHost host = await StartAsync())
            {
                string token = _keys.SignRs256(_keys.ValidHeader(), TestKeys.ValidPayload(Now));
                Assert.Equal(HttpStatusCode.OK, await PostAsync(host, token));
                Assert.Equal(HttpStatusCode.OK, await PostAsync(host, token));
                Assert.Single(_calls);
            }
        }

        [Fact]
        public async Task InvalidToken_400_HandlerNotCalled()
        {
            using (IHost host = await StartAsync())
            {
                Dictionary<string, object> payload = TestKeys.ValidPayload(Now);
                payload["nonce"] = "n";
                Assert.Equal(HttpStatusCode.BadRequest, await PostAsync(host, _keys.SignRs256(_keys.ValidHeader(), payload)));
                Assert.Empty(_calls);
            }
        }

        [Fact]
        public async Task MissingField_400()
        {
            using (IHost host = await StartAsync())
            {
                HttpClient client = host.GetTestClient();
                FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string> { { "other", "x" } });
                HttpResponseMessage response = await client.PostAsync(Route, content);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Fact]
        public async Task JsonBody_400()
        {
            using (IHost host = await StartAsync())
            {
                HttpClient client = host.GetTestClient();
                StringContent content = new StringContent("{\"logout_token\":\"x\"}", Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(Route, content);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Fact]
        public async Task HandlerThrows_500_ThenRetrySucceeds()
        {
            using (IHost host = await StartAsync())
            {
                string token = _keys.SignRs256(_keys.ValidHeader(), TestKeys.ValidPayload(Now));
                HandlerFails = true;
                Assert.Equal(HttpStatusCode.InternalServerError, await PostAsync(host, token));
                Assert.Empty(_calls);
                HandlerFails = false;
                Assert.Equal(HttpStatusCode.OK, await PostAsync(host, token));
                Assert.Single(_calls);
            }
        }

        [Fact]
        public async Task KeysUnavailable_500()
        {
            using (IHost host = await StartAsync())
            {
                _configuration.Fail = true;
                string token = _keys.SignRs256(_keys.ValidHeader(), TestKeys.ValidPayload(Now));
                Assert.Equal(HttpStatusCode.InternalServerError, await PostAsync(host, token));
                Assert.Empty(_calls);
            }
        }

        [Fact]
        public async Task Get_NotAllowed()
        {
            using (IHost host = await StartAsync())
            {
                HttpResponseMessage response = await host.GetTestClient().GetAsync(Route);
                Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
            }
        }

        [Fact]
        public void Map_WithoutAddAnointedAutomation_Throws()
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder();
            builder.WebHost.UseTestServer();
            WebApplication app = builder.Build();
            Assert.Throws<InvalidOperationException>(() => app.MapAnointedBackchannelLogout(Route, (logout, context) => Task.CompletedTask));
        }
    }
}
