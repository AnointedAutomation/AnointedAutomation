// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using AnointedAutomation.SSO.Owin;
using System;
using Xunit;

namespace AnointedAutomation.SSO.Owin.Tests
{
    public class AnointedSignInButtonTests
    {
        [Fact]
        public void Default_RendersBrandLink()
        {
            string html = AnointedSignInButtonMarkup.Render(new AnointedSignInButtonOptions { StartUrl = "/auth/anointed/start" });
            Assert.StartsWith("<a href=\"/auth/anointed/start\" style=\"", html, StringComparison.Ordinal);
            Assert.Contains("data-anointed-sign-in=\"\"", html, StringComparison.Ordinal);
            Assert.Contains("<img src=\"https://www.anointedautomation.net/favicon.svg\" alt=\"\" aria-hidden=\"true\"", html, StringComparison.Ordinal);
            Assert.EndsWith("<span>Sign in with Anointed Automation</span></a>", html, StringComparison.Ordinal);
        }

        [Fact]
        public void CssClass_ReplacesInlineStyle()
        {
            string html = AnointedSignInButtonMarkup.Render(new AnointedSignInButtonOptions { StartUrl = "/s", CssClass = "btn btn-dark" });
            Assert.StartsWith("<a href=\"/s\" class=\"btn btn-dark\" data-anointed-sign-in", html, StringComparison.Ordinal);
        }

        [Fact]
        public void ReturnTo_IsEscapedAndAppended()
        {
            Assert.Equal("/s?returnTo=%2Faccount%3Ftab%3D1", AnointedSignInButtonMarkup.BuildHref(new AnointedSignInButtonOptions { StartUrl = "/s", ReturnTo = "/account?tab=1" }));
            Assert.Equal("/s?a=1&next=%2Fx", AnointedSignInButtonMarkup.BuildHref(new AnointedSignInButtonOptions { StartUrl = "/s?a=1", ReturnTo = "/x", ReturnToParam = "next" }));
        }

        [Fact]
        public void Label_AndHref_AreHtmlEncoded()
        {
            string html = AnointedSignInButtonMarkup.Render(new AnointedSignInButtonOptions { StartUrl = "/s?a=1&b=\"2\"", Label = "<b>Go</b>" });
            Assert.Contains("href=\"/s?a=1&amp;b=&quot;2&quot;\"", html, StringComparison.Ordinal);
            Assert.Contains("<span>&lt;b&gt;Go&lt;/b&gt;</span>", html, StringComparison.Ordinal);
        }

        [Fact]
        public void AbsoluteHttpsStart_Allowed()
        {
            Assert.Equal("https://www.example.com/start", AnointedSignInButtonMarkup.BuildHref(new AnointedSignInButtonOptions { StartUrl = "https://www.example.com/start" }));
        }

        [Theory]
        [InlineData(null)]
        [InlineData(" ")]
        [InlineData("javascript:alert(1)")]
        [InlineData("//evil.test/start")]
        [InlineData("/\\evil.test")]
        [InlineData("auth/start")]
        [InlineData("~/auth/start")]
        public void BadStartUrl_Throws(string startUrl)
        {
            Assert.Throws<ArgumentException>(() => AnointedSignInButtonMarkup.Render(new AnointedSignInButtonOptions { StartUrl = startUrl }));
        }

        [Theory]
        [InlineData("https://evil.test/")]
        [InlineData("//evil.test")]
        [InlineData("account")]
        public void NonLocalReturnTo_Throws(string returnTo)
        {
            Assert.Throws<ArgumentException>(() => AnointedSignInButtonMarkup.Render(new AnointedSignInButtonOptions { StartUrl = "/s", ReturnTo = returnTo }));
        }

        [Fact]
        public void EmptyLabel_Throws()
        {
            Assert.Throws<ArgumentException>(() => AnointedSignInButtonMarkup.Render(new AnointedSignInButtonOptions { StartUrl = "/s", Label = " " }));
        }
    }
}
