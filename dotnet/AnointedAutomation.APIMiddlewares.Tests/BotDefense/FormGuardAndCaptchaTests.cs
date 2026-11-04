// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.APIMiddleware;
using AnointedAutomation.APIMiddleware.BotDefense;
using AnointedAutomation.APIMiddlewares.Tests.Security;
using AnointedAutomation.Optimization.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AnointedAutomation.APIMiddlewares.Tests.BotDefense
{
    [Collection("Sequential")]
    public class FormGuardAndCaptchaTests : IDisposable
    {
        private readonly SecurityTestLogs.Window _logs = new SecurityTestLogs.Window();

        private const string Key = "0123456789abcdef0123456789abcdef";
        private DateTimeOffset _now = new DateTimeOffset(2026, 11, 2, 12, 0, 0, TimeSpan.Zero);

        public FormGuardAndCaptchaTests()
        {
        }

        public void Dispose()
        {
        }

        private FormBotGuard Guard(int minSeconds = 3, bool enabled = true)
        {
            return new FormBotGuard(new FormBotGuardOptions { Enabled = enabled, MinSubmitSeconds = minSeconds, SigningKey = Key }, () => _now);
        }

        private static DefaultHttpContext Http()
        {
            DefaultHttpContext http = new DefaultHttpContext();
            http.Connection.RemoteIpAddress = IPAddress.Parse("198.51.100.4");
            http.Request.Path = "/contact";
            http.Request.Method = "POST";
            return http;
        }

        [Fact]
        public void Disabled_AlwaysOkAndSilent()
        {
            Assert.Equal(FormBotCheckResult.Ok, Guard(3, false).Check(Http(), "bot filled", null));
            Assert.Empty(_logs.Get());
            Assert.False(new FormBotGuardOptions().Enabled);
        }

        [Fact]
        public void FilledHoneypot_IsBotAndLogsWarning()
        {
            Assert.Equal(FormBotCheckResult.HoneypotFilled, Guard().Check(Http(), "http://spam.example", null));
            LogMessage log = _logs.Get().Single();
            Assert.Equal(MessageType.Warning, log.messageType);
            Assert.Contains("198.51.100.4", log.message);
            Assert.Contains("/contact", log.message);
            Assert.Contains("honeypot", log.message);
        }

        [Fact]
        public void EmptyHoneypotAndSlowEnough_IsOk()
        {
            FormBotGuard guard = Guard();
            string token = guard.CreateTimingToken();
            _now = _now.AddSeconds(10);
            Assert.Equal(FormBotCheckResult.Ok, guard.Check(Http(), "", token));
            Assert.Equal(FormBotCheckResult.Ok, guard.Check(Http(), null, token));
            Assert.Empty(_logs.Get());
        }

        [Fact]
        public void TooFast_IsRejected()
        {
            FormBotGuard guard = Guard();
            string token = guard.CreateTimingToken();
            _now = _now.AddSeconds(1);
            Assert.Equal(FormBotCheckResult.TooFast, guard.Check(Http(), "", token));
            Assert.Contains("faster", _logs.Get().Single().message);
        }

        [Fact]
        public void TimingOff_IgnoresTokenEntirely()
        {
            Assert.Equal(FormBotCheckResult.Ok, Guard(0).Check(Http(), "", null));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("garbage")]
        [InlineData("123.ABCD")]
        [InlineData("notanumber.ABCD")]
        public void MissingOrMalformedToken_IsInvalid(string token)
        {
            Assert.Equal(FormBotCheckResult.InvalidTiming, Guard().Check(Http(), "", token));
        }

        [Fact]
        public void ForgedAndWrongKeyAndExpiredTokens_AreInvalid()
        {
            FormBotGuard guard = Guard();
            string token = guard.CreateTimingToken();
            string forged = (_now.ToUnixTimeSeconds() - 100) + token.Substring(token.IndexOf('.'));
            _now = _now.AddSeconds(10);
            Assert.Equal(FormBotCheckResult.InvalidTiming, guard.Check(Http(), "", forged));

            FormBotGuard otherKey = new FormBotGuard(new FormBotGuardOptions { Enabled = true, MinSubmitSeconds = 3, SigningKey = "ffffffffffffffffffffffffffffffff" }, () => _now);
            Assert.Equal(FormBotCheckResult.InvalidTiming, otherKey.Check(Http(), "", token));

            _now = _now.AddHours(3);
            Assert.Equal(FormBotCheckResult.InvalidTiming, guard.Check(Http(), "", token));
        }

        [Fact]
        public void TokenFromTheFuture_IsInvalid()
        {
            FormBotGuard guard = Guard();
            string token = guard.CreateTimingToken();
            _now = _now.AddSeconds(-60);
            Assert.Equal(FormBotCheckResult.InvalidTiming, guard.Check(Http(), "", token));
        }

        [Fact]
        public void Validate_RequiresKeyForTimingAndSaneAges()
        {
            Assert.Throws<InvalidOperationException>(() => new FormBotGuard(new FormBotGuardOptions { Enabled = true, MinSubmitSeconds = 3 }));
            Assert.Throws<InvalidOperationException>(() => new FormBotGuard(new FormBotGuardOptions { Enabled = true, MinSubmitSeconds = 3, SigningKey = "short" }));
            Assert.Throws<InvalidOperationException>(() => new FormBotGuard(new FormBotGuardOptions { Enabled = true, HoneypotFieldName = " " }));
            Assert.Throws<InvalidOperationException>(() => new FormBotGuard(new FormBotGuardOptions { Enabled = true, MinSubmitSeconds = -1 }));
            Assert.Throws<InvalidOperationException>(() => new FormBotGuard(new FormBotGuardOptions { Enabled = true, MinSubmitSeconds = 30, SigningKey = Key, TokenMaxAge = TimeSpan.FromSeconds(10) }));
            Assert.Equal("website", Guard().HoneypotFieldName);
        }

        [Fact]
        public void Extensions_RegisterGuard()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddFormBotGuard(options => { options.Enabled = true; });
            Assert.NotNull(services.BuildServiceProvider().GetRequiredService<FormBotGuard>());
        }

        private sealed class StubHandler : HttpMessageHandler
        {
            public string Body = "{}";
            public HttpStatusCode Status = HttpStatusCode.OK;
            public Exception Throw;
            public string LastRequest;

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request.RequestUri + " " + await request.Content.ReadAsStringAsync(cancellationToken);
                if (Throw != null)
                {
                    throw Throw;
                }
                return new HttpResponseMessage(Status) { Content = new StringContent(Body) };
            }
        }

        private static CaptchaVerifier Verifier(CaptchaProvider provider, StubHandler handler, double minScore = 0.5)
        {
            return new CaptchaVerifier(new HttpClient(handler), new CaptchaOptions { Provider = provider, SecretKey = "secret-value", MinScore = minScore });
        }

        [Fact]
        public async Task ReCaptcha_PassesAtOrAboveThresholdAndSendsExpectedForm()
        {
            StubHandler handler = new StubHandler { Body = "{\"success\":true,\"score\":0.9}" };
            CaptchaResult result = await Verifier(CaptchaProvider.ReCaptchaV3, handler).VerifyAsync("tok", "198.51.100.4", CancellationToken.None);
            Assert.True(result.Success);
            Assert.Equal(0.9, result.Score);
            Assert.StartsWith("https://www.google.com/recaptcha/api/siteverify", handler.LastRequest, StringComparison.Ordinal);
            Assert.Contains("secret=secret-value", handler.LastRequest);
            Assert.Contains("response=tok", handler.LastRequest);
            Assert.Contains("remoteip=198.51.100.4", handler.LastRequest);
        }

        [Fact]
        public async Task ReCaptcha_LowScoreFails()
        {
            StubHandler handler = new StubHandler { Body = "{\"success\":true,\"score\":0.3}" };
            CaptchaResult result = await Verifier(CaptchaProvider.ReCaptchaV3, handler).VerifyAsync("tok", null, CancellationToken.None);
            Assert.False(result.Success);
            Assert.Contains("below", result.Reason);
            Assert.DoesNotContain("remoteip", handler.LastRequest);
        }

        [Fact]
        public async Task ReCaptcha_ThresholdIsInclusive()
        {
            StubHandler handler = new StubHandler { Body = "{\"success\":true,\"score\":0.5}" };
            Assert.True((await Verifier(CaptchaProvider.ReCaptchaV3, handler, 0.5).VerifyAsync("tok", null, CancellationToken.None)).Success);
        }

        [Fact]
        public async Task ReCaptcha_MissingScoreFailsClosedAndLogsError()
        {
            StubHandler handler = new StubHandler { Body = "{\"success\":true}" };
            CaptchaResult result = await Verifier(CaptchaProvider.ReCaptchaV3, handler).VerifyAsync("tok", null, CancellationToken.None);
            Assert.False(result.Success);
            Assert.Equal(MessageType.Error, _logs.Get().Single().messageType);
        }

        [Fact]
        public async Task Turnstile_SuccessWithoutScorePasses_AndRejectionShowsErrorCodes()
        {
            StubHandler ok = new StubHandler { Body = "{\"success\":true}" };
            Assert.True((await Verifier(CaptchaProvider.Turnstile, ok).VerifyAsync("tok", null, CancellationToken.None)).Success);
            Assert.StartsWith("https://challenges.cloudflare.com/turnstile/v0/siteverify", ok.LastRequest, StringComparison.Ordinal);

            StubHandler bad = new StubHandler { Body = "{\"success\":false,\"error-codes\":[\"invalid-input-response\",\"timeout-or-duplicate\"]}" };
            CaptchaResult result = await Verifier(CaptchaProvider.Turnstile, bad).VerifyAsync("tok", null, CancellationToken.None);
            Assert.False(result.Success);
            Assert.Contains("invalid-input-response,timeout-or-duplicate", result.Reason);
        }

        [Fact]
        public async Task HCaptcha_UsesItsUrl_ScoreAppliedWhenPresent()
        {
            StubHandler handler = new StubHandler { Body = "{\"success\":true,\"score\":0.1}" };
            CaptchaResult result = await Verifier(CaptchaProvider.HCaptcha, handler).VerifyAsync("tok", null, CancellationToken.None);
            Assert.False(result.Success);
            Assert.StartsWith("https://api.hcaptcha.com/siteverify", handler.LastRequest, StringComparison.Ordinal);
            StubHandler noScore = new StubHandler { Body = "{\"success\":true}" };
            Assert.True((await Verifier(CaptchaProvider.HCaptcha, noScore).VerifyAsync("tok", null, CancellationToken.None)).Success);
        }

        [Fact]
        public async Task EmptyToken_FailsWithoutCallingProvider()
        {
            StubHandler handler = new StubHandler();
            CaptchaResult result = await Verifier(CaptchaProvider.Turnstile, handler).VerifyAsync("  ", null, CancellationToken.None);
            Assert.False(result.Success);
            Assert.Null(handler.LastRequest);
        }

        [Fact]
        public async Task ProviderFailures_FailClosedAndLogErrorWithException()
        {
            StubHandler down = new StubHandler { Throw = new HttpRequestException("network down") };
            Assert.False((await Verifier(CaptchaProvider.Turnstile, down).VerifyAsync("tok", null, CancellationToken.None)).Success);
            StubHandler http500 = new StubHandler { Status = HttpStatusCode.InternalServerError };
            Assert.False((await Verifier(CaptchaProvider.Turnstile, http500).VerifyAsync("tok", null, CancellationToken.None)).Success);
            StubHandler junk = new StubHandler { Body = "<html>" };
            Assert.False((await Verifier(CaptchaProvider.Turnstile, junk).VerifyAsync("tok", null, CancellationToken.None)).Success);
            List<LogMessage> logs = _logs.Get().ToList();
            Assert.Equal(3, logs.Count);
            Assert.All(logs, l => Assert.Equal(MessageType.Error, l.messageType));
            Assert.Contains("HttpRequestException", logs[0].exceptionType);
        }

        [Fact]
        public async Task Cancellation_Rethrows()
        {
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                cts.Cancel();
                StubHandler handler = new StubHandler { Throw = new OperationCanceledException(cts.Token) };
                await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Verifier(CaptchaProvider.Turnstile, handler).VerifyAsync("tok", null, cts.Token));
            }
            Assert.Empty(_logs.Get());
        }

        [Fact]
        public void Options_RequireProviderAndSecretAndAllowUrlOverride()
        {
            Assert.Throws<InvalidOperationException>(() => new CaptchaOptions { SecretKey = "x" }.Validate());
            Assert.Throws<InvalidOperationException>(() => new CaptchaOptions { Provider = CaptchaProvider.Turnstile }.Validate());
            Assert.Throws<InvalidOperationException>(() => new CaptchaOptions { Provider = CaptchaProvider.Turnstile, SecretKey = "x", MinScore = 2 }.Validate());
            Assert.Equal("https://proxy.example/verify", new CaptchaOptions { Provider = CaptchaProvider.Turnstile, SecretKey = "x", VerifyUrl = "https://proxy.example/verify" }.ResolveVerifyUrl());
        }

        [Fact]
        public void Extensions_RegisterVerifier()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddCaptchaVerifier(options => { options.Provider = CaptchaProvider.Turnstile; options.SecretKey = "x"; });
            Assert.IsType<CaptchaVerifier>(services.BuildServiceProvider().GetRequiredService<ICaptchaVerifier>());
        }

        [Fact]
        public void RateLimiter_IsHardBoundedAndSweepsAtMostOncePerInterval()
        {
            DateTimeOffset now = _now;
            FixedWindowRateLimiter limiter = new FixedWindowRateLimiter(() => now);
            TimeSpan retry;
            for (int i = 0; i < FixedWindowRateLimiter.MaxKeys + 2000; i++)
            {
                Assert.True(limiter.TryAcquire("k" + i, 1, TimeSpan.FromHours(1), out retry));
            }
            Assert.True(limiter.Count <= FixedWindowRateLimiter.MaxKeys);
            // Keys that were admitted before the cap are still counted.
            Assert.False(limiter.TryAcquire("k0", 1, TimeSpan.FromHours(1), out retry));
            Assert.True(retry > TimeSpan.Zero);
            // A full table of live windows logs once, not per request.
            int fullWarnings = _logs.Get().Count(l => l.message.Contains("rate limiter table is full"));
            Assert.Equal(1, fullWarnings);
        }

        [Fact]
        public void RateLimiter_ReclaimsExpiredWindowsWhenFull()
        {
            DateTimeOffset now = _now;
            FixedWindowRateLimiter limiter = new FixedWindowRateLimiter(() => now);
            TimeSpan retry;
            for (int i = 0; i < FixedWindowRateLimiter.MaxKeys; i++)
            {
                limiter.TryAcquire("k" + i, 1, TimeSpan.FromSeconds(1), out retry);
            }
            now = now.AddMinutes(5);
            Assert.True(limiter.TryAcquire("fresh", 1, TimeSpan.FromSeconds(1), out retry));
            Assert.False(limiter.TryAcquire("fresh", 1, TimeSpan.FromSeconds(1), out retry));
            Assert.True(limiter.Count < FixedWindowRateLimiter.MaxKeys);
        }
    }
}
