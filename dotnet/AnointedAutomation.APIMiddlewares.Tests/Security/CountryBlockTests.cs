// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.APIMiddleware;
using AnointedAutomation.APIMiddleware.Security;
using AnointedAutomation.Optimization.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AnointedAutomation.APIMiddlewares.Tests.Security
{
    [Collection("Sequential")]
    public class CountryBlockTests : IDisposable
    {
        private readonly SecurityTestLogs.Window _logs = new SecurityTestLogs.Window();

        private const string Stats =
            "2|testrir|20261101|5|19700101|20261101|+0000\n"
            + "testrir|*|ipv4|*|3|summary\n"
            + "# a comment\n"
            + "testrir|DE|ipv4|198.51.100.0|256|20200101|allocated\n"
            + "testrir|US|ipv4|203.0.113.0|128|20200101|assigned\n"
            + "testrir|FR|ipv4|192.0.2.0|300|20200101|allocated\n"
            + "testrir|RU|ipv4|100.1.0.0|256|20200101|available\n"
            + "testrir|ZZ|ipv4|100.2.0.0|256|20200101|allocated\n"
            + "testrir|JP|ipv6|2001:db8::|32|20200101|allocated\n"
            + "testrir|BR|ipv6|2001:db9::|48|20200101|assigned\n"
            + "testrir||ipv4|100.3.0.0|256|20200101|reserved\n"
            + "testrir|XX|asn|64512|1|20200101|allocated\n"
            + "garbage line\n";

        private static readonly RirDelegatedCountryLookup Lookup = RirDelegatedCountryLookup.FromStats(new string[] { Stats });

        public CountryBlockTests()
        {
        }

        public void Dispose()
        {
        }

        [Theory]
        [InlineData("198.51.100.0", "DE")]
        [InlineData("198.51.100.255", "DE")]
        [InlineData("198.51.101.0", null)]
        [InlineData("203.0.113.127", "US")]
        [InlineData("203.0.113.128", null)]
        [InlineData("192.0.2.0", "FR")]
        [InlineData("192.0.3.43", "FR")]
        [InlineData("192.0.3.44", null)]
        [InlineData("100.1.0.5", null)]
        [InlineData("100.2.0.5", null)]
        [InlineData("100.3.0.5", null)]
        [InlineData("1.1.1.1", null)]
        [InlineData("2001:db8::1", "JP")]
        [InlineData("2001:db8:ffff:ffff:ffff:ffff:ffff:ffff", "JP")]
        [InlineData("2001:db9:0:1::1", "BR")]
        [InlineData("2001:db9:1::1", null)]
        [InlineData("2001:dba::1", null)]
        [InlineData("::ffff:198.51.100.7", "DE")]
        public void RirLookup_FindsCountry(string address, string expected)
        {
            Assert.Equal(expected, Lookup.Lookup(IPAddress.Parse(address)));
        }

        [Fact]
        public void RirLookup_CountsRangesAndHandlesNullAndEmpty()
        {
            Assert.Equal(3, Lookup.Ipv4RangeCount);
            Assert.Equal(2, Lookup.Ipv6RangeCount);
            Assert.Null(Lookup.Lookup(null));
            Assert.Equal(5, RirDelegatedCountryLookup.CountRanges(Stats));
            Assert.Equal(0, RirDelegatedCountryLookup.CountRanges("<html>not a stats file</html>"));
            Assert.Null(RirDelegatedCountryLookup.FromStats(new string[0]).Lookup(IPAddress.Parse("1.1.1.1")));
        }

        private sealed class FakeLookup : ICountryLookup
        {
            public Dictionary<string, string> Map = new Dictionary<string, string>();

            public string Lookup(IPAddress address)
            {
                string country;
                return Map.TryGetValue(address.ToString(), out country) ? country : null;
            }
        }

        private static CountryBlockOptions Options(CountryBlockMode mode, UnknownCountryAction unknown, params string[] countries)
        {
            CountryBlockOptions options = new CountryBlockOptions { Enabled = true, Mode = mode, UnknownCountry = unknown };
            options.Countries.AddRange(countries);
            return options;
        }

        private static async Task<(int status, bool nextCalled)> Run(CountryBlockOptions options, ICountryLookup lookup, string peer, string path = "/x", string cfCountry = null, string xff = null)
        {
            bool nextCalled = false;
            CountryBlockMiddleware middleware = new CountryBlockMiddleware(context => { nextCalled = true; return Task.CompletedTask; }, options, lookup);
            DefaultHttpContext http = new DefaultHttpContext();
            if (peer != null)
            {
                http.Connection.RemoteIpAddress = IPAddress.Parse(peer);
            }
            http.Request.Path = path;
            http.Request.Method = "GET";
            if (cfCountry != null)
            {
                http.Request.Headers["CF-IPCountry"] = cfCountry;
            }
            if (xff != null)
            {
                http.Request.Headers["X-Forwarded-For"] = xff;
            }
            await middleware.InvokeAsync(http);
            return (http.Response.StatusCode, nextCalled);
        }

        [Fact]
        public async Task OffByDefault_AllowsEverything()
        {
            Assert.False(new CountryBlockOptions().Enabled);
            (int status, bool nextCalled) result = await Run(new CountryBlockOptions(), new FakeLookup(), "198.51.100.9");
            Assert.True(result.nextCalled);
        }

        [Fact]
        public async Task BlockMode_BlocksListedCountryAndLogsIpCountryPath()
        {
            FakeLookup lookup = new FakeLookup();
            lookup.Map["198.51.100.9"] = "DE";
            (int status, bool nextCalled) result = await Run(Options(CountryBlockMode.Block, UnknownCountryAction.Allow, "de", "RU"), lookup, "198.51.100.9", "/secret");
            Assert.False(result.nextCalled);
            Assert.Equal(403, result.status);
            LogMessage log = _logs.Get().Single();
            Assert.Equal(MessageType.Warning, log.messageType);
            Assert.Contains("198.51.100.9", log.message);
            Assert.Contains("country=DE", log.message);
            Assert.Contains("/secret", log.message);
        }

        [Fact]
        public async Task BlockMode_AllowsUnlistedCountry()
        {
            FakeLookup lookup = new FakeLookup();
            lookup.Map["198.51.100.9"] = "US";
            Assert.True((await Run(Options(CountryBlockMode.Block, UnknownCountryAction.Block, "DE"), lookup, "198.51.100.9")).nextCalled);
        }

        [Fact]
        public async Task AllowOnlyMode_AllowsListedBlocksOthers()
        {
            FakeLookup lookup = new FakeLookup();
            lookup.Map["198.51.100.9"] = "US";
            lookup.Map["198.51.100.10"] = "DE";
            CountryBlockOptions options = Options(CountryBlockMode.AllowOnly, UnknownCountryAction.Block, "US", "CA");
            Assert.True((await Run(options, lookup, "198.51.100.9")).nextCalled);
            Assert.False((await Run(options, lookup, "198.51.100.10")).nextCalled);
            Assert.Contains("allow-only", _logs.Get().Single().message);
        }

        [Fact]
        public async Task UnknownCountry_FollowsExplicitSetting()
        {
            FakeLookup lookup = new FakeLookup();
            Assert.False((await Run(Options(CountryBlockMode.Block, UnknownCountryAction.Block, "DE"), lookup, "198.51.100.9")).nextCalled);
            Assert.Contains("country=unknown", _logs.Get().Single().message);
            Assert.True((await Run(Options(CountryBlockMode.Block, UnknownCountryAction.Allow, "DE"), lookup, "198.51.100.9")).nextCalled);
            Assert.True((await Run(Options(CountryBlockMode.AllowOnly, UnknownCountryAction.Allow, "US"), lookup, "198.51.100.9")).nextCalled);
        }

        [Fact]
        public async Task NoClientAddress_IsUnknownCountry()
        {
            Assert.False((await Run(Options(CountryBlockMode.Block, UnknownCountryAction.Block, "DE"), new FakeLookup(), null)).nextCalled);
            Assert.True((await Run(Options(CountryBlockMode.Block, UnknownCountryAction.Allow, "DE"), new FakeLookup(), null)).nextCalled);
        }

        [Fact]
        public async Task PrivateAddress_SkipsCheckByDefaultAndCanBeChecked()
        {
            CountryBlockOptions options = Options(CountryBlockMode.AllowOnly, UnknownCountryAction.Block, "US");
            Assert.True((await Run(options, new FakeLookup(), "10.1.2.3")).nextCalled);
            Assert.True((await Run(options, new FakeLookup(), "::1")).nextCalled);
            options.AllowPrivateAddresses = false;
            Assert.False((await Run(options, new FakeLookup(), "10.1.2.3")).nextCalled);
        }

        [Fact]
        public async Task ExemptPath_SkipsCheck()
        {
            CountryBlockOptions options = Options(CountryBlockMode.AllowOnly, UnknownCountryAction.Block, "US");
            options.ExemptPaths.Add("/webhooks");
            Assert.True((await Run(options, new FakeLookup(), "198.51.100.9", "/webhooks/stripe")).nextCalled);
            Assert.False((await Run(options, new FakeLookup(), "198.51.100.9", "/other")).nextCalled);
        }

        [Fact]
        public async Task CfIpCountryHeader_UsedOnlyFromTrustedProxyAndWhenEnabled()
        {
            FakeLookup lookup = new FakeLookup();
            lookup.Map["203.0.113.5"] = "US";
            CountryBlockOptions options = Options(CountryBlockMode.Block, UnknownCountryAction.Allow, "DE");
            options.UseCfIpCountryHeader = true;
            options.ClientIp.TrustedProxyProviders.Add("Cloudflare");
            options.ClientIp.HonorCfConnectingIp = true;

            // Trusted Cloudflare peer: header says DE, lookup would say US, header wins.
            Assert.False((await Run(options, lookup, "173.245.48.9", cfCountry: "de", xff: "203.0.113.5")).nextCalled);
            // Untrusted peer forging the header: ignored, lookup (US) decides, so allowed.
            Assert.True((await Run(options, lookup, "198.51.100.77", cfCountry: "DE", xff: "203.0.113.5")).nextCalled);
            // Feature off: header ignored even from a trusted peer.
            options.UseCfIpCountryHeader = false;
            Assert.True((await Run(options, lookup, "173.245.48.9", cfCountry: "DE", xff: "203.0.113.5")).nextCalled);
        }

        [Theory]
        [InlineData("XX")]
        [InlineData("T1")]
        [InlineData("")]
        [InlineData("DEU")]
        public async Task CfIpCountryHeader_UnknownValuesFallBackToLookup(string header)
        {
            FakeLookup lookup = new FakeLookup();
            lookup.Map["203.0.113.5"] = "DE";
            CountryBlockOptions options = Options(CountryBlockMode.Block, UnknownCountryAction.Allow, "DE");
            options.UseCfIpCountryHeader = true;
            options.ClientIp.TrustedProxyProviders.Add("Cloudflare");
            Assert.False((await Run(options, lookup, "173.245.48.9", cfCountry: header, xff: "203.0.113.5")).nextCalled);
        }

        [Fact]
        public void Validate_RequiresExplicitModeUnknownAndCountries()
        {
            Assert.Throws<InvalidOperationException>(() => new CountryBlockOptions { Enabled = true, UnknownCountry = UnknownCountryAction.Allow, Countries = { "DE" } }.Validate());
            Assert.Throws<InvalidOperationException>(() => new CountryBlockOptions { Enabled = true, Mode = CountryBlockMode.Block, Countries = { "DE" } }.Validate());
            Assert.Throws<InvalidOperationException>(() => Options(CountryBlockMode.Block, UnknownCountryAction.Allow).Validate());
            Assert.Throws<FormatException>(() => Options(CountryBlockMode.Block, UnknownCountryAction.Allow, "Germany").Validate());
            Assert.Throws<FormatException>(() => Options(CountryBlockMode.Block, UnknownCountryAction.Allow, "D1").Validate());
            new CountryBlockOptions().Validate();
        }

        [Fact]
        public void ConfiguredDataUrls_ReplaceTheDefaultsWhenBound()
        {
            Microsoft.Extensions.Configuration.IConfiguration config = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> { { "CountryBlock:DataUrls:0", "https://mirror.example/delegated-extended" } })
                .Build();
            ServiceCollection services = new ServiceCollection();
            services.AddCountryBlock(config.GetSection("CountryBlock"));
            CountryBlockOptions bound = services.BuildServiceProvider().GetRequiredService<Microsoft.Extensions.Options.IOptions<CountryBlockOptions>>().Value;
            Assert.Equal(new string[] { "https://mirror.example/delegated-extended" }, bound.ResolveDataUrls().ToArray());

            ServiceCollection unconfigured = new ServiceCollection();
            unconfigured.AddCountryBlock(new Microsoft.Extensions.Configuration.ConfigurationBuilder().Build());
            Assert.Equal(5, unconfigured.BuildServiceProvider().GetRequiredService<Microsoft.Extensions.Options.IOptions<CountryBlockOptions>>().Value.ResolveDataUrls().Count);
        }

        [Fact]
        public void Builder_ParsesEachFileOnceAndCountsRanges()
        {
            RirDelegatedCountryLookup.Builder builder = new RirDelegatedCountryLookup.Builder();
            Assert.Equal(5, builder.Add(Stats));
            Assert.Equal(0, builder.Add("<html>nope</html>"));
            Assert.Equal("DE", builder.Build().Lookup(IPAddress.Parse("198.51.100.1")));
        }

        [Fact]
        public void Validate_RejectsBadDataSettings()
        {
            CountryBlockOptions badUrl = Options(CountryBlockMode.Block, UnknownCountryAction.Allow, "DE");
            badUrl.DataUrls = new List<string> { "not a url" };
            Assert.Throws<FormatException>(() => badUrl.Validate());
            CountryBlockOptions noUrls = Options(CountryBlockMode.Block, UnknownCountryAction.Allow, "DE");
            noUrls.Validate();
            Assert.Equal(CountryBlockOptions.RirDataUrls.Length, noUrls.ResolveDataUrls().Count);
            CountryBlockOptions badInterval = Options(CountryBlockMode.Block, UnknownCountryAction.Allow, "DE");
            badInterval.RefreshInterval = TimeSpan.Zero;
            Assert.Throws<InvalidOperationException>(() => badInterval.Validate());
            CountryBlockOptions noDownload = Options(CountryBlockMode.Block, UnknownCountryAction.Allow, "DE");
            noDownload.DownloadRirData = false;
            noDownload.DataUrls = null;
            noDownload.Validate();
        }

        [Fact]
        public void Constructor_ValidatesOptions()
        {
            Assert.Throws<InvalidOperationException>(() => new CountryBlockMiddleware(c => Task.CompletedTask, new CountryBlockOptions { Enabled = true }, new FakeLookup()));
        }

        private sealed class FakeFetcher : ICountryDataFetcher
        {
            public Dictionary<string, string> Bodies = new Dictionary<string, string>();
            public Exception Throw;
            public int Calls;

            public Task<string> FetchAsync(string url, CancellationToken cancellationToken)
            {
                Calls++;
                if (Throw != null)
                {
                    throw Throw;
                }
                return Task.FromResult(Bodies[url]);
            }
        }

        private static CountryBlockOptions TwoFileOptions()
        {
            CountryBlockOptions options = Options(CountryBlockMode.Block, UnknownCountryAction.Allow, "DE");
            options.DataUrls = new List<string> { "https://example.invalid/a", "https://example.invalid/b" };
            return options;
        }

        [Fact]
        public async Task Refresh_Success_LoadsData()
        {
            CountryDataState state = new CountryDataState();
            Assert.False(state.HasData);
            Assert.Null(state.Lookup(IPAddress.Parse("198.51.100.1")));
            FakeFetcher fetcher = new FakeFetcher();
            fetcher.Bodies["https://example.invalid/a"] = Stats;
            fetcher.Bodies["https://example.invalid/b"] = "x|AU|ipv4|1.2.3.0|256|20200101|allocated\n";
            Assert.True(await new CountryDataRefresher(state, fetcher, TwoFileOptions()).RefreshAsync(CancellationToken.None));
            Assert.True(state.HasData);
            Assert.Equal("DE", state.Lookup(IPAddress.Parse("198.51.100.1")));
            Assert.Equal("AU", state.Lookup(IPAddress.Parse("1.2.3.4")));
        }

        [Fact]
        public async Task Refresh_FailureLogsErrorAndKeepsLastGoodData()
        {
            CountryDataState state = new CountryDataState();
            state.Replace(RirDelegatedCountryLookup.FromStats(new string[] { Stats }));
            FakeFetcher fetcher = new FakeFetcher { Throw = new InvalidOperationException("down") };
            Assert.False(await new CountryDataRefresher(state, fetcher, TwoFileOptions()).RefreshAsync(CancellationToken.None));
            Assert.Equal("DE", state.Lookup(IPAddress.Parse("198.51.100.1")));
            LogMessage log = _logs.Get().Single();
            Assert.Equal(MessageType.Error, log.messageType);
            Assert.Contains("keeping the last good data", log.message);
        }

        [Fact]
        public async Task Refresh_OneEmptyFileDiscardsWholeRefresh()
        {
            CountryDataState state = new CountryDataState();
            state.Replace(RirDelegatedCountryLookup.FromStats(new string[] { Stats }));
            FakeFetcher fetcher = new FakeFetcher();
            fetcher.Bodies["https://example.invalid/a"] = "x|AU|ipv4|1.2.3.0|256|20200101|allocated\n";
            fetcher.Bodies["https://example.invalid/b"] = "<html>error page</html>";
            Assert.False(await new CountryDataRefresher(state, fetcher, TwoFileOptions()).RefreshAsync(CancellationToken.None));
            Assert.Equal("DE", state.Lookup(IPAddress.Parse("198.51.100.1")));
            Assert.Null(state.Lookup(IPAddress.Parse("1.2.3.4")));
            Assert.Equal(MessageType.Error, _logs.Get().Single().messageType);
        }

        [Fact]
        public async Task Refresh_FailureWithNoDataSaysUnknownPolicyApplies()
        {
            CountryDataState state = new CountryDataState();
            FakeFetcher fetcher = new FakeFetcher { Throw = new InvalidOperationException("down") };
            Assert.False(await new CountryDataRefresher(state, fetcher, TwoFileOptions()).RefreshAsync(CancellationToken.None));
            Assert.False(state.HasData);
            Assert.Contains("unknown-country policy", _logs.Get().Single().message);
        }

        [Fact]
        public async Task Refresh_CancellationRethrowsWithoutErrorLog()
        {
            CountryDataState state = new CountryDataState();
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                cts.Cancel();
                FakeFetcher fetcher = new FakeFetcher { Throw = new OperationCanceledException(cts.Token) };
                await Assert.ThrowsAsync<OperationCanceledException>(() => new CountryDataRefresher(state, fetcher, TwoFileOptions()).RefreshAsync(cts.Token));
            }
            Assert.Empty(_logs.Get());
        }

        [Fact]
        public async Task RefreshService_LoadsThenSkipsWhenHostBringsOwnLookup()
        {
            CountryBlockOptions options = TwoFileOptions();
            FakeFetcher fetcher = new FakeFetcher();
            fetcher.Bodies["https://example.invalid/a"] = Stats;
            fetcher.Bodies["https://example.invalid/b"] = Stats;

            CountryDataState state = new CountryDataState();
            CountryDataRefreshService service = new CountryDataRefreshService(options, state, new CountryDataRefresher(state, fetcher, options), state);
            await service.StartAsync(CancellationToken.None);
            for (int i = 0; i < 100 && !state.HasData; i++)
            {
                await Task.Delay(20);
            }
            await service.StopAsync(CancellationToken.None);
            Assert.True(state.HasData);

            FakeFetcher untouched = new FakeFetcher();
            CountryDataState other = new CountryDataState();
            CountryDataRefreshService custom = new CountryDataRefreshService(options, other, new CountryDataRefresher(other, untouched, options), new FakeLookup());
            await custom.StartAsync(CancellationToken.None);
            await custom.StopAsync(CancellationToken.None);
            Assert.Equal(0, untouched.Calls);
        }

        [Fact]
        public async Task RefreshService_RetriesAfterFailureUntilData()
        {
            CountryBlockOptions options = TwoFileOptions();
            options.RetryInterval = TimeSpan.FromMilliseconds(30);
            FakeFetcher fetcher = new FakeFetcher { Throw = new InvalidOperationException("down") };
            CountryDataState state = new CountryDataState();
            CountryDataRefreshService service = new CountryDataRefreshService(options, state, new CountryDataRefresher(state, fetcher, options), state);
            await service.StartAsync(CancellationToken.None);
            for (int i = 0; i < 100 && fetcher.Calls < 2; i++)
            {
                await Task.Delay(20);
            }
            await service.StopAsync(CancellationToken.None);
            Assert.True(fetcher.Calls >= 2);
            Assert.False(state.HasData);
        }

        [Fact]
        public void Extensions_RegisterAndBuildPipeline()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddCountryBlock(options =>
            {
                options.Enabled = true;
                options.Mode = CountryBlockMode.Block;
                options.UnknownCountry = UnknownCountryAction.Allow;
                options.Countries.Add("DE");
            });
            ServiceProvider provider = services.BuildServiceProvider();
            Assert.Same(provider.GetRequiredService<CountryDataState>(), provider.GetRequiredService<ICountryLookup>());
            Assert.NotNull(provider.GetRequiredService<CountryDataRefresher>());
            Assert.NotNull(new ApplicationBuilder(provider).UseCountryBlock());
        }

        [Fact]
        public void Extensions_HostLookupWinsAndInvalidOptionsThrow()
        {
            FakeLookup custom = new FakeLookup();
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<ICountryLookup>(custom);
            services.AddCountryBlock(options => options.Enabled = true);
            ServiceProvider provider = services.BuildServiceProvider();
            Assert.Same(custom, provider.GetRequiredService<ICountryLookup>());
            Assert.Throws<InvalidOperationException>(() => new ApplicationBuilder(provider).UseCountryBlock());
        }
    }
}
