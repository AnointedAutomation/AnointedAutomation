// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using AnointedAutomation.SSO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AnointedAutomation.SSO.Tests
{
    public class AnointedSessionStatusClientTests
    {
        private const string StatusPath = "/api/session-status";
        private const string TokenPath = "/api/auth/machine-token";
        private readonly MutableTimeProvider _clock = new MutableTimeProvider(new DateTimeOffset(2026, 10, 7, 12, 0, 0, TimeSpan.Zero));
        private readonly AnointedSessionStatusCache _cache = new AnointedSessionStatusCache();
        private readonly FakeHttpHandler _http;
        private int _exchanges;

        /// <summary>Wires the fake API.</summary>
        public AnointedSessionStatusClientTests()
        {
            _http = new FakeHttpHandler(Respond);
            StatusResponder = request => FakeHttpHandler.Json(HttpStatusCode.OK, "{\"success\":true,\"Data\":{\"active\":true,\"Reason\":\"active\"}}");
            ExchangeResponder = () => FakeHttpHandler.Json(HttpStatusCode.OK, TokenBody("tok-" + _exchanges.ToString(CultureInfo.InvariantCulture), _clock.Now.AddHours(1)));
        }

        /// <summary>Answers session-status requests.</summary>
        private Func<HttpRequestMessage, Task<HttpResponseMessage>> StatusResponder { get; set; }

        /// <summary>Answers machine-token requests.</summary>
        private Func<Task<HttpResponseMessage>> ExchangeResponder { get; set; }

        /// <summary>A machine-token response body.</summary>
        /// <param name="token">The token.</param>
        /// <param name="expiresAt">Its expiry.</param>
        /// <returns>The JSON body.</returns>
        private static string TokenBody(string token, DateTimeOffset expiresAt)
        {
            return "{\"Data\":{\"token\":\"" + token + "\",\"expiresAt\":\"" + expiresAt.UtcDateTime.ToString("o", CultureInfo.InvariantCulture) + "\"},\"success\":true}";
        }

        /// <summary>Routes a fake request.</summary>
        /// <param name="request">The request.</param>
        /// <param name="body">The request body.</param>
        /// <returns>The response.</returns>
        private Task<HttpResponseMessage> Respond(HttpRequestMessage request, string body)
        {
            if (request.RequestUri.AbsolutePath.EndsWith(TokenPath, StringComparison.Ordinal))
            {
                _exchanges++;
                return ExchangeResponder();
            }
            return StatusResponder(request);
        }

        /// <summary>A client over the fake API.</summary>
        /// <param name="timeout">The per-request timeout.</param>
        /// <returns>The client.</returns>
        private AnointedSessionStatusClient Create(TimeSpan timeout)
        {
            AnointedSessionStatusOptions options = new AnointedSessionStatusOptions
            {
                ApiKey = "partner-key",
                AppName = "Test App",
                BaseAddress = new Uri("https://api.test/"),
                Timeout = timeout,
            };
            return new AnointedSessionStatusClient(new HttpClient(_http), options, _cache, _clock, NullLogger<AnointedSessionStatusClient>.Instance);
        }

        /// <summary>A client with the default 3 second timeout.</summary>
        /// <returns>The client.</returns>
        private AnointedSessionStatusClient Create()
        {
            return Create(TimeSpan.FromSeconds(3));
        }

        [Fact]
        public async Task Active_SendsBearerAndQuery()
        {
            AnointedSessionCheck check = await Create().CheckAsync("user 1", true, CancellationToken.None);
            Assert.Equal(AnointedSessionOutcome.Active, check.Outcome);
            Assert.Equal("active", check.Reason);
            Assert.False(check.ShouldEndSession);
            HttpRequestMessage status = _http.Requests[1];
            Assert.Equal("https://api.test/api/session-status?userId=user%201&grant=true", status.RequestUri.AbsoluteUri);
            Assert.Equal("Bearer", status.Headers.Authorization.Scheme);
            Assert.Equal("tok-1", status.Headers.Authorization.Parameter);
        }

        [Fact]
        public async Task Exchange_SendsApiKeyAndName()
        {
            await Create().CheckAsync("u1", false, CancellationToken.None);
            Assert.Equal(HttpMethod.Post, _http.Requests[0].Method);
            using (JsonDocument body = JsonDocument.Parse(_http.Bodies[0]))
            {
                Assert.Equal("partner-key", body.RootElement.GetProperty("apiKey").GetString());
                Assert.Equal("Test App", body.RootElement.GetProperty("name").GetString());
            }
            Assert.EndsWith("grant=false", _http.Requests[1].RequestUri.Query, StringComparison.Ordinal);
        }

        [Theory]
        [InlineData("account_gone")]
        [InlineData("account_banned")]
        [InlineData("signin_off")]
        [InlineData("app_disconnected")]
        public async Task Ended_WithReason(string reason)
        {
            StatusResponder = request => FakeHttpHandler.Json(HttpStatusCode.OK, "{\"success\":true,\"Data\":{\"active\":false,\"Reason\":\"" + reason + "\"}}");
            AnointedSessionCheck check = await Create().CheckAsync("u1", true, CancellationToken.None);
            Assert.Equal(AnointedSessionOutcome.Ended, check.Outcome);
            Assert.Equal(reason, check.Reason);
            Assert.True(check.ShouldEndSession);
        }

        [Fact]
        public async Task Parses_CaseInsensitively()
        {
            StatusResponder = request => FakeHttpHandler.Json(HttpStatusCode.OK, "{\"SUCCESS\":true,\"data\":{\"Active\":false,\"reason\":\"signin_off\"}}");
            AnointedSessionCheck check = await Create().CheckAsync("u1", true, CancellationToken.None);
            Assert.Equal(AnointedSessionOutcome.Ended, check.Outcome);
            Assert.Equal("signin_off", check.Reason);
        }

        [Theory]
        [InlineData(HttpStatusCode.InternalServerError, "{}")]
        [InlineData(HttpStatusCode.Forbidden, "{}")]
        [InlineData(HttpStatusCode.NotFound, "{}")]
        [InlineData(HttpStatusCode.OK, "not json")]
        [InlineData(HttpStatusCode.OK, "[]")]
        [InlineData(HttpStatusCode.OK, "{\"success\":true}")]
        [InlineData(HttpStatusCode.OK, "{\"success\":true,\"Data\":{\"Reason\":\"x\"}}")]
        [InlineData(HttpStatusCode.OK, "{\"success\":true,\"Data\":{\"active\":\"false\"}}")]
        [InlineData(HttpStatusCode.OK, "{\"success\":false,\"Data\":{\"active\":false}}")]
        public async Task FailOpen_BadStatusOrBody_Unknown(HttpStatusCode status, string body)
        {
            StatusResponder = request => FakeHttpHandler.Json(status, body);
            AnointedSessionCheck check = await Create().CheckAsync("u1", true, CancellationToken.None);
            Assert.Equal(AnointedSessionOutcome.Unknown, check.Outcome);
            Assert.False(check.ShouldEndSession);
            Assert.NotNull(check.Error);
            Assert.Null(check.Reason);
        }

        [Fact]
        public async Task FailOpen_NetworkError_Unknown()
        {
            StatusResponder = request => Task.FromException<HttpResponseMessage>(new HttpRequestException("down"));
            AnointedSessionCheck check = await Create().CheckAsync("u1", true, CancellationToken.None);
            Assert.Equal(AnointedSessionOutcome.Unknown, check.Outcome);
        }

        [Fact]
        public async Task FailOpen_Timeout_Unknown()
        {
            StatusResponder = request => new TaskCompletionSource<HttpResponseMessage>().Task;
            AnointedSessionCheck check = await Create(TimeSpan.FromMilliseconds(150)).CheckAsync("u1", true, CancellationToken.None);
            Assert.Equal(AnointedSessionOutcome.Unknown, check.Outcome);
        }

        [Theory]
        [InlineData(HttpStatusCode.InternalServerError, "{}")]
        [InlineData(HttpStatusCode.Unauthorized, "{}")]
        [InlineData(HttpStatusCode.OK, "nope")]
        [InlineData(HttpStatusCode.OK, "{\"Data\":{\"expiresAt\":\"2026-10-07T13:00:00Z\"}}")]
        [InlineData(HttpStatusCode.OK, "{\"Data\":{\"token\":\"t\"}}")]
        [InlineData(HttpStatusCode.OK, "{\"Data\":{\"token\":\"t\",\"expiresAt\":\"soon\"}}")]
        public async Task FailOpen_TokenExchangeFails_Unknown(HttpStatusCode status, string body)
        {
            ExchangeResponder = () => FakeHttpHandler.Json(status, body);
            AnointedSessionCheck check = await Create().CheckAsync("u1", true, CancellationToken.None);
            Assert.Equal(AnointedSessionOutcome.Unknown, check.Outcome);
            Assert.Equal(0, _http.Count(StatusPath));
        }

        [Fact]
        public async Task FailOpen_TokenExchangeNetworkError_Unknown()
        {
            ExchangeResponder = () => Task.FromException<HttpResponseMessage>(new HttpRequestException("down"));
            AnointedSessionCheck check = await Create().CheckAsync("u1", true, CancellationToken.None);
            Assert.Equal(AnointedSessionOutcome.Unknown, check.Outcome);
        }

        [Fact]
        public async Task Unauthorized_ReExchangesOnce_ThenSucceeds()
        {
            int statusCalls = 0;
            StatusResponder = request =>
            {
                statusCalls++;
                if (statusCalls == 1)
                {
                    return FakeHttpHandler.Json(HttpStatusCode.Unauthorized, "{}");
                }
                return FakeHttpHandler.Json(HttpStatusCode.OK, "{\"success\":true,\"Data\":{\"active\":false,\"Reason\":\"app_disconnected\"}}");
            };
            AnointedSessionCheck check = await Create().CheckAsync("u1", true, CancellationToken.None);
            Assert.Equal(AnointedSessionOutcome.Ended, check.Outcome);
            Assert.Equal(2, _exchanges);
            Assert.Equal(2, _http.Count(StatusPath));
            Assert.Equal("tok-2", _http.Requests[3].Headers.Authorization.Parameter);
        }

        [Fact]
        public async Task Unauthorized_Twice_Unknown_NoThirdTry()
        {
            StatusResponder = request => FakeHttpHandler.Json(HttpStatusCode.Unauthorized, "{}");
            AnointedSessionCheck check = await Create().CheckAsync("u1", true, CancellationToken.None);
            Assert.Equal(AnointedSessionOutcome.Unknown, check.Outcome);
            Assert.Equal(2, _exchanges);
            Assert.Equal(2, _http.Count(StatusPath));
        }

        [Fact]
        public async Task MachineToken_IsCachedAcrossUsers()
        {
            AnointedSessionStatusClient client = Create();
            await client.CheckAsync("u1", true, CancellationToken.None);
            await client.CheckAsync("u2", true, CancellationToken.None);
            await Create().CheckAsync("u3", true, CancellationToken.None);
            Assert.Equal(1, _exchanges);
            Assert.Equal(3, _http.Count(StatusPath));
        }

        [Fact]
        public async Task MachineToken_ReExchangedNearExpiry()
        {
            AnointedSessionStatusClient client = Create();
            await client.CheckAsync("u1", true, CancellationToken.None);
            _clock.Advance(TimeSpan.FromMinutes(59).Add(TimeSpan.FromSeconds(1)));
            await client.CheckAsync("u2", true, CancellationToken.None);
            Assert.Equal(2, _exchanges);
        }

        [Fact]
        public async Task Answers_CachedFiveMinutesPerUserAndGrant()
        {
            AnointedSessionStatusClient client = Create();
            await client.CheckAsync("u1", true, CancellationToken.None);
            await client.CheckAsync("u1", true, CancellationToken.None);
            Assert.Equal(1, _http.Count(StatusPath));
            await client.CheckAsync("u1", false, CancellationToken.None);
            Assert.Equal(2, _http.Count(StatusPath));
            _clock.Advance(TimeSpan.FromMinutes(4));
            await client.CheckAsync("u1", true, CancellationToken.None);
            Assert.Equal(2, _http.Count(StatusPath));
            _clock.Advance(TimeSpan.FromMinutes(1).Add(TimeSpan.FromSeconds(1)));
            await client.CheckAsync("u1", true, CancellationToken.None);
            Assert.Equal(3, _http.Count(StatusPath));
        }

        [Fact]
        public async Task EndedAnswer_IsCached()
        {
            StatusResponder = request => FakeHttpHandler.Json(HttpStatusCode.OK, "{\"success\":true,\"Data\":{\"active\":false,\"Reason\":\"account_banned\"}}");
            AnointedSessionStatusClient client = Create();
            await client.CheckAsync("u1", true, CancellationToken.None);
            AnointedSessionCheck second = await client.CheckAsync("u1", true, CancellationToken.None);
            Assert.True(second.ShouldEndSession);
            Assert.Equal(1, _http.Count(StatusPath));
        }

        [Fact]
        public async Task Failures_AreNotCached()
        {
            int statusCalls = 0;
            StatusResponder = request =>
            {
                statusCalls++;
                if (statusCalls == 1)
                {
                    return FakeHttpHandler.Json(HttpStatusCode.ServiceUnavailable, "{}");
                }
                return FakeHttpHandler.Json(HttpStatusCode.OK, "{\"success\":true,\"Data\":{\"active\":true,\"Reason\":\"active\"}}");
            };
            AnointedSessionStatusClient client = Create();
            AnointedSessionCheck first = await client.CheckAsync("u1", true, CancellationToken.None);
            AnointedSessionCheck second = await client.CheckAsync("u1", true, CancellationToken.None);
            Assert.Equal(AnointedSessionOutcome.Unknown, first.Outcome);
            Assert.Equal(AnointedSessionOutcome.Active, second.Outcome);
            Assert.Equal(2, _http.Count(StatusPath));
        }

        [Fact]
        public async Task CallerCancellation_Throws()
        {
            using (CancellationTokenSource cancelled = new CancellationTokenSource())
            {
                cancelled.Cancel();
                await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Create().CheckAsync("u1", true, cancelled.Token));
            }
        }

        [Fact]
        public async Task EmptyUserId_Throws()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => Create().CheckAsync(" ", true, CancellationToken.None));
        }

        [Theory]
        [InlineData(null, "App")]
        [InlineData("", "App")]
        [InlineData("key", null)]
        [InlineData("key", " ")]
        public void AddAnointedSessionStatus_MissingRequired_Throws(string apiKey, string appName)
        {
            ServiceCollection services = new ServiceCollection();
            Assert.Throws<InvalidOperationException>(() => services.AddAnointedSessionStatus(options =>
            {
                options.ApiKey = apiKey;
                options.AppName = appName;
            }));
        }

        [Fact]
        public void Options_HttpBaseAddress_Throws()
        {
            AnointedSessionStatusOptions options = new AnointedSessionStatusOptions { ApiKey = "k", AppName = "a", BaseAddress = new Uri("http://api.test/") };
            Assert.Throws<InvalidOperationException>(() => options.Validate());
        }

        [Fact]
        public async Task AddAnointedSessionStatus_ResolvesTypedClient()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<TimeProvider>(_clock);
            services.AddAnointedSessionStatus(options =>
            {
                options.ApiKey = "partner-key";
                options.AppName = "Test App";
                options.BaseAddress = new Uri("https://api.test/");
            }).ConfigurePrimaryHttpMessageHandler(() => _http);
            ServiceProvider provider = services.BuildServiceProvider();
            IAnointedSessionStatus status = provider.GetRequiredService<IAnointedSessionStatus>();
            Assert.IsType<AnointedSessionStatusClient>(status);
            AnointedSessionCheck check = await status.CheckAsync("u1", true, CancellationToken.None);
            Assert.Equal(AnointedSessionOutcome.Active, check.Outcome);
            await provider.GetRequiredService<IAnointedSessionStatus>().CheckAsync("u2", true, CancellationToken.None);
            Assert.Equal(1, _exchanges);
        }
    }

    public class AnointedSessionStatusCookieEventsTests
    {
        /// <summary>A session status stub that returns one fixed answer.</summary>
        private sealed class StubStatus : IAnointedSessionStatus
        {
            private readonly AnointedSessionCheck _answer;

            /// <summary>Creates the stub.</summary>
            /// <param name="answer">The answer to give.</param>
            public StubStatus(AnointedSessionCheck answer)
            {
                _answer = answer;
            }

            /// <summary>Every (userId, grant) asked.</summary>
            public List<string> Calls { get; } = new List<string>();

            /// <summary>Records the call and answers.</summary>
            /// <param name="userId">The user id.</param>
            /// <param name="grant">The grant flag.</param>
            /// <param name="cancellationToken">Unused.</param>
            /// <returns>The fixed answer.</returns>
            public Task<AnointedSessionCheck> CheckAsync(string userId, bool grant, CancellationToken cancellationToken)
            {
                Calls.Add(userId + "|" + grant.ToString(CultureInfo.InvariantCulture));
                return Task.FromResult(_answer);
            }
        }

        /// <summary>An authentication service that records sign-outs.</summary>
        private sealed class RecordingAuthenticationService : IAuthenticationService
        {
            /// <summary>Schemes signed out.</summary>
            public List<string> SignedOut { get; } = new List<string>();

            /// <summary>Not used.</summary>
            /// <param name="context">The context.</param>
            /// <param name="scheme">The scheme.</param>
            /// <returns>No result.</returns>
            public Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string scheme)
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            /// <summary>Not used.</summary>
            /// <param name="context">The context.</param>
            /// <param name="scheme">The scheme.</param>
            /// <param name="properties">The properties.</param>
            /// <returns>A completed task.</returns>
            public Task ChallengeAsync(HttpContext context, string scheme, AuthenticationProperties properties)
            {
                return Task.CompletedTask;
            }

            /// <summary>Not used.</summary>
            /// <param name="context">The context.</param>
            /// <param name="scheme">The scheme.</param>
            /// <param name="properties">The properties.</param>
            /// <returns>A completed task.</returns>
            public Task ForbidAsync(HttpContext context, string scheme, AuthenticationProperties properties)
            {
                return Task.CompletedTask;
            }

            /// <summary>Not used.</summary>
            /// <param name="context">The context.</param>
            /// <param name="scheme">The scheme.</param>
            /// <param name="principal">The principal.</param>
            /// <param name="properties">The properties.</param>
            /// <returns>A completed task.</returns>
            public Task SignInAsync(HttpContext context, string scheme, ClaimsPrincipal principal, AuthenticationProperties properties)
            {
                return Task.CompletedTask;
            }

            /// <summary>Records the sign-out.</summary>
            /// <param name="context">The context.</param>
            /// <param name="scheme">The scheme.</param>
            /// <param name="properties">The properties.</param>
            /// <returns>A completed task.</returns>
            public Task SignOutAsync(HttpContext context, string scheme, AuthenticationProperties properties)
            {
                SignedOut.Add(scheme);
                return Task.CompletedTask;
            }
        }

        /// <summary>Runs the hook against a principal with the given claims.</summary>
        /// <param name="answer">What the session status says.</param>
        /// <param name="claims">The cookie principal's claims.</param>
        /// <returns>The context, the stub and the auth service.</returns>
        private static async Task<Tuple<CookieValidatePrincipalContext, StubStatus, RecordingAuthenticationService>> RunAsync(AnointedSessionCheck answer, params Claim[] claims)
        {
            StubStatus status = new StubStatus(answer);
            RecordingAuthenticationService auth = new RecordingAuthenticationService();
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<IAnointedSessionStatus>(status);
            services.AddSingleton<IAuthenticationService>(auth);
            DefaultHttpContext http = new DefaultHttpContext { RequestServices = services.BuildServiceProvider() };
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "cookie"));
            AuthenticationScheme scheme = new AuthenticationScheme(CookieAuthenticationDefaults.AuthenticationScheme, null, typeof(CookieAuthenticationHandler));
            AuthenticationTicket ticket = new AuthenticationTicket(principal, CookieAuthenticationDefaults.AuthenticationScheme);
            CookieValidatePrincipalContext context = new CookieValidatePrincipalContext(http, scheme, new CookieAuthenticationOptions(), ticket);
            await AnointedSessionStatusCookieEvents.ValidatePrincipalAsync(context, true);
            return Tuple.Create(context, status, auth);
        }

        [Fact]
        public async Task Ended_RejectsAndSignsOut()
        {
            Tuple<CookieValidatePrincipalContext, StubStatus, RecordingAuthenticationService> run = await RunAsync(AnointedSessionCheck.Ended("account_banned"), new Claim("sub", "u1"));
            Assert.Null(run.Item1.Principal);
            Assert.Equal(new[] { "u1|True" }, run.Item2.Calls.ToArray());
            Assert.Equal(new[] { CookieAuthenticationDefaults.AuthenticationScheme }, run.Item3.SignedOut.ToArray());
        }

        [Fact]
        public async Task Active_Keeps()
        {
            Tuple<CookieValidatePrincipalContext, StubStatus, RecordingAuthenticationService> run = await RunAsync(AnointedSessionCheck.Active("active"), new Claim("sub", "u1"));
            Assert.NotNull(run.Item1.Principal);
            Assert.Empty(run.Item3.SignedOut);
        }

        [Fact]
        public async Task Unknown_KeepsFailOpen()
        {
            Tuple<CookieValidatePrincipalContext, StubStatus, RecordingAuthenticationService> run = await RunAsync(AnointedSessionCheck.Unknown("timeout"), new Claim("sub", "u1"));
            Assert.NotNull(run.Item1.Principal);
            Assert.Empty(run.Item3.SignedOut);
        }

        [Fact]
        public async Task NoSub_NotChecked()
        {
            Tuple<CookieValidatePrincipalContext, StubStatus, RecordingAuthenticationService> run = await RunAsync(AnointedSessionCheck.Ended("account_gone"), new Claim("email", "a@b.c"));
            Assert.NotNull(run.Item1.Principal);
            Assert.Empty(run.Item2.Calls);
        }
    }
}
