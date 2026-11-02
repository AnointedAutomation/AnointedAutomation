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
using Xunit;

namespace AnointedAutomation.APIMiddlewares.Tests.Security
{
    [Collection("Sequential")]
    public class IpAllowlistRefreshTests : IDisposable
    {
        private readonly SecurityTestLogs.Window _logs = new SecurityTestLogs.Window();

        private sealed class FakeFetcher : IProviderRangeFetcher
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

        public IpAllowlistRefreshTests()
        {
        }

        public void Dispose()
        {
        }

        private static string Rule(IpAllowlistState state, string address)
        {
            return state.FindAllowingRule(IPAddress.Parse(address), IPAddress.Parse(address));
        }

        private static IpAllowlistState Cloudflare()
        {
            IpAllowlistOptions options = new IpAllowlistOptions { Enabled = true };
            options.Providers.Add("Cloudflare");
            return new IpAllowlistState(options);
        }

        private static FakeFetcher GoodCloudflare(string v4, string v6)
        {
            FakeFetcher fetcher = new FakeFetcher();
            fetcher.Bodies["https://www.cloudflare.com/ips-v4"] = v4;
            fetcher.Bodies["https://www.cloudflare.com/ips-v6"] = v6;
            return fetcher;
        }

        [Fact]
        public async Task Success_ReplacesShippedListWithPublishedOne()
        {
            IpAllowlistState state = Cloudflare();
            FakeFetcher fetcher = GoodCloudflare("198.18.0.0/15\n", "2001:db8::/32");
            int failures = await new IpAllowlistRefresher(state, fetcher).RefreshAllAsync(CancellationToken.None);
            Assert.Equal(0, failures);
            Assert.NotNull(Rule(state, "198.18.1.1"));
            Assert.Null(Rule(state, "173.245.48.9"));
            Assert.Empty(_logs.Get());
        }

        [Fact]
        public async Task NetworkFailure_LogsErrorKeepsLastGoodListAndStaysClosed()
        {
            IpAllowlistState state = Cloudflare();
            FakeFetcher fetcher = new FakeFetcher { Throw = new HttpRequestExceptionForTest() };
            int failures = await new IpAllowlistRefresher(state, fetcher).RefreshAllAsync(CancellationToken.None);
            Assert.Equal(1, failures);
            Assert.NotNull(Rule(state, "173.245.48.9"));
            Assert.Null(Rule(state, "198.51.100.1"));
            LogMessage log = _logs.Get().Single();
            Assert.Equal(MessageType.Error, log.messageType);
            Assert.Contains("Cloudflare", log.message);
            Assert.Contains("HttpRequestExceptionForTest", log.exceptionType + log.message);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("not a cidr", "2001:db8::/32")]
        [InlineData("198.18.0.0/15", "")]
        [InlineData("198.18.0.0/15\n999.1.1.1/8", "2001:db8::/32")]
        public async Task MalformedOrEmptyDownload_KeepsLastGoodList(string v4, string v6)
        {
            IpAllowlistState state = Cloudflare();
            int before = state.GetProviderRangeCount("Cloudflare");
            int failures = await new IpAllowlistRefresher(state, GoodCloudflare(v4, v6)).RefreshAllAsync(CancellationToken.None);
            Assert.Equal(1, failures);
            Assert.Equal(before, state.GetProviderRangeCount("Cloudflare"));
            Assert.NotNull(Rule(state, "173.245.48.9"));
            Assert.Equal(MessageType.Error, _logs.Get().Single().messageType);
        }

        [Fact]
        public async Task Cancellation_Rethrows_WithoutErrorLog()
        {
            IpAllowlistState state = Cloudflare();
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                cts.Cancel();
                FakeFetcher fetcher = new FakeFetcher { Throw = new OperationCanceledException(cts.Token) };
                await Assert.ThrowsAsync<OperationCanceledException>(() => new IpAllowlistRefresher(state, fetcher).RefreshAllAsync(cts.Token));
            }
            Assert.Empty(_logs.Get());
        }

        [Fact]
        public async Task TrustedProxyProvider_IsRefreshedToo()
        {
            IpAllowlistOptions options = new IpAllowlistOptions { Enabled = true };
            options.Allowlist.Add("203.0.113.0/24");
            options.ClientIp.TrustedProxyProviders.Add("Cloudflare");
            IpAllowlistState state = new IpAllowlistState(options);
            Assert.Contains("Cloudflare", state.ProviderNames);
            Assert.True(state.TrustedProxies.Contains(IPAddress.Parse("173.245.48.9")));
            int failures = await new IpAllowlistRefresher(state, GoodCloudflare("198.18.0.0/15", "2001:db8::/32")).RefreshAllAsync(CancellationToken.None);
            Assert.Equal(0, failures);
            Assert.True(state.TrustedProxies.Contains(IPAddress.Parse("198.18.1.1")));
            Assert.False(state.TrustedProxies.Contains(IPAddress.Parse("173.245.48.9")));
        }

        [Fact]
        public void ReplaceProvider_RejectsEmptyListAndInactiveProvider()
        {
            IpAllowlistState state = Cloudflare();
            Assert.Throws<ArgumentException>(() => state.ReplaceProvider("Cloudflare", IpNetworkList.Empty));
            Assert.Throws<ArgumentException>(() => state.ReplaceProvider("Fastly", IpNetworkList.Parse(new string[] { "10.0.0.0/8" })));
        }

        [Fact]
        public void Parser_ReadsFastlyJson()
        {
            string json = "{\"addresses\":[\"23.235.32.0/20\"],\"ipv6_addresses\":[\"2a04:4e40::/32\"]}";
            List<string> cidrs = IpProviderListParser.Parse(IpProviderFormat.FastlyJson, new string[] { json });
            Assert.Equal(new string[] { "23.235.32.0/20", "2a04:4e40::/32" }, cidrs);
        }

        [Fact]
        public void Parser_FiltersAwsToCloudFront()
        {
            string json = "{\"prefixes\":[{\"ip_prefix\":\"13.32.0.0/15\",\"service\":\"CLOUDFRONT\"},{\"ip_prefix\":\"3.5.0.0/19\",\"service\":\"S3\"}],"
                + "\"ipv6_prefixes\":[{\"ipv6_prefix\":\"2600:9000::/28\",\"service\":\"CLOUDFRONT\"}]}";
            List<string> cidrs = IpProviderListParser.Parse(IpProviderFormat.AwsCloudFrontJson, new string[] { json });
            Assert.Equal(new string[] { "13.32.0.0/15", "2600:9000::/28" }, cidrs);
        }

        [Fact]
        public void Parser_RejectsMissingArraysAndEmptyResults()
        {
            Assert.Throws<FormatException>(() => IpProviderListParser.Parse(IpProviderFormat.FastlyJson, new string[] { "{}" }));
            Assert.Throws<FormatException>(() => IpProviderListParser.Parse(IpProviderFormat.AwsCloudFrontJson, new string[] { "{\"prefixes\":[],\"ipv6_prefixes\":[]}" }));
            Assert.ThrowsAny<Exception>(() => IpProviderListParser.Parse(IpProviderFormat.FastlyJson, new string[] { "<html>" }));
        }

        [Fact]
        public async Task RefreshService_DoesNothingWhenRefreshDisabled()
        {
            IpAllowlistState state = Cloudflare();
            FakeFetcher fetcher = new FakeFetcher { Throw = new InvalidOperationException("must not be called") };
            IpAllowlistRefreshService service = new IpAllowlistRefreshService(state, new IpAllowlistRefresher(state, fetcher));
            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);
            Assert.Equal(0, fetcher.Calls);
        }

        [Fact]
        public async Task RefreshService_RefreshesOnInterval()
        {
            IpAllowlistOptions options = new IpAllowlistOptions { Enabled = true, RefreshEnabled = true, RefreshInterval = TimeSpan.FromMilliseconds(50) };
            options.Providers.Add("Cloudflare");
            IpAllowlistState state = new IpAllowlistState(options);
            FakeFetcher fetcher = GoodCloudflare("198.18.0.0/15", "2001:db8::/32");
            IpAllowlistRefreshService service = new IpAllowlistRefreshService(state, new IpAllowlistRefresher(state, fetcher));
            await service.StartAsync(CancellationToken.None);
            for (int i = 0; i < 100 && Rule(state, "198.18.1.1") == null; i++)
            {
                await Task.Delay(50);
            }
            await service.StopAsync(CancellationToken.None);
            Assert.NotNull(Rule(state, "198.18.1.1"));
        }

        private sealed class HttpRequestExceptionForTest : Exception
        {
            public HttpRequestExceptionForTest() : base("boom")
            {
            }
        }
    }
}
