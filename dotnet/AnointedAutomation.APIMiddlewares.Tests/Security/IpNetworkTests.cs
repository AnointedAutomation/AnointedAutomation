// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Net;
using AnointedAutomation.APIMiddleware.Security;
using Xunit;

namespace AnointedAutomation.APIMiddlewares.Tests.Security
{
    public class IpNetworkTests
    {
        [Theory]
        [InlineData("203.0.113.7", "203.0.113.7", true)]
        [InlineData("203.0.113.7", "203.0.113.8", false)]
        [InlineData("203.0.113.0/24", "203.0.113.255", true)]
        [InlineData("203.0.113.0/24", "203.0.114.0", false)]
        [InlineData("10.0.0.0/8", "10.200.1.1", true)]
        [InlineData("10.0.0.0/8", "11.0.0.1", false)]
        [InlineData("0.0.0.0/0", "8.8.8.8", true)]
        [InlineData("192.168.1.130/25", "192.168.1.200", true)]
        [InlineData("192.168.1.130/25", "192.168.1.100", false)]
        [InlineData("2001:db8::1", "2001:db8::1", true)]
        [InlineData("2001:db8::1", "2001:db8::2", false)]
        [InlineData("2001:db8::/32", "2001:db8:ffff::1", true)]
        [InlineData("2001:db8::/32", "2001:db9::1", false)]
        [InlineData("2400:cb00::/32", "2400:cb00:1:2::3", true)]
        [InlineData("2a06:98c0::/29", "2a06:98c7:ffff::1", true)]
        [InlineData("2a06:98c0::/29", "2a06:98c8::1", false)]
        public void Contains_MatchesCidr(string rule, string address, bool expected)
        {
            IpNetwork network = IpNetwork.Parse(rule);
            Assert.Equal(expected, network.Contains(IPAddress.Parse(address)));
        }

        [Fact]
        public void Contains_DoesNotMatchAcrossFamilies()
        {
            Assert.False(IpNetwork.Parse("0.0.0.0/0").Contains(IPAddress.Parse("2001:db8::1")));
            Assert.False(IpNetwork.Parse("::/0").Contains(IPAddress.Parse("8.8.8.8")));
        }

        [Fact]
        public void Contains_TreatsIpv4MappedIpv6AsIpv4()
        {
            Assert.True(IpNetwork.Parse("203.0.113.0/24").Contains(IPAddress.Parse("::ffff:203.0.113.9")));
            Assert.True(IpNetwork.Parse("::ffff:203.0.113.0/120").Contains(IPAddress.Parse("203.0.113.9")));
        }

        [Fact]
        public void Contains_NullIsFalse()
        {
            Assert.False(IpNetwork.Parse("10.0.0.0/8").Contains(null));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("not-an-ip")]
        [InlineData("10.0.0.0/33")]
        [InlineData("10.0.0.0/-1")]
        [InlineData("10.0.0.0/abc")]
        [InlineData("2001:db8::/129")]
        [InlineData("300.1.1.1")]
        [InlineData("10")]
        [InlineData("10.1")]
        [InlineData("203.0.113")]
        [InlineData("10/8")]
        [InlineData("10.1/16")]
        [InlineData("1.2.3.4.5")]
        [InlineData("::ffff:10.0.0.0/64")]
        public void TryParse_RejectsInvalid(string text)
        {
            IpNetwork network;
            Assert.False(IpNetwork.TryParse(text, out network));
            Assert.Null(network);
        }

        [Fact]
        public void Parse_ThrowsFormatExceptionOnInvalid()
        {
            Assert.Throws<FormatException>(() => IpNetwork.Parse("bogus"));
        }

        [Fact]
        public void List_FindMatchReturnsFirstMatchingRule()
        {
            IpNetworkList list = IpNetworkList.Parse(new string[] { "10.0.0.0/8", "2001:db8::/32" });
            Assert.Equal("10.0.0.0/8", list.FindMatch(IPAddress.Parse("10.1.2.3")).Text);
            Assert.Equal("2001:db8::/32", list.FindMatch(IPAddress.Parse("2001:db8::5")).Text);
            Assert.Null(list.FindMatch(IPAddress.Parse("8.8.8.8")));
            Assert.False(IpNetworkList.Empty.Contains(IPAddress.Parse("8.8.8.8")));
        }

        [Fact]
        public void List_ParseThrowsOnAnyInvalidEntry()
        {
            Assert.Throws<FormatException>(() => IpNetworkList.Parse(new string[] { "10.0.0.0/8", "oops" }));
        }
    }
}
