// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using AnointedAutomation.SSO.UI.Components;
using AngleSharp.Dom;
using Bunit;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System;

namespace AnointedAutomation.SSO.UI.Tests
{
    public class ComponentTests : IDisposable
    {
        private BunitContext _context;

        public ComponentTests()
        {
            _context = new BunitContext();
            _context.Services.AddSingleton<AntiforgeryStateProvider>(new FakeAntiforgeryStateProvider());
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public void SignInButton_RendersBrandLink()
        {
            IRenderedComponent<AnointedSignInButton> cut = _context.Render<AnointedSignInButton>(p => p
                .Add(x => x.StartUrl, "/auth/anointed/start")
                .Add(x => x.ReturnTo, "/account"));

            IElement link = cut.Find("a[data-anointed-sign-in]");
            Assert.Equal("/auth/anointed/start?returnTo=%2Faccount", link.GetAttribute("href"));
            Assert.Equal("false", link.GetAttribute("data-enhance-nav"));
            Assert.Equal(AnointedButtonLook.ButtonStyle, link.GetAttribute("style"));
            Assert.Equal("true", cut.Find("img").GetAttribute("aria-hidden"));
            Assert.Equal(string.Empty, cut.Find("img").GetAttribute("alt"));
            Assert.Equal(AnointedButtonLook.DefaultSignInLabel, cut.Find("span").TextContent);
        }

        [Fact]
        public void SignInButton_WithClass_DropsInlineStyle()
        {
            IRenderedComponent<AnointedSignInButton> cut = _context.Render<AnointedSignInButton>(p => p
                .Add(x => x.StartUrl, "/s")
                .Add(x => x.Class, "my-btn")
                .Add(x => x.Label, "Continue"));

            IElement link = cut.Find("a");
            Assert.Equal("my-btn", link.GetAttribute("class"));
            Assert.False(link.HasAttribute("style"));
            Assert.Equal("Continue", cut.Find("span").TextContent);
        }

        [Fact]
        public void SignInButton_MissingStartUrl_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => _context.Render<AnointedSignInButton>());
        }

        [Fact]
        public void SignOutButton_RendersPostFormWithToken()
        {
            IRenderedComponent<AnointedSignOutButton> cut = _context.Render<AnointedSignOutButton>(p => p
                .Add(x => x.Action, "/auth/anointed/sign-out")
                .Add(x => x.ReturnTo, "/bye"));

            IElement form = cut.Find("form[data-anointed-sign-out]");
            Assert.Equal("post", form.GetAttribute("method"));
            Assert.Equal("/auth/anointed/sign-out", form.GetAttribute("action"));
            Assert.Equal("token-value", cut.Find("input[name=__test_token]").GetAttribute("value"));
            Assert.Equal("/bye", cut.Find("input[name=returnTo]").GetAttribute("value"));
            Assert.Equal("submit", cut.Find("button").GetAttribute("type"));
            Assert.Equal(AnointedButtonLook.DefaultSignOutLabel, cut.Find("button span").TextContent);
        }

        [Fact]
        public void SignOutButton_MissingAction_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => _context.Render<AnointedSignOutButton>());
        }

        private sealed class FakeAntiforgeryStateProvider : AntiforgeryStateProvider
        {
            public override AntiforgeryRequestToken GetAntiforgeryToken()
            {
                return new AntiforgeryRequestToken("token-value", "__test_token");
            }
        }
    }
}
