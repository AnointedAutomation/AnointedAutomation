// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.APIMiddleware;
using AnointedAutomation.APIMiddleware.BotDefense;
using AnointedAutomation.APIMiddlewares.Tests.Security;
using AnointedAutomation.Optimization.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace AnointedAutomation.APIMiddlewares.Tests.BotDefense
{
    [Collection("Sequential")]
    public class BotDefenseMiddlewareTests : IDisposable
    {
        private readonly SecurityTestLogs.Window _logs = new SecurityTestLogs.Window();

        private sealed class FakeCrawlerVerifier : ICrawlerVerifier
        {
            public bool Result;
            public int Calls;
            public IReadOnlyList<string> LastSuffixes;

            public Task<bool> VerifyAsync(IPAddress address, IReadOnlyList<string> hostSuffixes, CancellationToken cancellationToken)
            {
                Calls++;
                LastSuffixes = hostSuffixes;
                return Task.FromResult(Result);
            }
        }

        private sealed class ThrowingBanStore : IBanStore
        {
            public Task<BanRecord> GetBanAsync(string ip, CancellationToken cancellationToken)
            {
                throw new InvalidOperationException("store down");
            }

            public Task BanAsync(string ip, string reason, TimeSpan duration, CancellationToken cancellationToken)
            {
                throw new InvalidOperationException("store down");
            }

            public Task UnbanAsync(string ip, CancellationToken cancellationToken)
            {
                throw new InvalidOperationException("store down");
            }
        }

        private DateTimeOffset _now = new DateTimeOffset(2026, 11, 2, 12, 0, 0, TimeSpan.Zero);
        private readonly FakeCrawlerVerifier _crawlers = new FakeCrawlerVerifier();

        public BotDefenseMiddlewareTests()
        {
        }

        public void Dispose()
        {
        }

        private BotDefenseMiddleware Build(BotDefenseOptions options, IBanStore bans, Action onNext)
        {
            return new BotDefenseMiddleware(
                context => { onNext(); return Task.CompletedTask; },
                Options.Create(options),
                bans,
                _crawlers,
                new FixedWindowRateLimiter(() => _now));
        }

        private static DefaultHttpContext Request(string peer, string path, string userAgent = null, string method = "GET")
        {
            DefaultHttpContext http = new DefaultHttpContext();
            http.Connection.RemoteIpAddress = IPAddress.Parse(peer);
            http.Request.Path = path;
            http.Request.Method = method;
            if (userAgent != null)
            {
                http.Request.Headers["User-Agent"] = userAgent;
            }
            return http;
        }

        private static BotDefenseOptions On()
        {
            return new BotDefenseOptions { Enabled = true };
        }

        private InMemoryBanStore NewStore()
        {
            return new InMemoryBanStore(() => _now);
        }

        [Fact]
        public async Task OffByDefault_PassesEverything()
        {
            Assert.False(new BotDefenseOptions().Enabled);
            Assert.False(new BotDefenseOptions().HoneypotPaths.Enabled);
            Assert.False(new BotDefenseOptions().UserAgent.Enabled);
            Assert.False(new BotDefenseOptions().RateLimit.Enabled);
            int next = 0;
            BotDefenseOptions options = new BotDefenseOptions();
            options.HoneypotPaths.Enabled = true;
            await Build(options, NewStore(), () => next++).InvokeAsync(Request("198.51.100.1", "/wp-admin"));
            Assert.Equal(1, next);
        }

        [Fact]
        public async Task EnabledWithNoRulesOn_PassesEverything()
        {
            int next = 0;
            await Build(On(), NewStore(), () => next++).InvokeAsync(Request("198.51.100.1", "/wp-admin", "sqlmap"));
            Assert.Equal(1, next);
        }

        [Theory]
        [InlineData("/wp-admin")]
        [InlineData("/wp-admin/setup.php")]
        [InlineData("/PHPMYADMIN")]
        [InlineData("/.env")]
        [InlineData("/app/config/.env")]
        [InlineData("/.git/config")]
        public async Task HoneypotPath_BansIpAndLogsWarning(string path)
        {
            BotDefenseOptions options = On();
            options.HoneypotPaths.Enabled = true;
            InMemoryBanStore bans = NewStore();
            int next = 0;
            BotDefenseMiddleware middleware = Build(options, bans, () => next++);
            DefaultHttpContext first = Request("198.51.100.1", path);
            await middleware.InvokeAsync(first);
            Assert.Equal(404, first.Response.StatusCode);
            Assert.Equal(0, next);
            Assert.NotNull(await bans.GetBanAsync("198.51.100.1", CancellationToken.None));
            LogMessage log = _logs.Get().Single();
            Assert.Equal(MessageType.Warning, log.messageType);
            Assert.Contains("198.51.100.1", log.message);
            Assert.Contains("honeypot", log.message);
            Assert.Contains(path, log.message, StringComparison.OrdinalIgnoreCase);

            // Next, perfectly normal request from the banned IP is refused.
            DefaultHttpContext second = Request("198.51.100.1", "/api/products");
            await middleware.InvokeAsync(second);
            Assert.Equal(403, second.Response.StatusCode);
            Assert.Equal(0, next);
            Assert.Contains("banned", _logs.Get().Last().message);

            // Another IP is unaffected.
            await middleware.InvokeAsync(Request("198.51.100.2", "/api/products"));
            Assert.Equal(1, next);
        }

        [Fact]
        public async Task HoneypotPath_LookalikeIsNotTripped()
        {
            BotDefenseOptions options = On();
            options.HoneypotPaths.Enabled = true;
            int next = 0;
            BotDefenseMiddleware middleware = Build(options, NewStore(), () => next++);
            await middleware.InvokeAsync(Request("198.51.100.1", "/wp-administrator"));
            await middleware.InvokeAsync(Request("198.51.100.1", "/environment"));
            await middleware.InvokeAsync(Request("198.51.100.1", "/"));
            Assert.Equal(3, next);
        }

        [Fact]
        public async Task Ban_ExpiresAfterConfiguredDuration()
        {
            BotDefenseOptions options = On();
            options.HoneypotPaths.Enabled = true;
            options.HoneypotPaths.BanDuration = TimeSpan.FromMinutes(10);
            int next = 0;
            BotDefenseMiddleware middleware = Build(options, NewStore(), () => next++);
            await middleware.InvokeAsync(Request("198.51.100.1", "/wp-admin"));
            _now = _now.AddMinutes(9);
            await middleware.InvokeAsync(Request("198.51.100.1", "/ok"));
            Assert.Equal(0, next);
            _now = _now.AddMinutes(2);
            await middleware.InvokeAsync(Request("198.51.100.1", "/ok"));
            Assert.Equal(1, next);
        }

        [Fact]
        public async Task ExemptPath_SkipsEveryRule()
        {
            BotDefenseOptions options = On();
            options.ExemptPaths.Add("/health");
            options.UserAgent.Enabled = true;
            options.UserAgent.BlockEmpty = true;
            int next = 0;
            await Build(options, NewStore(), () => next++).InvokeAsync(Request("198.51.100.1", "/health"));
            Assert.Equal(1, next);
        }

        [Theory]
        [InlineData("sqlmap/1.7")]
        [InlineData("Mozilla/5.0 (compatible; Nikto/2.1.6)")]
        [InlineData("Nmap Scripting Engine")]
        public async Task UserAgent_KnownBadIsBlocked(string userAgent)
        {
            BotDefenseOptions options = On();
            options.UserAgent.Enabled = true;
            int next = 0;
            DefaultHttpContext http = Request("198.51.100.1", "/x", userAgent);
            await Build(options, NewStore(), () => next++).InvokeAsync(http);
            Assert.Equal(403, http.Response.StatusCode);
            Assert.Equal(0, next);
            Assert.Contains("bad bot", _logs.Get().Single().message);
        }

        [Fact]
        public async Task UserAgent_CustomFragmentEmptyAndNormalBrowsers()
        {
            BotDefenseOptions options = On();
            options.UserAgent.Enabled = true;
            options.UserAgent.BlockEmpty = true;
            options.UserAgent.BlockedFragments.Add("EvilScraper");
            int next = 0;
            BotDefenseMiddleware middleware = Build(options, NewStore(), () => next++);
            DefaultHttpContext custom = Request("198.51.100.1", "/x", "xx evilscraper xx");
            await middleware.InvokeAsync(custom);
            Assert.Equal(403, custom.Response.StatusCode);
            DefaultHttpContext empty = Request("198.51.100.1", "/x");
            await middleware.InvokeAsync(empty);
            Assert.Equal(403, empty.Response.StatusCode);
            await middleware.InvokeAsync(Request("198.51.100.1", "/x", "Mozilla/5.0 (X11; Linux x86_64) Firefox/130.0"));
            await middleware.InvokeAsync(Request("198.51.100.1", "/x", "curl/8.5.0"));
            Assert.Equal(2, next);
        }

        [Fact]
        public async Task UserAgent_EmptyAllowedWhenNotConfigured_AndKnownBadCanBeDisabled()
        {
            BotDefenseOptions options = On();
            options.UserAgent.Enabled = true;
            options.UserAgent.BlockKnownBad = false;
            int next = 0;
            BotDefenseMiddleware middleware = Build(options, NewStore(), () => next++);
            await middleware.InvokeAsync(Request("198.51.100.1", "/x"));
            await middleware.InvokeAsync(Request("198.51.100.1", "/x", "sqlmap"));
            Assert.Equal(2, next);
        }

        [Fact]
        public async Task GoodCrawler_VerifiedPassesEvenOnHoneypotAndBadRules()
        {
            _crawlers.Result = true;
            BotDefenseOptions options = On();
            options.UserAgent.Enabled = true;
            options.HoneypotPaths.Enabled = true;
            int next = 0;
            await Build(options, NewStore(), () => next++).InvokeAsync(Request("198.51.100.1", "/wp-admin", "Mozilla/5.0 (compatible; Googlebot/2.1)"));
            Assert.Equal(1, next);
            Assert.Contains(".googlebot.com", _crawlers.LastSuffixes);
            Assert.Empty(_logs.Get());
        }

        [Fact]
        public async Task GoodCrawler_SpoofedIsBlockedAndLogged()
        {
            _crawlers.Result = false;
            BotDefenseOptions options = On();
            options.UserAgent.Enabled = true;
            int next = 0;
            DefaultHttpContext http = Request("198.51.100.1", "/x", "Mozilla/5.0 (compatible; bingbot/2.0)");
            await Build(options, NewStore(), () => next++).InvokeAsync(http);
            Assert.Equal(403, http.Response.StatusCode);
            Assert.Equal(0, next);
            LogMessage log = _logs.Get().Single();
            Assert.Contains("spoofed crawler", log.message);
            Assert.Contains("198.51.100.1", log.message);
        }

        [Fact]
        public async Task RateLimit_PerIpGlobalReturns429WithRetryAfterAndResets()
        {
            BotDefenseOptions options = On();
            options.RateLimit.Enabled = true;
            options.RateLimit.PerIpLimit = 3;
            options.RateLimit.PerIpWindow = TimeSpan.FromMinutes(1);
            int next = 0;
            BotDefenseMiddleware middleware = Build(options, NewStore(), () => next++);
            for (int i = 0; i < 3; i++)
            {
                await middleware.InvokeAsync(Request("198.51.100.1", "/x"));
            }
            DefaultHttpContext limited = Request("198.51.100.1", "/x");
            await middleware.InvokeAsync(limited);
            Assert.Equal(429, limited.Response.StatusCode);
            Assert.Equal("60", limited.Response.Headers["Retry-After"].ToString());
            Assert.Equal(3, next);
            Assert.Contains("rate limit", _logs.Get().Single().message);

            await middleware.InvokeAsync(Request("198.51.100.2", "/x"));
            Assert.Equal(4, next);

            _now = _now.AddMinutes(1);
            await middleware.InvokeAsync(Request("198.51.100.1", "/x"));
            Assert.Equal(5, next);
        }

        [Fact]
        public async Task RateLimit_PerRouteLimitsOnlyThatRoute()
        {
            BotDefenseOptions options = On();
            options.RateLimit.Enabled = true;
            options.RateLimit.Routes.Add(new RouteRateLimit { PathPrefix = "/login", Limit = 2, Window = TimeSpan.FromMinutes(5) });
            int next = 0;
            BotDefenseMiddleware middleware = Build(options, NewStore(), () => next++);
            await middleware.InvokeAsync(Request("198.51.100.1", "/login"));
            await middleware.InvokeAsync(Request("198.51.100.1", "/login/reset"));
            DefaultHttpContext third = Request("198.51.100.1", "/login");
            await middleware.InvokeAsync(third);
            Assert.Equal(429, third.Response.StatusCode);
            Assert.Equal(2, next);
            for (int i = 0; i < 10; i++)
            {
                await middleware.InvokeAsync(Request("198.51.100.1", "/products"));
            }
            Assert.Equal(12, next);
        }

        [Fact]
        public async Task RateLimit_LongestRoutePrefixWins()
        {
            BotDefenseOptions options = On();
            options.RateLimit.Enabled = true;
            options.RateLimit.Routes.Add(new RouteRateLimit { PathPrefix = "/api", Limit = 100, Window = TimeSpan.FromMinutes(1) });
            options.RateLimit.Routes.Add(new RouteRateLimit { PathPrefix = "/api/search", Limit = 1, Window = TimeSpan.FromMinutes(1) });
            int next = 0;
            BotDefenseMiddleware middleware = Build(options, NewStore(), () => next++);
            await middleware.InvokeAsync(Request("198.51.100.1", "/api/search"));
            DefaultHttpContext second = Request("198.51.100.1", "/api/search");
            await middleware.InvokeAsync(second);
            Assert.Equal(429, second.Response.StatusCode);
            await middleware.InvokeAsync(Request("198.51.100.1", "/api/other"));
            Assert.Equal(2, next);
        }

        [Fact]
        public async Task ForwardedFor_OnlyHonoredFromTrustedProxyForBansAndLimits()
        {
            BotDefenseOptions options = On();
            options.HoneypotPaths.Enabled = true;
            options.ClientIp.TrustedProxies.Add("10.0.0.0/8");
            InMemoryBanStore bans = NewStore();
            int next = 0;
            BotDefenseMiddleware middleware = Build(options, bans, () => next++);
            DefaultHttpContext viaProxy = Request("10.0.0.4", "/wp-admin");
            viaProxy.Request.Headers["X-Forwarded-For"] = "203.0.113.50";
            await middleware.InvokeAsync(viaProxy);
            Assert.NotNull(await bans.GetBanAsync("203.0.113.50", CancellationToken.None));
            Assert.Null(await bans.GetBanAsync("10.0.0.4", CancellationToken.None));

            DefaultHttpContext forged = Request("198.51.100.9", "/wp-admin");
            forged.Request.Headers["X-Forwarded-For"] = "203.0.113.99";
            await middleware.InvokeAsync(forged);
            Assert.NotNull(await bans.GetBanAsync("198.51.100.9", CancellationToken.None));
            Assert.Null(await bans.GetBanAsync("203.0.113.99", CancellationToken.None));
        }

        [Fact]
        public async Task BanStoreReadFailure_LogsErrorAndFollowsSetting()
        {
            BotDefenseOptions options = On();
            options.HoneypotPaths.Enabled = true;
            int next = 0;
            await Build(options, new ThrowingBanStore(), () => next++).InvokeAsync(Request("198.51.100.1", "/ok"));
            Assert.Equal(1, next);
            LogMessage log = _logs.Get().Single();
            Assert.Equal(MessageType.Error, log.messageType);
            Assert.Contains("InvalidOperationException", log.exceptionType);

            options.BlockWhenBanStoreFails = true;
            DefaultHttpContext blocked = Request("198.51.100.1", "/ok");
            await Build(options, new ThrowingBanStore(), () => next++).InvokeAsync(blocked);
            Assert.Equal(503, blocked.Response.StatusCode);
            Assert.Equal(1, next);
        }

        [Fact]
        public async Task BanStoreWriteFailure_LogsErrorButStillAnswersHoneypot()
        {
            BotDefenseOptions options = On();
            options.HoneypotPaths.Enabled = true;
            options.BlockWhenBanStoreFails = false;
            ThrowingWriteStore store = new ThrowingWriteStore();
            DefaultHttpContext http = Request("198.51.100.1", "/wp-admin");
            await Build(options, store, () => { }).InvokeAsync(http);
            Assert.Equal(404, http.Response.StatusCode);
            List<LogMessage> logs = _logs.Get().ToList();
            Assert.Contains(logs, l => l.messageType == MessageType.Error && l.message.Contains("not banned"));
            Assert.Contains(logs, l => l.messageType == MessageType.Warning);
        }

        private sealed class ThrowingWriteStore : IBanStore
        {
            public Task<BanRecord> GetBanAsync(string ip, CancellationToken cancellationToken)
            {
                return Task.FromResult<BanRecord>(null);
            }

            public Task BanAsync(string ip, string reason, TimeSpan duration, CancellationToken cancellationToken)
            {
                throw new InvalidOperationException("write failed");
            }

            public Task UnbanAsync(string ip, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }

        [Fact]
        public void Validate_RejectsBrokenRuleConfigurations()
        {
            BotDefenseOptions badHoneypotPath = On();
            badHoneypotPath.HoneypotPaths.Enabled = true;
            badHoneypotPath.HoneypotPaths.Paths.Add("no-slash");
            Assert.Throws<ArgumentException>(() => badHoneypotPath.Validate());

            BotDefenseOptions badDuration = On();
            badDuration.HoneypotPaths.Enabled = true;
            badDuration.HoneypotPaths.BanDuration = TimeSpan.Zero;
            Assert.Throws<InvalidOperationException>(() => badDuration.Validate());

            BotDefenseOptions badStatus = On();
            badStatus.HoneypotPaths.Enabled = true;
            badStatus.HoneypotPaths.ResponseStatus = 12;
            Assert.Throws<InvalidOperationException>(() => badStatus.Validate());

            BotDefenseOptions emptyLimit = On();
            emptyLimit.RateLimit.Enabled = true;
            Assert.Throws<InvalidOperationException>(() => emptyLimit.Validate());

            BotDefenseOptions badRoute = On();
            badRoute.RateLimit.Enabled = true;
            badRoute.RateLimit.Routes.Add(new RouteRateLimit { PathPrefix = "/x", Limit = 0 });
            Assert.Throws<InvalidOperationException>(() => badRoute.Validate());

            BotDefenseOptions badCrawler = On();
            badCrawler.UserAgent.Enabled = true;
            badCrawler.UserAgent.GoodCrawlers.Add(new GoodCrawler { Name = "x", UserAgentFragment = "x" });
            Assert.Throws<InvalidOperationException>(() => badCrawler.Validate());

            BotDefenseOptions badPath = On();
            badPath.ExemptPaths.Add("nope");
            Assert.Throws<ArgumentException>(() => badPath.Validate());

            new BotDefenseOptions { HoneypotPaths = null }.Validate();
        }

        [Fact]
        public async Task ConfiguredHoneypotListsAndCrawlersReplaceDefaults()
        {
            BotDefenseOptions options = On();
            options.HoneypotPaths.Enabled = true;
            options.HoneypotPaths.Paths.Add("/only-this");
            options.HoneypotPaths.FileNames.Add("secret.txt");
            int next = 0;
            BotDefenseMiddleware middleware = Build(options, NewStore(), () => next++);
            await middleware.InvokeAsync(Request("198.51.100.1", "/wp-admin"));
            await middleware.InvokeAsync(Request("198.51.100.1", "/.env"));
            Assert.Equal(2, next);
            DefaultHttpContext hit = Request("198.51.100.2", "/only-this");
            await middleware.InvokeAsync(hit);
            Assert.Equal(404, hit.Response.StatusCode);
            DefaultHttpContext fileHit = Request("198.51.100.3", "/x/secret.txt");
            await middleware.InvokeAsync(fileHit);
            Assert.Equal(404, fileHit.Response.StatusCode);

            BotDefenseOptions defaults = On();
            Assert.Equal(HoneypotPathOptions.DefaultPaths.Length, defaults.HoneypotPaths.ResolvePaths().Count);
            Assert.Equal(5, defaults.UserAgent.ResolveGoodCrawlers().Count);
            defaults.UserAgent.GoodCrawlers.Add(new GoodCrawler { Name = "Mine", UserAgentFragment = "MyBot", HostSuffixes = new List<string> { ".mine.example" } });
            Assert.Single(defaults.UserAgent.ResolveGoodCrawlers());
        }

        [Fact]
        public void ConfigurationBinding_ReplacesDefaultLists()
        {
            Microsoft.Extensions.Configuration.IConfiguration config = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "BotDefense:HoneypotPaths:Paths:0", "/only" },
                    { "BotDefense:HoneypotPaths:FileNames:0", "x.cfg" },
                    { "BotDefense:UserAgent:GoodCrawlers:0:Name", "Mine" },
                    { "BotDefense:UserAgent:GoodCrawlers:0:UserAgentFragment", "MyBot" },
                    { "BotDefense:UserAgent:GoodCrawlers:0:HostSuffixes:0", ".mine.example" }
                })
                .Build();
            ServiceCollection services = new ServiceCollection();
            services.AddBotDefense(config.GetSection("BotDefense"));
            BotDefenseOptions bound = services.BuildServiceProvider().GetRequiredService<IOptions<BotDefenseOptions>>().Value;
            Assert.Equal(new string[] { "/only" }, bound.HoneypotPaths.ResolvePaths().ToArray());
            Assert.Equal(new string[] { "x.cfg" }, bound.HoneypotPaths.ResolveFileNames().ToArray());
            Assert.Equal("Mine", bound.UserAgent.ResolveGoodCrawlers().Single().Name);
        }

        [Fact]
        public async Task MappedIpv4Peer_SharesBanKeyWithPlainIpv4AndVerifiesAsCrawler()
        {
            BotDefenseOptions options = On();
            options.HoneypotPaths.Enabled = true;
            InMemoryBanStore bans = NewStore();
            int next = 0;
            BotDefenseMiddleware middleware = Build(options, bans, () => next++);
            await middleware.InvokeAsync(Request("::ffff:198.51.100.1", "/wp-admin"));
            Assert.NotNull(await bans.GetBanAsync("198.51.100.1", CancellationToken.None));
            DefaultHttpContext plain = Request("198.51.100.1", "/ok");
            await middleware.InvokeAsync(plain);
            Assert.Equal(403, plain.Response.StatusCode);
        }

        [Fact]
        public async Task ReverseDnsVerifier_MappedPeerAndMappedForwardResultsStillVerify()
        {
            Dictionary<string, IPHostEntry> table = new Dictionary<string, IPHostEntry>
            {
                { "66.249.66.1", new IPHostEntry { HostName = "crawl.googlebot.com" } },
                { "crawl.googlebot.com", new IPHostEntry { HostName = "crawl.googlebot.com", AddressList = new IPAddress[] { IPAddress.Parse("::ffff:66.249.66.1") } } }
            };
            ReverseDnsCrawlerVerifier verifier = new ReverseDnsCrawlerVerifier(TimeSpan.FromHours(1), () => _now, FakeDns(table, new List<string>()));
            Assert.True(await verifier.VerifyAsync(IPAddress.Parse("::ffff:66.249.66.1"), new string[] { ".googlebot.com" }, CancellationToken.None));
            Assert.True(await verifier.VerifyAsync(IPAddress.Parse("66.249.66.1"), new string[] { ".googlebot.com" }, CancellationToken.None));
        }

        [Fact]
        public async Task ReverseDnsVerifier_CacheStaysBounded()
        {
            ReverseDnsCrawlerVerifier verifier = new ReverseDnsCrawlerVerifier(TimeSpan.FromHours(1), () => _now, FakeDnsFail());
            for (int i = 0; i < ReverseDnsCrawlerVerifier.MaxCacheEntries + 500; i++)
            {
                await verifier.VerifyAsync(new IPAddress(new byte[] { 10, (byte)(i >> 16), (byte)(i >> 8), (byte)i }), new string[] { ".googlebot.com" }, CancellationToken.None);
            }
            Assert.True(verifier.CacheCount <= ReverseDnsCrawlerVerifier.MaxCacheEntries);
            // Once entries expire, space is reclaimed.
            _now = _now.AddHours(2);
            _now = _now.AddMinutes(2);
            await verifier.VerifyAsync(IPAddress.Parse("192.0.2.77"), new string[] { ".googlebot.com" }, CancellationToken.None);
            Assert.True(verifier.CacheCount < ReverseDnsCrawlerVerifier.MaxCacheEntries);
        }

        private static Func<string, CancellationToken, Task<IPHostEntry>> FakeDnsFail()
        {
            return (name, token) => Task.FromResult(new IPHostEntry { HostName = "nothing.example" });
        }

        [Fact]
        public async Task InMemoryBanStore_ExtendsAndUnbans()
        {
            InMemoryBanStore store = NewStore();
            await store.BanAsync("1.1.1.1", "a", TimeSpan.FromMinutes(10), CancellationToken.None);
            await store.BanAsync("1.1.1.1", "b", TimeSpan.FromMinutes(1), CancellationToken.None);
            _now = _now.AddMinutes(5);
            BanRecord record = await store.GetBanAsync("1.1.1.1", CancellationToken.None);
            Assert.NotNull(record);
            await store.UnbanAsync("1.1.1.1", CancellationToken.None);
            Assert.Null(await store.GetBanAsync("1.1.1.1", CancellationToken.None));
            Assert.Equal(0, store.Count);
        }

        [Fact]
        public void HostMatches_RequiresLabelBoundary()
        {
            Assert.True(ReverseDnsCrawlerVerifier.HostMatches("crawl-66-249-66-1.googlebot.com", new string[] { ".googlebot.com" }));
            Assert.True(ReverseDnsCrawlerVerifier.HostMatches("CRAWL.GOOGLEBOT.COM", new string[] { "googlebot.com" }));
            Assert.False(ReverseDnsCrawlerVerifier.HostMatches("evilgooglebot.com", new string[] { ".googlebot.com" }));
            Assert.False(ReverseDnsCrawlerVerifier.HostMatches("googlebot.com.evil.example", new string[] { ".googlebot.com" }));
        }

        private static Func<string, CancellationToken, Task<IPHostEntry>> FakeDns(Dictionary<string, IPHostEntry> table, List<string> calls)
        {
            return (name, token) =>
            {
                calls.Add(name);
                IPHostEntry entry;
                if (!table.TryGetValue(name, out entry))
                {
                    throw new System.Net.Sockets.SocketException(11001);
                }
                return Task.FromResult(entry);
            };
        }

        [Fact]
        public async Task ReverseDnsVerifier_ConfirmsForwardAndReverseAndCaches()
        {
            Dictionary<string, IPHostEntry> table = new Dictionary<string, IPHostEntry>
            {
                { "66.249.66.1", new IPHostEntry { HostName = "crawl-66-249-66-1.googlebot.com." } },
                { "crawl-66-249-66-1.googlebot.com", new IPHostEntry { HostName = "crawl-66-249-66-1.googlebot.com", AddressList = new IPAddress[] { IPAddress.Parse("66.249.66.1") } } }
            };
            List<string> calls = new List<string>();
            ReverseDnsCrawlerVerifier verifier = new ReverseDnsCrawlerVerifier(TimeSpan.FromHours(1), () => _now, FakeDns(table, calls));
            Assert.True(await verifier.VerifyAsync(IPAddress.Parse("66.249.66.1"), new string[] { ".googlebot.com" }, CancellationToken.None));
            Assert.True(await verifier.VerifyAsync(IPAddress.Parse("66.249.66.1"), new string[] { ".googlebot.com" }, CancellationToken.None));
            Assert.Equal(2, calls.Count);
            _now = _now.AddHours(2);
            Assert.True(await verifier.VerifyAsync(IPAddress.Parse("66.249.66.1"), new string[] { ".googlebot.com" }, CancellationToken.None));
            Assert.Equal(4, calls.Count);
        }

        [Fact]
        public async Task ReverseDnsVerifier_RejectsWrongSuffixAndForwardMismatch()
        {
            Dictionary<string, IPHostEntry> table = new Dictionary<string, IPHostEntry>
            {
                { "198.51.100.1", new IPHostEntry { HostName = "bot.evil.example" } },
                { "198.51.100.2", new IPHostEntry { HostName = "crawl.googlebot.com" } },
                { "crawl.googlebot.com", new IPHostEntry { HostName = "crawl.googlebot.com", AddressList = new IPAddress[] { IPAddress.Parse("66.249.66.1") } } }
            };
            ReverseDnsCrawlerVerifier verifier = new ReverseDnsCrawlerVerifier(TimeSpan.FromHours(1), () => _now, FakeDns(table, new List<string>()));
            Assert.False(await verifier.VerifyAsync(IPAddress.Parse("198.51.100.1"), new string[] { ".googlebot.com" }, CancellationToken.None));
            Assert.False(await verifier.VerifyAsync(IPAddress.Parse("198.51.100.2"), new string[] { ".googlebot.com" }, CancellationToken.None));
            Assert.Empty(_logs.Get());
        }

        [Fact]
        public async Task ReverseDnsVerifier_DnsFailureLogsWarningWithExceptionAndIsUnverified()
        {
            ReverseDnsCrawlerVerifier verifier = new ReverseDnsCrawlerVerifier(TimeSpan.FromHours(1), () => _now, FakeDns(new Dictionary<string, IPHostEntry>(), new List<string>()));
            Assert.False(await verifier.VerifyAsync(IPAddress.Parse("192.0.2.1"), new string[] { ".googlebot.com" }, CancellationToken.None));
            LogMessage log = _logs.Get().Single();
            Assert.Equal(MessageType.Warning, log.messageType);
            Assert.Contains("SocketException", log.exceptionType);
        }

        [Fact]
        public void Extensions_RegisterAndBuildPipeline()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddBotDefense(options =>
            {
                options.Enabled = true;
                options.HoneypotPaths.Enabled = true;
            });
            ServiceProvider provider = services.BuildServiceProvider();
            Assert.IsType<InMemoryBanStore>(provider.GetRequiredService<IBanStore>());
            Assert.NotNull(new ApplicationBuilder(provider).UseBotDefense());
        }

        [Fact]
        public void Extensions_HostBanStoreWinsAndInvalidOptionsThrow()
        {
            ThrowingBanStore custom = new ThrowingBanStore();
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<IBanStore>(custom);
            services.AddBotDefense(options =>
            {
                options.Enabled = true;
                options.RateLimit.Enabled = true;
            });
            ServiceProvider provider = services.BuildServiceProvider();
            Assert.Same(custom, provider.GetRequiredService<IBanStore>());
            Assert.Throws<InvalidOperationException>(() => new ApplicationBuilder(provider).UseBotDefense());
        }
    }
}
