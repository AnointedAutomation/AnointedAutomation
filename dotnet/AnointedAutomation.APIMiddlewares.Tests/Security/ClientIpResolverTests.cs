// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Net;
using AnointedAutomation.APIMiddleware.Security;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace AnointedAutomation.APIMiddlewares.Tests.Security
{
    public class ClientIpResolverTests
    {
        private static readonly IpNetworkList Proxies = IpNetworkList.Parse(new string[] { "172.16.0.0/12", "2400:cb00::/32" });

        private static DefaultHttpContext Make(string peer, string xff, string cf)
        {
            DefaultHttpContext context = new DefaultHttpContext();
            if (peer != null)
            {
                context.Connection.RemoteIpAddress = IPAddress.Parse(peer);
            }
            if (xff != null)
            {
                context.Request.Headers["X-Forwarded-For"] = xff;
            }
            if (cf != null)
            {
                context.Request.Headers["CF-Connecting-IP"] = cf;
            }
            return context;
        }

        [Fact]
        public void UntrustedPeer_IgnoresForwardedHeaders()
        {
            DefaultHttpContext context = Make("198.51.100.9", "203.0.113.1", "203.0.113.2");
            Assert.Equal("198.51.100.9", ClientIpResolver.Resolve(context, Proxies, true).ToString());
        }

        [Fact]
        public void NoTrustedProxiesConfigured_NeverHonorsHeaders()
        {
            DefaultHttpContext context = Make("172.16.0.5", "203.0.113.1", "203.0.113.2");
            Assert.Equal("172.16.0.5", ClientIpResolver.Resolve(context, IpNetworkList.Empty, true).ToString());
        }

        [Fact]
        public void TrustedPeer_UsesXForwardedFor()
        {
            DefaultHttpContext context = Make("172.16.0.5", "203.0.113.1", null);
            Assert.Equal("203.0.113.1", ClientIpResolver.Resolve(context, Proxies, false).ToString());
        }

        [Fact]
        public void TrustedPeer_SkipsTrustedHopsFromTheRight_SoSpoofedLeftEntryIsIgnored()
        {
            DefaultHttpContext context = Make("172.16.0.5", "1.1.1.1, 203.0.113.1, 172.16.0.9", null);
            Assert.Equal("203.0.113.1", ClientIpResolver.Resolve(context, Proxies, false).ToString());
        }

        [Fact]
        public void TrustedPeer_CfConnectingIpWinsWhenEnabled()
        {
            DefaultHttpContext context = Make("2400:cb00::1", "203.0.113.1", "203.0.113.2");
            Assert.Equal("203.0.113.2", ClientIpResolver.Resolve(context, Proxies, true).ToString());
            Assert.Equal("203.0.113.1", ClientIpResolver.Resolve(context, Proxies, false).ToString());
        }

        [Fact]
        public void TrustedNonCloudflarePeer_IgnoresCfConnectingIp()
        {
            // 172.16.0.5 is a trusted proxy (a load balancer or another CDN) but not a Cloudflare edge, so a client
            // could have set CF-Connecting-IP itself; only X-Forwarded-For is believed.
            DefaultHttpContext context = Make("172.16.0.5", "203.0.113.1", "203.0.113.2");
            Assert.Equal("203.0.113.1", ClientIpResolver.Resolve(context, Proxies, true).ToString());
        }

        [Fact]
        public void TrustedNonCloudflarePeer_WithOnlyCfHeader_ReturnsPeer()
        {
            DefaultHttpContext context = Make("172.16.0.5", null, "203.0.113.2");
            Assert.Equal("172.16.0.5", ClientIpResolver.Resolve(context, Proxies, true).ToString());
        }

        [Fact]
        public void CustomCloudflareRanges_DecideWhichPeerIsBelieved()
        {
            IpNetworkList cloudflare = IpNetworkList.Parse(new string[] { "172.16.0.0/24" });
            DefaultHttpContext inRange = Make("172.16.0.5", "203.0.113.1", "203.0.113.2");
            Assert.Equal("203.0.113.2", ClientIpResolver.Resolve(inRange, Proxies, true, cloudflare).ToString());
            DefaultHttpContext shippedEdge = Make("2400:cb00::1", "203.0.113.1", "203.0.113.2");
            Assert.Equal("203.0.113.1", ClientIpResolver.Resolve(shippedEdge, Proxies, true, cloudflare).ToString());
        }

        [Fact]
        public void HonoringCfWithNullCloudflareRanges_Throws()
        {
            DefaultHttpContext context = Make("2400:cb00::1", "203.0.113.1", "203.0.113.2");
            Assert.Throws<System.ArgumentNullException>(() => ClientIpResolver.Resolve(context, Proxies, true, null));
            Assert.Equal("203.0.113.1", ClientIpResolver.Resolve(context, Proxies, false, null).ToString());
        }

        [Fact]
        public void CloudflareEdge_HoldsShippedCloudflareRanges()
        {
            Assert.True(ClientIpResolver.CloudflareEdge.Contains(IPAddress.Parse("2400:cb00::1")));
            Assert.False(ClientIpResolver.CloudflareEdge.Contains(IPAddress.Parse("172.16.0.5")));
        }

        [Fact]
        public void TrustedPeer_InvalidCfHeaderFallsBackToForwardedFor()
        {
            DefaultHttpContext context = Make("2400:cb00::1", "203.0.113.1", "garbage");
            Assert.Equal("203.0.113.1", ClientIpResolver.Resolve(context, Proxies, true).ToString());
        }

        [Fact]
        public void TrustedPeer_NoHeaders_ReturnsPeer()
        {
            DefaultHttpContext context = Make("172.16.0.5", null, null);
            Assert.Equal("172.16.0.5", ClientIpResolver.Resolve(context, Proxies, true).ToString());
        }

        [Fact]
        public void TrustedPeer_GarbledHopIsNotTrusted()
        {
            DefaultHttpContext context = Make("172.16.0.5", "203.0.113.1, not-an-ip", null);
            Assert.Equal("172.16.0.5", ClientIpResolver.Resolve(context, Proxies, false).ToString());
        }

        [Fact]
        public void TrustedPeer_AcceptsPortsAndBracketedIpv6()
        {
            Assert.Equal("203.0.113.1", ClientIpResolver.Resolve(Make("172.16.0.5", "203.0.113.1:5555", null), Proxies, false).ToString());
            Assert.Equal("2001:db8::7", ClientIpResolver.Resolve(Make("172.16.0.5", "[2001:db8::7]:443", null), Proxies, false).ToString());
        }

        [Fact]
        public void NoPeer_ReturnsNull()
        {
            Assert.Null(ClientIpResolver.Resolve(Make(null, "203.0.113.1", null), Proxies, true));
        }

        [Fact]
        public void IsPeerTrusted_ReflectsList()
        {
            Assert.True(ClientIpResolver.IsPeerTrusted(Make("172.16.0.5", null, null), Proxies));
            Assert.False(ClientIpResolver.IsPeerTrusted(Make("198.51.100.1", null, null), Proxies));
            Assert.False(ClientIpResolver.IsPeerTrusted(Make(null, null, null), Proxies));
        }

        [Fact]
        public void ClientIpOptions_BuildsFromAddressesAndProviders()
        {
            ClientIpOptions options = new ClientIpOptions();
            options.TrustedProxies.Add("10.0.0.0/8");
            options.TrustedProxyProviders.Add("cloudflare");
            IpNetworkList list = options.BuildTrustedProxies();
            Assert.True(list.Contains(IPAddress.Parse("10.1.1.1")));
            Assert.True(list.Contains(IPAddress.Parse("173.245.48.5")));
            Assert.False(list.Contains(IPAddress.Parse("8.8.8.8")));
        }

        [Fact]
        public void IsPeerCloudflare_RequiresTrustedPeerInsideCloudflareRanges()
        {
            IpNetworkList trusted = IpNetworkList.Parse(new string[] { "10.0.0.1", "173.245.48.0/20" });
            // Trusted and a Cloudflare edge.
            Assert.True(ClientIpResolver.IsPeerCloudflare(Make("173.245.48.9", null, null), trusted, ClientIpResolver.CloudflareEdge));
            // Trusted but not Cloudflare.
            Assert.False(ClientIpResolver.IsPeerCloudflare(Make("10.0.0.1", null, null), trusted, ClientIpResolver.CloudflareEdge));
            // Cloudflare but not trusted.
            Assert.False(ClientIpResolver.IsPeerCloudflare(Make("173.245.48.9", null, null), IpNetworkList.Parse(new string[] { "10.0.0.1" }), ClientIpResolver.CloudflareEdge));
            // No peer, no trusted list.
            Assert.False(ClientIpResolver.IsPeerCloudflare(Make(null, null, null), trusted, ClientIpResolver.CloudflareEdge));
            Assert.False(ClientIpResolver.IsPeerCloudflare(Make("173.245.48.9", null, null), null, ClientIpResolver.CloudflareEdge));
            Assert.Throws<ArgumentNullException>(() => ClientIpResolver.IsPeerCloudflare(Make("173.245.48.9", null, null), trusted, null));
        }
    }
}
