// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using AnointedAutomation.SSO.UI;
using NUnit.Framework;
using System;

namespace AnointedAutomation.SSO.UI.Tests
{
    [TestFixture]
    public class AnointedButtonLookTests
    {
        [Test]
        public void BuildStartUrl_NoReturnTo_ReturnsStartUrl()
        {
            Assert.That(AnointedButtonLook.BuildStartUrl("/auth/anointed/start", null, "returnTo"), Is.EqualTo("/auth/anointed/start"));
        }

        [Test]
        public void BuildStartUrl_EncodesReturnTo()
        {
            Assert.That(AnointedButtonLook.BuildStartUrl("/s", "/a b?x=1", "returnTo"), Is.EqualTo("/s?returnTo=%2Fa%20b%3Fx%3D1"));
        }

        [Test]
        public void BuildStartUrl_KeepsQueryAndHash()
        {
            Assert.That(AnointedButtonLook.BuildStartUrl("/s?x=1#top", "/a", "next"), Is.EqualTo("/s?x=1&next=%2Fa#top"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void BuildStartUrl_MissingStartUrl_Throws(string startUrl)
        {
            Assert.Throws<InvalidOperationException>(() => AnointedButtonLook.BuildStartUrl(startUrl, null, "returnTo"));
        }

        [Test]
        public void BuildStartUrl_EmptyParamWithReturnTo_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => AnointedButtonLook.BuildStartUrl("/s", "/a", " "));
        }

        [TestCase("/account", "/account")]
        [TestCase("/a?b=c#d", "/a?b=c#d")]
        [TestCase("//evil.example", null)]
        [TestCase("/\\evil.example", null)]
        [TestCase("https://evil.example/", null)]
        [TestCase("javascript:alert(1)", null)]
        [TestCase("/a\nb", null)]
        [TestCase("", null)]
        [TestCase(null, null)]
        public void SafeRelativePath_OnlyKeepsSameSitePaths(string input, string expected)
        {
            Assert.That(AnointedReturnTo.SafeRelativePath(input), Is.EqualTo(expected));
        }

        [Test]
        public void SafeRelativePath_TooLong_ReturnsNull()
        {
            Assert.That(AnointedReturnTo.SafeRelativePath("/" + new string('a', AnointedReturnTo.MaxLength)), Is.Null);
        }

        [Test]
        public void EndpointOptions_UnsafeDefaultReturn_Throws()
        {
            AnointedAuthEndpointOptions options = new AnointedAuthEndpointOptions { DefaultReturnPath = "https://evil.example" };
            Assert.Throws<InvalidOperationException>(() => options.Validate());
        }

        [Test]
        public void EndpointOptions_EmptyCookieScheme_Throws()
        {
            AnointedAuthEndpointOptions options = new AnointedAuthEndpointOptions { CookieScheme = "" };
            Assert.Throws<InvalidOperationException>(() => options.Validate());
        }
    }
}
