// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace AnointedAutomation.SSO.UI.TagHelpers
{
    /// <summary>
    /// <c>&lt;anointed-sign-in start-url="/auth/anointed/start" return-to="/account" /&gt;</c> renders the brand
    /// "Sign in with Anointed Automation" link styled as a button. A real link: keyboard and screen readers work
    /// and no script is needed. Pass <c>class</c> to drop the built-in inline look and style it yourself.
    /// Add <c>@addTagHelper *, AnointedAutomation.SSO.UI</c> to <c>_ViewImports.cshtml</c>.
    /// </summary>
    [HtmlTargetElement("anointed-sign-in", TagStructure = TagStructure.WithoutEndTag)]
    public class AnointedSignInTagHelper : TagHelper
    {
        /// <summary>Required. Path or URL of your start route (see <c>MapAnointedAutomationAuth</c>).</summary>
        [HtmlAttributeName("start-url")]
        public string StartUrl { get; set; }

        /// <summary>Where to land after sign-in. Omit for none.</summary>
        [HtmlAttributeName("return-to")]
        public string ReturnTo { get; set; }

        /// <summary>The parameter your server reads. Default <c>returnTo</c>.</summary>
        [HtmlAttributeName("return-to-param")]
        public string ReturnToParam { get; set; } = AnointedButtonLook.DefaultReturnToParam;

        /// <summary>Button text. Default "Sign in with Anointed Automation".</summary>
        [HtmlAttributeName("label")]
        public string Label { get; set; } = AnointedButtonLook.DefaultSignInLabel;

        /// <inheritdoc />
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }
            string href = AnointedButtonLook.BuildStartUrl(StartUrl, ReturnTo, ReturnToParam);
            output.TagName = "a";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("href", href);
            output.Attributes.SetAttribute("data-anointed-sign-in", string.Empty);
            if (!output.Attributes.ContainsName("class"))
            {
                output.Attributes.SetAttribute("style", AnointedButtonLook.ButtonStyle);
            }
            output.Content.SetHtmlContent(AnointedTagMarkup.LogoAndLabel(Label));
        }
    }
}
