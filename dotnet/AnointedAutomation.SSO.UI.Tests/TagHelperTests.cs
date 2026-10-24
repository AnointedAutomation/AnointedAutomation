// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using AnointedAutomation.SSO.UI.TagHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnointedAutomation.SSO.UI.Tests
{
    [TestFixture]
    public class TagHelperTests
    {
        private static TagHelperOutput NewOutput(string tag, TagHelperAttributeList attributes)
        {
            return new TagHelperOutput(tag, attributes, (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
        }

        private static TagHelperContext NewContext(TagHelperAttributeList attributes)
        {
            return new TagHelperContext(attributes, new Dictionary<object, object>(), Guid.NewGuid().ToString("N"));
        }

        [Test]
        public void SignIn_RendersStyledLinkWithLogoAndLabel()
        {
            AnointedSignInTagHelper helper = new AnointedSignInTagHelper { StartUrl = "/auth/anointed/start", ReturnTo = "/account" };
            TagHelperOutput output = NewOutput("anointed-sign-in", new TagHelperAttributeList());
            helper.Process(NewContext(new TagHelperAttributeList()), output);

            Assert.That(output.TagName, Is.EqualTo("a"));
            Assert.That(output.Attributes["href"].Value, Is.EqualTo("/auth/anointed/start?returnTo=%2Faccount"));
            Assert.That(output.Attributes["style"].Value, Is.EqualTo(AnointedButtonLook.ButtonStyle));
            Assert.That(output.Attributes.ContainsName("data-anointed-sign-in"), Is.True);
            string content = output.Content.GetContent();
            Assert.That(content, Does.Contain("alt=\"\" aria-hidden=\"true\""));
            Assert.That(content, Does.Contain("<span>Sign in with Anointed Automation</span>"));
        }

        [Test]
        public void SignIn_WithClass_DropsInlineStyle_AndEncodesLabel()
        {
            TagHelperAttributeList attributes = new TagHelperAttributeList { { "class", "btn" } };
            AnointedSignInTagHelper helper = new AnointedSignInTagHelper { StartUrl = "/s", Label = "<b>Go</b>" };
            TagHelperOutput output = NewOutput("anointed-sign-in", attributes);
            helper.Process(NewContext(attributes), output);

            Assert.That(output.Attributes.ContainsName("style"), Is.False);
            Assert.That(output.Content.GetContent(), Does.Contain("&lt;b&gt;Go&lt;/b&gt;"));
        }

        [Test]
        public void SignIn_MissingStartUrl_Throws()
        {
            AnointedSignInTagHelper helper = new AnointedSignInTagHelper();
            Assert.Throws<InvalidOperationException>(() => helper.Process(NewContext(new TagHelperAttributeList()), NewOutput("anointed-sign-in", new TagHelperAttributeList())));
        }

        [Test]
        public void SignOut_RendersPostFormWithAntiforgeryAndReturnTo()
        {
            ServiceProvider services = new ServiceCollection().AddLogging().AddAntiforgery().BuildServiceProvider();
            DefaultHttpContext http = new DefaultHttpContext { RequestServices = services };
            AnointedSignOutTagHelper helper = new AnointedSignOutTagHelper
            {
                Action = "/auth/anointed/sign-out",
                ReturnTo = "/bye",
                ViewContext = new ViewContext { HttpContext = http },
            };
            TagHelperOutput output = NewOutput("anointed-sign-out", new TagHelperAttributeList());
            helper.Process(NewContext(new TagHelperAttributeList()), output);

            Assert.That(output.TagName, Is.EqualTo("form"));
            Assert.That(output.Attributes["method"].Value, Is.EqualTo("post"));
            Assert.That(output.Attributes["action"].Value, Is.EqualTo("/auth/anointed/sign-out"));
            string content = output.Content.GetContent();
            Assert.That(content, Does.Contain("name=\"__RequestVerificationToken\""));
            Assert.That(content, Does.Contain("name=\"returnTo\" value=\"/bye\""));
            Assert.That(content, Does.Contain("<button type=\"submit\" style="));
            Assert.That(content, Does.Contain("<span>Sign out</span>"));
        }

        [Test]
        public void SignOut_WithClass_MovesClassToButton()
        {
            TagHelperAttributeList attributes = new TagHelperAttributeList { { "class", "btn" } };
            TagHelperOutput output = NewOutput("anointed-sign-out", attributes);
            AnointedSignOutTagHelper.Render(output, "/so", null, "returnTo", "Leave", "f", "t");

            Assert.That(output.Attributes.ContainsName("class"), Is.False);
            string content = output.Content.GetContent();
            Assert.That(content, Does.Contain("<button type=\"submit\" class=\"btn\">"));
            Assert.That(content, Does.Not.Contain("name=\"returnTo\""));
        }

        [Test]
        public void SignOut_MissingAction_Throws()
        {
            AnointedSignOutTagHelper helper = new AnointedSignOutTagHelper();
            Assert.Throws<InvalidOperationException>(() => helper.Process(NewContext(new TagHelperAttributeList()), NewOutput("anointed-sign-out", new TagHelperAttributeList())));
        }
    }
}
