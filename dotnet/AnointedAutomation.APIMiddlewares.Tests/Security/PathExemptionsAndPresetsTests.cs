// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Net;
using AnointedAutomation.APIMiddleware.Security;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace AnointedAutomation.APIMiddlewares.Tests.Security
{
    public class PathExemptionsAndPresetsTests
    {
        [Theory]
        [InlineData("/health", true)]
        [InlineData("/HEALTH", true)]
        [InlineData("/health/live", true)]
        [InlineData("/healthz", false)]
        [InlineData("/api/webhooks/shopify", true)]
        [InlineData("/api/orders", false)]
        public void IsExempt_MatchesBySegment(string path, bool expected)
        {
            PathExemptions exemptions = new PathExemptions(new string[] { "/health", "/api/webhooks/" });
            Assert.Equal(expected, exemptions.IsExempt(new PathString(path)));
        }

        [Fact]
        public void Constructor_RejectsPrefixWithoutSlashAndRootPrefix()
        {
            Assert.Throws<ArgumentException>(() => new PathExemptions(new string[] { "health" }));
            Assert.Throws<ArgumentException>(() => new PathExemptions(new string[] { "/" }));
        }

        [Theory]
        [InlineData("Cloudflare", "173.245.48.1")]
        [InlineData("Cloudflare", "2606:4700::1")]
        [InlineData("Fastly", "151.101.1.1")]
        [InlineData("Fastly", "2a04:4e42::1")]
        [InlineData("CloudFront", "13.32.0.1")]
        public void ShippedRanges_ContainKnownEdgeAddresses(string provider, string address)
        {
            IpNetworkList list = IpNetworkList.Parse(IpProviderPresets.GetShippedRanges(provider));
            Assert.True(list.Contains(IPAddress.Parse(address)), provider + " should contain " + address);
            Assert.False(list.Contains(IPAddress.Parse("192.0.2.1")));
        }

        [Fact]
        public void ShippedRanges_AreAllValidAndSourcesDocumented()
        {
            foreach (string name in IpProviderPresets.Names)
            {
                IpProviderSource source = IpProviderPresets.GetSource(name);
                Assert.NotEmpty(source.Urls);
                Assert.All(source.Urls, url => Assert.StartsWith("https://", url, StringComparison.Ordinal));
                Assert.True(IpNetworkList.Parse(IpProviderPresets.GetShippedRanges(name)).Count > 5);
            }
        }

        [Fact]
        public void UnknownProvider_Throws()
        {
            Assert.Throws<ArgumentException>(() => IpProviderPresets.GetShippedRanges("NoSuchCdn"));
            Assert.Throws<ArgumentException>(() => IpProviderPresets.GetSource(null));
        }
    }
}
