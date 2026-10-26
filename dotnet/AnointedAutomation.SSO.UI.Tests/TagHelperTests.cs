// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using AnointedAutomation.SSO.UI.TagHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnointedAutomation.SSO.UI.Tests
{
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

        [Fact]
        public void SignIn_RendersStyledLinkWithLogoAndLabel()
        {
            AnointedSignInTagHelper helper = new AnointedSignInTagHelper { StartUrl = "/auth/anointed/start", ReturnTo = "/account" };
            TagHelperOutput output = NewOutput("anointed-sign-in", new TagHelperAttributeList());
            helper.Process(NewContext(new TagHelperAttributeList()), output);

            Assert.Equal("a", output.TagName);
            Assert.Equal("/auth/anointed/start?returnTo=%2Faccount", output.Attributes["href"].Value);
            Assert.Equal(AnointedButtonLook.ButtonStyle, output.Attributes["style"].Value);
            Assert.True(output.Attributes.ContainsName("data-anointed-sign-in"));
            string content = output.Content.GetContent();
            Assert.Contains("alt=\"\" aria-hidden=\"true\"", content);
            Assert.Contains("<span>Sign in with Anointed Automation</span>", content);
        }

        [Fact]
        public void SignIn_WithClass_DropsInlineStyle_AndEncodesLabel()
        {
            TagHelperAttributeList attributes = new TagHelperAttributeList { { "class", "btn" } };
            AnointedSignInTagHelper helper = new AnointedSignInTagHelper { StartUrl = "/s", Label = "<b>Go</b>" };
            TagHelperOutput output = NewOutput("anointed-sign-in", attributes);
            helper.Process(NewContext(attributes), output);

            Assert.False(output.Attributes.ContainsName("style"));
            Assert.Contains("&lt;b&gt;Go&lt;/b&gt;", output.Content.GetContent());
        }

        [Fact]
        public void SignIn_MissingStartUrl_Throws()
        {
            AnointedSignInTagHelper helper = new AnointedSignInTagHelper();
            Assert.Throws<InvalidOperationException>(() => helper.Process(NewContext(new TagHelperAttributeList()), NewOutput("anointed-sign-in", new TagHelperAttributeList())));
        }

        [Fact]
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

            Assert.Equal("form", output.TagName);
            Assert.Equal("post", output.Attributes["method"].Value);
            Assert.Equal("/auth/anointed/sign-out", output.Attributes["action"].Value);
            string content = output.Content.GetContent();
            Assert.Contains("name=\"__RequestVerificationToken\"", content);
            Assert.Contains("name=\"returnTo\" value=\"/bye\"", content);
            Assert.Contains("<button type=\"submit\" style=", content);
            Assert.Contains("<span>Sign out</span>", content);
        }

        [Fact]
        public void SignOut_WithClass_MovesClassToButton()
        {
            TagHelperAttributeList attributes = new TagHelperAttributeList { { "class", "btn" } };
            TagHelperOutput output = NewOutput("anointed-sign-out", attributes);
            AnointedSignOutTagHelper.Render(output, "/so", null, "returnTo", "Leave", "f", "t");

            Assert.False(output.Attributes.ContainsName("class"));
            string content = output.Content.GetContent();
            Assert.Contains("<button type=\"submit\" class=\"btn\">", content);
            Assert.DoesNotContain("name=\"returnTo\"", content);
        }

        [Fact]
        public void SignOut_MissingAction_Throws()
        {
            AnointedSignOutTagHelper helper = new AnointedSignOutTagHelper();
            Assert.Throws<InvalidOperationException>(() => helper.Process(NewContext(new TagHelperAttributeList()), NewOutput("anointed-sign-out", new TagHelperAttributeList())));
        }
    }
}
