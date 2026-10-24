// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using AnointedAutomation.SSO.UI.Components;
using AngleSharp.Dom;
using Bunit;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;

namespace AnointedAutomation.SSO.UI.Tests
{
    [TestFixture]
    public class ComponentTests
    {
        private BunitContext _context;

        [SetUp]
        public void SetUp()
        {
            _context = new BunitContext();
            _context.Services.AddSingleton<AntiforgeryStateProvider>(new FakeAntiforgeryStateProvider());
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public void SignInButton_RendersBrandLink()
        {
            IRenderedComponent<AnointedSignInButton> cut = _context.Render<AnointedSignInButton>(p => p
                .Add(x => x.StartUrl, "/auth/anointed/start")
                .Add(x => x.ReturnTo, "/account"));

            IElement link = cut.Find("a[data-anointed-sign-in]");
            Assert.That(link.GetAttribute("href"), Is.EqualTo("/auth/anointed/start?returnTo=%2Faccount"));
            Assert.That(link.GetAttribute("data-enhance-nav"), Is.EqualTo("false"));
            Assert.That(link.GetAttribute("style"), Is.EqualTo(AnointedButtonLook.ButtonStyle));
            Assert.That(cut.Find("img").GetAttribute("aria-hidden"), Is.EqualTo("true"));
            Assert.That(cut.Find("img").GetAttribute("alt"), Is.EqualTo(string.Empty));
            Assert.That(cut.Find("span").TextContent, Is.EqualTo(AnointedButtonLook.DefaultSignInLabel));
        }

        [Test]
        public void SignInButton_WithClass_DropsInlineStyle()
        {
            IRenderedComponent<AnointedSignInButton> cut = _context.Render<AnointedSignInButton>(p => p
                .Add(x => x.StartUrl, "/s")
                .Add(x => x.Class, "my-btn")
                .Add(x => x.Label, "Continue"));

            IElement link = cut.Find("a");
            Assert.That(link.GetAttribute("class"), Is.EqualTo("my-btn"));
            Assert.That(link.HasAttribute("style"), Is.False);
            Assert.That(cut.Find("span").TextContent, Is.EqualTo("Continue"));
        }

        [Test]
        public void SignInButton_MissingStartUrl_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => _context.Render<AnointedSignInButton>());
        }

        [Test]
        public void SignOutButton_RendersPostFormWithToken()
        {
            IRenderedComponent<AnointedSignOutButton> cut = _context.Render<AnointedSignOutButton>(p => p
                .Add(x => x.Action, "/auth/anointed/sign-out")
                .Add(x => x.ReturnTo, "/bye"));

            IElement form = cut.Find("form[data-anointed-sign-out]");
            Assert.That(form.GetAttribute("method"), Is.EqualTo("post"));
            Assert.That(form.GetAttribute("action"), Is.EqualTo("/auth/anointed/sign-out"));
            Assert.That(cut.Find("input[name=__test_token]").GetAttribute("value"), Is.EqualTo("token-value"));
            Assert.That(cut.Find("input[name=returnTo]").GetAttribute("value"), Is.EqualTo("/bye"));
            Assert.That(cut.Find("button").GetAttribute("type"), Is.EqualTo("submit"));
            Assert.That(cut.Find("button span").TextContent, Is.EqualTo(AnointedButtonLook.DefaultSignOutLabel));
        }

        [Test]
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
