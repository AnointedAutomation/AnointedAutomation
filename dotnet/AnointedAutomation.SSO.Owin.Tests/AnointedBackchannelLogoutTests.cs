// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using AnointedAutomation.SSO.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AnointedAutomation.SSO.Owin.Tests
{
    public class AnointedBackchannelLogoutTests
    {
        private const string Path = "/auth/anointed/backchannel-logout";
        private static readonly DateTimeOffset Now = new DateTimeOffset(2026, 10, 7, 12, 0, 0, TimeSpan.Zero);
        private readonly TestKeys _keys = new TestKeys("k1");
        private readonly MutableTimeProvider _clock = new MutableTimeProvider(Now);
        private readonly FakeConfigurationManager _configuration;
        private readonly List<AnointedLogout> _calls = new List<AnointedLogout>();
        private readonly AnointedBackchannelLogoutMiddleware _middleware;
        private bool _nextCalled;

        /// <summary>Builds the middleware over the test key.</summary>
        public AnointedBackchannelLogoutTests()
        {
            _configuration = new FakeConfigurationManager(TestKeys.ConfigurationWith(_keys), null);
            AnointedLogoutTokenValidator validator = AnointedLogoutTokenValidatorTests.CreateValidator(_configuration, _clock);
            AnointedBackchannelLogoutHandler handler = new AnointedBackchannelLogoutHandler(validator, new InMemoryAnointedLogoutReplayStore(_clock), new TestLogger());
            _middleware = new AnointedBackchannelLogoutMiddleware(new NextMiddleware(this), new PathString(Path), handler, OnLogout);
        }

        /// <summary>When true, the logout handler throws.</summary>
        private bool HandlerFails { get; set; }

        /// <summary>Records that the request passed through.</summary>
        private sealed class NextMiddleware : OwinMiddleware
        {
            private readonly AnointedBackchannelLogoutTests _owner;

            /// <summary>Creates the terminal middleware.</summary>
            /// <param name="owner">The test.</param>
            public NextMiddleware(AnointedBackchannelLogoutTests owner) : base(null)
            {
                _owner = owner;
            }

            /// <summary>Marks the pass-through.</summary>
            /// <param name="context">The context.</param>
            /// <returns>A completed task.</returns>
            public override Task Invoke(IOwinContext context)
            {
                _owner._nextCalled = true;
                context.Response.StatusCode = 404;
                return Task.CompletedTask;
            }
        }

        /// <summary>The app's logout handler under test.</summary>
        /// <param name="logout">The validated logout.</param>
        /// <param name="context">The request context.</param>
        /// <returns>A completed task, or throws when <see cref="HandlerFails"/>.</returns>
        private Task OnLogout(AnointedLogout logout, IOwinContext context)
        {
            if (HandlerFails)
            {
                throw new InvalidOperationException("session store down");
            }
            _calls.Add(logout);
            return Task.CompletedTask;
        }

        /// <summary>Sends one request through the middleware.</summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="path">The path.</param>
        /// <param name="contentType">The content type, or null.</param>
        /// <param name="body">The body.</param>
        /// <returns>The finished context.</returns>
        private async Task<IOwinContext> SendAsync(string method, string path, string contentType, string body)
        {
            OwinContext context = new OwinContext();
            context.Request.Method = method;
            context.Request.Path = new PathString(path);
            if (contentType != null)
            {
                context.Request.ContentType = contentType;
            }
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
            context.Response.Body = new MemoryStream();
            await _middleware.Invoke(context);
            return context;
        }

        /// <summary>Posts a logout_token form.</summary>
        /// <param name="token">The token.</param>
        /// <returns>The response status.</returns>
        private async Task<int> PostAsync(string token)
        {
            IOwinContext context = await SendAsync("POST", Path, "application/x-www-form-urlencoded", "logout_token=" + Uri.EscapeDataString(token));
            Assert.Equal("no-store", context.Response.Headers.Get("Cache-Control"));
            return context.Response.StatusCode;
        }

        /// <summary>A valid signed token.</summary>
        /// <returns>The token.</returns>
        private string ValidToken()
        {
            return _keys.SignRs256(_keys.ValidHeader(), TestKeys.ValidPayload(Now));
        }

        [Fact]
        public async Task Valid_200_CallsHandlerOnce()
        {
            Assert.Equal(200, await PostAsync(ValidToken()));
            Assert.Single(_calls);
            Assert.Equal("user-1", _calls[0].Sub);
            Assert.Equal("grant-1", _calls[0].Sid);
        }

        [Fact]
        public async Task Replay_200_HandlerNotCalledAgain()
        {
            string token = ValidToken();
            Assert.Equal(200, await PostAsync(token));
            Assert.Equal(200, await PostAsync(token));
            Assert.Single(_calls);
        }

        [Fact]
        public async Task Invalid_400_HandlerNotCalled()
        {
            Assert.Equal(400, await PostAsync("not-a-jwt"));
            Assert.Empty(_calls);
        }

        [Fact]
        public async Task MissingToken_400()
        {
            IOwinContext context = await SendAsync("POST", Path, "application/x-www-form-urlencoded", "other=1");
            Assert.Equal(400, context.Response.StatusCode);
        }

        [Fact]
        public async Task RepeatedToken_400()
        {
            string token = Uri.EscapeDataString(ValidToken());
            IOwinContext context = await SendAsync("POST", Path, "application/x-www-form-urlencoded", "logout_token=" + token + "&logout_token=" + token);
            Assert.Equal(400, context.Response.StatusCode);
            Assert.Empty(_calls);
        }

        [Fact]
        public async Task NotAForm_400()
        {
            IOwinContext context = await SendAsync("POST", Path, "application/json", "{\"logout_token\":\"x\"}");
            Assert.Equal(400, context.Response.StatusCode);
        }

        [Fact]
        public async Task HandlerThrows_500_RetryRunsAgain()
        {
            string token = ValidToken();
            HandlerFails = true;
            Assert.Equal(500, await PostAsync(token));
            HandlerFails = false;
            Assert.Equal(200, await PostAsync(token));
            Assert.Single(_calls);
        }

        [Fact]
        public async Task KeysUnavailable_500()
        {
            _configuration.Fail = true;
            Assert.Equal(500, await PostAsync(ValidToken()));
            Assert.Empty(_calls);
        }

        [Fact]
        public async Task Get_405()
        {
            IOwinContext context = await SendAsync("GET", Path, null, string.Empty);
            Assert.Equal(405, context.Response.StatusCode);
            Assert.Equal("POST", context.Response.Headers.Get("Allow"));
            Assert.False(_nextCalled);
        }

        [Fact]
        public async Task OtherPath_PassesThrough()
        {
            IOwinContext context = await SendAsync("POST", "/elsewhere", "application/x-www-form-urlencoded", "logout_token=x");
            Assert.True(_nextCalled);
            Assert.Equal(404, context.Response.StatusCode);
        }
    }
}
