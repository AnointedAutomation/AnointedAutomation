// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using AnointedAutomation.SSO.UI;
using Xunit;
using System;

namespace AnointedAutomation.SSO.UI.Tests
{
    public class AnointedButtonLookTests
    {
        [Fact]
        public void BuildStartUrl_NoReturnTo_ReturnsStartUrl()
        {
            Assert.Equal("/auth/anointed/start", AnointedButtonLook.BuildStartUrl("/auth/anointed/start", null, "returnTo"));
        }

        [Fact]
        public void BuildStartUrl_EncodesReturnTo()
        {
            Assert.Equal("/s?returnTo=%2Fa%20b%3Fx%3D1", AnointedButtonLook.BuildStartUrl("/s", "/a b?x=1", "returnTo"));
        }

        [Fact]
        public void BuildStartUrl_KeepsQueryAndHash()
        {
            Assert.Equal("/s?x=1&next=%2Fa#top", AnointedButtonLook.BuildStartUrl("/s?x=1#top", "/a", "next"));
        }
        [Theory]

        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void BuildStartUrl_MissingStartUrl_Throws(string startUrl)
        {
            Assert.Throws<InvalidOperationException>(() => AnointedButtonLook.BuildStartUrl(startUrl, null, "returnTo"));
        }

        [Fact]
        public void BuildStartUrl_EmptyParamWithReturnTo_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => AnointedButtonLook.BuildStartUrl("/s", "/a", " "));
        }
        [Theory]

        [InlineData("/account", "/account")]
        [InlineData("/a?b=c#d", "/a?b=c#d")]
        [InlineData("//evil.example", null)]
        [InlineData("/\\evil.example", null)]
        [InlineData("https://evil.example/", null)]
        [InlineData("javascript:alert(1)", null)]
        [InlineData("/a\nb", null)]
        [InlineData("", null)]
        [InlineData(null, null)]
        public void SafeRelativePath_OnlyKeepsSameSitePaths(string input, string expected)
        {
            Assert.Equal(expected, AnointedReturnTo.SafeRelativePath(input));
        }

        [Fact]
        public void SafeRelativePath_TooLong_ReturnsNull()
        {
            Assert.Null(AnointedReturnTo.SafeRelativePath("/" + new string('a', AnointedReturnTo.MaxLength)));
        }

        [Fact]
        public void EndpointOptions_UnsafeDefaultReturn_Throws()
        {
            AnointedAuthEndpointOptions options = new AnointedAuthEndpointOptions { DefaultReturnPath = "https://evil.example" };
            Assert.Throws<InvalidOperationException>(() => options.Validate());
        }

        [Fact]
        public void EndpointOptions_EmptyCookieScheme_Throws()
        {
            AnointedAuthEndpointOptions options = new AnointedAuthEndpointOptions { CookieScheme = "" };
            Assert.Throws<InvalidOperationException>(() => options.Validate());
        }
    }
}
