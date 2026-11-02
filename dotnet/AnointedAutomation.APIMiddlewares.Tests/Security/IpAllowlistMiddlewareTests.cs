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
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AnointedAutomation.APIMiddlewares.Tests.Security
{
    [Collection("Sequential")]
    public class IpAllowlistMiddlewareTests : IDisposable
    {
        public IpAllowlistMiddlewareTests()
        {
            IPBlacklistMiddleware.ClearLogs();
        }

        public void Dispose()
        {
            IPBlacklistMiddleware.ClearLogs();
        }

        private static async Task<(int status, bool nextCalled)> Run(IpAllowlistOptions options, string peer, string path, string xff = null, string cf = null)
        {
            bool nextCalled = false;
            IpAllowlistMiddleware middleware = new IpAllowlistMiddleware(
                context => { nextCalled = true; return Task.CompletedTask; },
                new IpAllowlistState(options));
            DefaultHttpContext http = new DefaultHttpContext();
            if (peer != null)
            {
                http.Connection.RemoteIpAddress = IPAddress.Parse(peer);
            }
            http.Request.Path = path;
            http.Request.Method = "GET";
            if (xff != null)
            {
                http.Request.Headers["X-Forwarded-For"] = xff;
            }
            if (cf != null)
            {
                http.Request.Headers["CF-Connecting-IP"] = cf;
            }
            await middleware.InvokeAsync(http);
            return (http.Response.StatusCode, nextCalled);
        }

        private static IpAllowlistOptions On()
        {
            IpAllowlistOptions options = new IpAllowlistOptions { Enabled = true };
            options.Allowlist.Add("203.0.113.0/24");
            options.Allowlist.Add("2001:db8::/32");
            return options;
        }

        [Fact]
        public async Task Disabled_AllowsEveryone()
        {
            IpAllowlistOptions options = new IpAllowlistOptions();
            (int status, bool nextCalled) result = await Run(options, "198.51.100.1", "/x");
            Assert.True(result.nextCalled);
            Assert.Equal(200, result.status);
        }

        [Fact]
        public async Task OffByDefault()
        {
            Assert.False(new IpAllowlistOptions().Enabled);
            Assert.False(new IpAllowlistOptions().RefreshEnabled);
            await Task.CompletedTask;
        }

        [Fact]
        public async Task AllowedV4_PassesThrough()
        {
            (int status, bool nextCalled) result = await Run(On(), "203.0.113.50", "/x");
            Assert.True(result.nextCalled);
        }

        [Fact]
        public async Task AllowedV6_PassesThrough()
        {
            (int status, bool nextCalled) result = await Run(On(), "2001:db8::9", "/x");
            Assert.True(result.nextCalled);
        }

        [Fact]
        public async Task NotListed_Gets403AndWarningWithIpPathAndRule()
        {
            (int status, bool nextCalled) result = await Run(On(), "198.51.100.7", "/secret");
            Assert.False(result.nextCalled);
            Assert.Equal(403, result.status);
            LogMessage log = SecurityTestLogs.Get().Single();
            Assert.Equal(MessageType.Warning, log.messageType);
            Assert.Contains("198.51.100.7", log.message);
            Assert.Contains("/secret", log.message);
            Assert.Contains("no allowlist rule matched", log.message);
        }

        [Fact]
        public async Task NoClientAddress_IsBlockedNotAllowed()
        {
            (int status, bool nextCalled) result = await Run(On(), null, "/x");
            Assert.False(result.nextCalled);
            Assert.Equal(403, result.status);
        }

        [Fact]
        public async Task ExemptPath_SkipsAllowlist()
        {
            IpAllowlistOptions options = On();
            options.ExemptPaths.Add("/health");
            (int status, bool nextCalled) result = await Run(options, "198.51.100.7", "/health");
            Assert.True(result.nextCalled);
            (int status, bool nextCalled) other = await Run(options, "198.51.100.7", "/healthy");
            Assert.False(other.nextCalled);
        }

        [Fact]
        public async Task ProviderPreset_AllowsCloudflareAndBlocksOthers()
        {
            IpAllowlistOptions options = new IpAllowlistOptions { Enabled = true };
            options.Providers.Add("Cloudflare");
            Assert.True((await Run(options, "173.245.48.9", "/x")).nextCalled);
            Assert.True((await Run(options, "2606:4700::5", "/x")).nextCalled);
            Assert.False((await Run(options, "198.51.100.7", "/x")).nextCalled);
        }

        [Fact]
        public async Task ProviderPreset_MatchesDirectPeerNotForwardedClient()
        {
            IpAllowlistOptions options = new IpAllowlistOptions { Enabled = true };
            options.Providers.Add("Cloudflare");
            options.ClientIp.TrustedProxyProviders.Add("Cloudflare");
            options.ClientIp.HonorCfConnectingIp = true;
            // Arrives through Cloudflare: allowed whatever the visitor is.
            Assert.True((await Run(options, "173.245.48.9", "/x", cf: "198.51.100.7")).nextCalled);
            // Not through Cloudflare, forging the header with a Cloudflare address: still blocked.
            Assert.False((await Run(options, "198.51.100.9", "/x", cf: "173.245.48.9", xff: "173.245.48.9")).nextCalled);
        }

        [Fact]
        public async Task AllowlistEntry_StillMatchesResolvedClient()
        {
            IpAllowlistOptions options = On();
            options.Providers.Add("Cloudflare");
            options.ClientIp.TrustedProxyProviders.Add("Cloudflare");
            options.ClientIp.HonorCfConnectingIp = true;
            Assert.True((await Run(options, "198.51.100.9", "/x", cf: "203.0.113.5")).nextCalled == false);
            Assert.True((await Run(options, "173.245.48.9", "/x", cf: "203.0.113.5")).nextCalled);
        }

        [Fact]
        public void State_ReportsProviderRuleAsMatch()
        {
            IpAllowlistOptions options = new IpAllowlistOptions { Enabled = true };
            options.Providers.Add("cloudflare");
            Assert.Equal("Cloudflare:173.245.48.0/20", new IpAllowlistState(options).FindAllowingRule(IPAddress.Parse("173.245.48.9"), IPAddress.Parse("173.245.48.9")));
        }

        [Fact]
        public async Task ForwardedHeader_IgnoredFromUntrustedPeer()
        {
            (int status, bool nextCalled) result = await Run(On(), "198.51.100.7", "/x", xff: "203.0.113.5");
            Assert.False(result.nextCalled);
        }

        [Fact]
        public async Task ForwardedHeader_HonoredFromTrustedProxy()
        {
            IpAllowlistOptions options = On();
            options.ClientIp.TrustedProxies.Add("10.0.0.0/8");
            Assert.True((await Run(options, "10.0.0.4", "/x", xff: "203.0.113.5")).nextCalled);
            Assert.False((await Run(options, "10.0.0.4", "/x", xff: "198.51.100.5")).nextCalled);
        }

        [Fact]
        public async Task CfConnectingIp_HonoredOnlyFromTrustedProviderPeerWhenEnabled()
        {
            IpAllowlistOptions options = On();
            options.ClientIp.TrustedProxyProviders.Add("Cloudflare");
            options.ClientIp.HonorCfConnectingIp = true;
            Assert.True((await Run(options, "173.245.48.9", "/x", cf: "203.0.113.5")).nextCalled);
            Assert.False((await Run(options, "198.51.100.9", "/x", cf: "203.0.113.5")).nextCalled);
        }

        [Fact]
        public void Validate_EnabledWithNothingAllowed_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => new IpAllowlistState(new IpAllowlistOptions { Enabled = true }));
        }

        [Fact]
        public void Validate_BadEntriesThrowAtStartup()
        {
            IpAllowlistOptions badIp = On();
            badIp.Allowlist.Add("nope");
            Assert.Throws<FormatException>(() => new IpAllowlistState(badIp));
            IpAllowlistOptions badProvider = On();
            badProvider.Providers.Add("NoSuchCdn");
            Assert.Throws<ArgumentException>(() => new IpAllowlistState(badProvider));
            IpAllowlistOptions badPath = On();
            badPath.ExemptPaths.Add("health");
            Assert.Throws<ArgumentException>(() => new IpAllowlistState(badPath));
            IpAllowlistOptions badInterval = On();
            badInterval.RefreshEnabled = true;
            badInterval.RefreshInterval = TimeSpan.Zero;
            Assert.Throws<InvalidOperationException>(() => new IpAllowlistState(badInterval));
        }

        [Fact]
        public void Validate_DisabledSkipsChecks()
        {
            IpAllowlistOptions options = new IpAllowlistOptions { Enabled = false };
            options.Allowlist.Add("nope");
            options.Validate();
        }

        [Fact]
        public void Extensions_RegisterAndBuildPipeline()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddIpAllowlist(options =>
            {
                options.Enabled = true;
                options.Allowlist.Add("203.0.113.0/24");
            });
            ServiceProvider provider = services.BuildServiceProvider();
            Assert.NotNull(provider.GetRequiredService<IpAllowlistState>());
            Assert.NotNull(provider.GetRequiredService<IpAllowlistRefresher>());
            IApplicationBuilder app = new ApplicationBuilder(provider);
            Assert.NotNull(app.UseIpAllowlist());
        }

        [Fact]
        public void Extensions_InvalidOptionsThrowWhenPipelineIsBuilt()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddIpAllowlist(options => options.Enabled = true);
            IApplicationBuilder app = new ApplicationBuilder(services.BuildServiceProvider());
            Assert.Throws<InvalidOperationException>(() => app.UseIpAllowlist());
        }
    }
}
